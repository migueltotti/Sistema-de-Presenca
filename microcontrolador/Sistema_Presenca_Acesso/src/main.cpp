#include <Arduino.h>
#include "types.h" 
#include "reader.h"
#include "wifi_connection.h"
#include "mqtt.h"
#include "display.h"

QueueHandle_t eventQueue, displayQueue, httpQueue;

bool aulaAtiva = false;
char sessionId[GUID_LEN] = {""};
char professorTagId[UID_LEN] = {""};

bool portaFechada = false; 

Fluxo     fluxo = FLUXO_OCIOSO;
TickType_t prazo = 0;                 // instante limite do passo atual
char      tagPendente[UID_LEN] = {""};// passo 2: tag de quem iniciou
bool      finalizarAula = false;      // passo 4: intenção de finalizar a aula
int       requestId = 0;              // incrementa a cada requisição
int       requestPendente = -1;       // requisição que estamos aguardando

void atualizarDisplayExterno() {
  mostrar(LADO_EXTERNO,
          portaFechada ? "Porta Fechada" : "Porta Aberta",
          aulaAtiva ? "Aula em curso" : (portaFechada ? ">> Bloqueado  <<" : ">>  Liberado  <<"));
}

void armarPrazo(uint32_t ms) {
  prazo = xTaskGetTickCount() + pdMS_TO_TICKS(ms);
};

void voltarAoEstadoOcioso() {
  fluxo = FLUXO_OCIOSO;
  tagPendente[0] = '\0';
  finalizarAula = false;
  requestPendente = -1;
  atualizarDisplayExterno();
};

void enviarRequisicaoTrancarPorta() {
  HttpRequest req = {};
  req.requestId = ++requestId;
  strlcpy(req.uid, tagPendente, sizeof(req.uid));

  if (finalizarAula) {
    strlcpy(req.sessionId, sessionId, sizeof(req.sessionId));
  }

  if (xQueueSend(httpQueue, &req, 0) != pdTRUE) {
    mostrar(LADO_EXTERNO, "Sistema ocupado", "Tente de novo", 2000);
    voltarAoEstadoOcioso();
    return;
  }

  requestPendente = req.requestId;
  fluxo = FLUXO_TRANCAR_AGUARDA_HTTP;
  armarPrazo(TIMEOUT_HTTP_MS);
  mostrar(LADO_EXTERNO, "Validando...", "Aguarde");
};

void controllerTask(void*) {
  mostrar(LADO_INTERNO, "Sistema de", "Presenca pronto", 3000);
  mostrar(LADO_EXTERNO, "Sistema de", "Presenca pronto", 3000);

  mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento", 0);
  atualizarDisplayExterno();

  AppEvent ev;
  for (;;) {
    // Sem fluxo em andamento: espera para sempre. Com fluxo: espera só até o prazo.
    TickType_t tempoEspera = portMAX_DELAY;
    if (fluxo != FLUXO_OCIOSO) {
      int32_t restante = (int32_t)(prazo - xTaskGetTickCount());
      tempoEspera = restante > 0 ? (TickType_t)restante : 0;
    }

    if (xQueueReceive(eventQueue, &ev, tempoEspera) != pdTRUE) {
      Serial.printf("[ctrl] timeout no fluxo (estado %d)\n", (int)fluxo);
      if (fluxo == FLUXO_TRANCAR_AGUARDA_HTTP) {
        mostrar(LADO_EXTERNO, "Sem resposta", "Tente de novo", 2000);
        voltarAoEstadoOcioso();
      } else {
        mostrar(LADO_EXTERNO, "Tempo esgotado", "", 2000);
        voltarAoEstadoOcioso();
      }
      continue;
    }

    switch (ev.type) {
      case EVT_TAG:
        Serial.printf("[ctrl] tag %s no leitor %s\n", ev.tag.uid, ev.tag.lado == LADO_INTERNO ? "interno" : "externo");

        if (fluxo != FLUXO_OCIOSO) break; // ignora caso ja tenha algum fluxo em andamento (ex.: confirmacao de fechar a porta)

        if (ev.tag.lado == LADO_INTERNO) {
          // TODO: modificar depois para o caso de uso de abrir a porta
          // TODO: publicar na httpQueue a requisição de presença do aluno
          // e tratar o resultado em EVT_HTTP_RESULTADO (hoje é só o mock).
          mostrar(LADO_INTERNO, "Presenca OK", "Maria Silva", 2000);
        } else if (ev.tag.lado == LADO_EXTERNO) {
          if (portaFechada) {
            // TODO: fluxo de ABRIR a porta
          } else {
            strlcpy(tagPendente, ev.tag.uid, sizeof(tagPendente));

            fluxo = FLUXO_CONFIRMA_FECHAR;
            armarPrazo(TIMEOUT_CONFIRMACAO_MS);
            mostrar(LADO_EXTERNO, "Fechar a sala?", "CONFIRMA/CANCELA");
          }
        }
        
        break;

      case EVT_BTN_CONFIRMA:
        // aguardando desenvolvimento da buttonTask para testar
        // TODO: modificar depois para o caso de uso de abrir a porta
        if (fluxo == FLUXO_CONFIRMA_FECHAR) {
          if (aulaAtiva) {
            fluxo = FLUXO_CONFIRMA_FINALIZAR;
            armarPrazo(TIMEOUT_CONFIRMACAO_MS);
            mostrar(LADO_EXTERNO, "Finalizar aula?", "CONFIRMA/CANCELA");
          } else {
            finalizarAula = false;
            enviarRequisicaoTrancarPorta();
          }
        } else if (fluxo == FLUXO_CONFIRMA_FINALIZAR) {
          finalizarAula = aulaAtiva;
          enviarRequisicaoTrancarPorta();
        }
        break;

      case EVT_BTN_CANCELA:
        // aguardando desenvolvimento da buttonTask para testar
        // TODO: modificar depois para o caso de uso de abrir a porta
        if (fluxo == FLUXO_CONFIRMA_FECHAR) {
          mostrar(LADO_EXTERNO, "Cancelado", "", 2000);
          voltarAoEstadoOcioso();
        } else if (fluxo == FLUXO_CONFIRMA_FINALIZAR) {
          finalizarAula = false;
          enviarRequisicaoTrancarPorta();
        }
        break;

      case EVT_INICIAR_REGISTRO_AULA:
        if (aulaAtiva) {
          Serial.printf("[ctrl] evento de aula iniciada recebido, mas já havia aula ativa (ativa: %s, recebida: %s).\n", sessionId, ev.aula.sessionId);
          break;
        }

        aulaAtiva = true;
        strlcpy(sessionId, ev.aula.sessionId, sizeof(ev.aula.sessionId));
        strlcpy(professorTagId, ev.aula.professorTagId, sizeof(ev.aula.professorTagId));

        mostrar(LADO_INTERNO, "Aula iniciada", "", 3000);               // temporaria
        mostrar(LADO_INTERNO, "Aproxime uma tag", ">>            <<");  // permanente
        Serial.printf("[ctrl] aula com id %s iniciada para o professor com tag %s.\n", ev.aula.sessionId, ev.aula.professorTagId);
        break;

      case EVT_FINALIZAR_REGISTRO_AULA:
        if (!aulaAtiva) {
          Serial.println("[ctrl] evento de aula finalizada recebido, mas não havia aula ativa.");
          break;
        }

        if (strcmp(sessionId, ev.aula.sessionId) != 0) {
          Serial.printf("[ctrl] evento de aula finalizada recebido, mas o id da aula não corresponde à aula ativa (ativa: %s, recebida: %s).\n", sessionId, ev.aula.sessionId);
          break;
        }
        
        aulaAtiva = false;
        strlcpy(sessionId, "", sizeof(sessionId));
        strlcpy(professorTagId, "", sizeof(professorTagId));

        mostrar(LADO_INTERNO, "Aula finalizada", "Obrigado!", 3000);     // temporaria
        mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente
        Serial.printf("[ctrl] aula com id %s finalizada pelo professor com tag %s.\n", ev.aula.sessionId, ev.aula.professorTagId);
        break;

      case EVT_HTTP_RESULTADO:
        // aguardando desenvolvimento da httpTask para testar
        // TODO: modificar depois para o caso de uso de abrir a porta
        // Ignora resultado atrasado/de outra requisição (ex.: já estourou o timeout)
        if (fluxo != FLUXO_TRANCAR_AGUARDA_HTTP || ev.http.requestId != requestPendente) {
          Serial.printf("[ctrl] resultado HTTP ignorado (id %d, esperado %d)\n", ev.http.requestId, requestPendente);
          break;
        }

        if (ev.http.httpStatus == 200) {
          portaFechada = fluxo == FLUXO_TRANCAR_AGUARDA_HTTP ? true : false;
          mostrar(
            LADO_EXTERNO,
            fluxo == FLUXO_TRANCAR_AGUARDA_HTTP ? "Sala trancada" : "Sala destrancada",
            "",
            2000);
          voltarAoEstadoOcioso();
        } else if (ev.http.httpStatus == 400) {
          mostrar(LADO_EXTERNO, "Acesso negado", ev.http.mensagem, 2000);
          voltarAoEstadoOcioso();
        } else {
          mostrar(
            LADO_EXTERNO, 
            fluxo == FLUXO_TRANCAR_AGUARDA_HTTP ? "Falha ao trancar" : "Falha ao abrir",
            "Tente de novo",
            2000);
          voltarAoEstadoOcioso();
        }
        break;

      default:
        break;
    }

    Serial.println("[ctrl] aguardando evento...");
  }
}

void setup() {
  Serial.begin(115200);  
  
  eventQueue    = xQueueCreate(16, sizeof(AppEvent));
  displayQueue  = xQueueCreate(8,  sizeof(DisplayMsg));
  //httpQueue     = xQueueCreate(8,  sizeof(HttpRequest));

  displayInit();
  readersInit();

  //connectToWiFi();
  //mqttInit();

  xTaskCreatePinnedToCore(readerTask,     "reader",   4096, nullptr, 3, &readerTaskHandle, 1);
  xTaskCreatePinnedToCore(displayTask,    "display",  4096, nullptr, 1, nullptr, 1);
  xTaskCreatePinnedToCore(controllerTask, "ctrl",     4096, nullptr, 2, nullptr, 1);
  //xTaskCreatePinnedToCore(mqttTask,       "mqtt",     4096, nullptr, 2, nullptr, 0);

  configureReadersIRQs();
}

void loop() { vTaskDelete(NULL); }