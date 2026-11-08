#include <Arduino.h>
#include "types.h" 
#include "reader.h"
#include "wifi_connection.h"
#include "mqtt.h"
#include "display.h"

QueueHandle_t eventQueue, displayQueue, httpQueue;

void controllerTask(void*) {
  bool aulaAtiva = false;
  char aulaId[GUID_LEN] = {""};
  char professorTagId[UID_LEN] = {""};

  mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente

  AppEvent ev;
  for (;;) {
    if (xQueueReceive(eventQueue, &ev, portMAX_DELAY) == pdTRUE) {
      switch (ev.type) {
        case EVT_TAG:
          if (!aulaAtiva && ev.tag.lado == LADO_INTERNO) {
            Serial.printf("[ctrl] tag %s no leitor interno, mas não há aula ativa.\n", ev.tag.uid);
            break;
          }

          // publica evento na queue para marcar a presenca do aluno no servidor.

          // tirar essas linhas abaixo pois elas serão executadas no 'case EVT_HTTP_RESULTADO' quando o resultado do HTTP chegar.
          // nesse case apeganas pegamos o que veio do evento de tag e colocamos na queue para fazer a req http.
          mostrar(LADO_INTERNO, "Presenca OK",   "Maria Silva", 2000);     // volta sozinho
          Serial.printf("[ctrl] tag %s no leitor %s\n", ev.tag.uid, ev.tag.lado == LADO_INTERNO ? "interno" : "externo");
          break;

        case EVT_BTN_CONFIRMA:
          break;

        case EVT_BTN_CANCELA:
          break;

        case EVT_AULA_INICIOU:
          if (aulaAtiva) {
            Serial.printf("[ctrl] evento de aula iniciada recebido, mas já havia aula ativa (ativa: %s, recebida: %s).\n", aulaId, ev.aula.aulaId);
            break;
          }

          aulaAtiva = true;
          strlcpy(aulaId, ev.aula.aulaId, sizeof(ev.aula.aulaId));
          strlcpy(professorTagId, ev.aula.professorTagId, sizeof(ev.aula.professorTagId));

          mostrar(LADO_INTERNO, "Aula iniciada", "", 3000);               // temporaria
          mostrar(LADO_INTERNO, "Aproxime uma tag", ">>            <<");  // permanente
          Serial.printf("[ctrl] aula com id %s iniciada para o professor com tag %s.\n", ev.aula.aulaId, ev.aula.professorTagId);
          break;

        case EVT_AULA_FINALIZOU:
          if (!aulaAtiva) {
            Serial.println("[ctrl] evento de aula finalizada recebido, mas não havia aula ativa.");
            break;
          }

          if (strcmp(aulaId, ev.aula.aulaId) != 0) {
            Serial.printf("[ctrl] evento de aula finalizada recebido, mas o id da aula não corresponde à aula ativa (ativa: %s, recebida: %s).\n", aulaId, ev.aula.aulaId);
            break;
          }

          if (strcmp(professorTagId, ev.aula.professorTagId) != 0) {
            Serial.printf("[ctrl] evento de aula finalizada recebido, mas o id da tag do professor não corresponde ao professor que iniciou a aula (ativa: %s, recebida: %s).\n", professorTagId, ev.aula.professorTagId);
            break;
          }
          
          aulaAtiva = false;
          strlcpy(aulaId, "", sizeof(aulaId));
          strlcpy(professorTagId, "", sizeof(professorTagId));

          mostrar(LADO_INTERNO, "Aula finalizada", "Obrigado!", 3000);     // temporaria
          mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente
          Serial.printf("[ctrl] aula com id %s finalizada pelo professor com tag %s.\n", ev.aula.aulaId, ev.aula.professorTagId);
          break;

        case EVT_HTTP_RESULTADO:
          break;

        default:
          break;
      }
    }

    Serial.println("[ctrl] aguardando evento...");
  }
}

void setup() {
  Serial.begin(115200);  
  
  eventQueue    = xQueueCreate(16, sizeof(AppEvent));
  displayQueue  = xQueueCreate(8,  sizeof(DisplayMsg));
  httpQueue     = xQueueCreate(8,  sizeof(HttpRequest));

  displayInit();
  readersInit();

  connectToWiFi();
  mqttInit();

  xTaskCreatePinnedToCore(readerTask,     "reader",   4096, nullptr, 3, nullptr, 1);
  xTaskCreatePinnedToCore(displayTask,    "display",  4096, nullptr, 1, nullptr, 1);
  xTaskCreatePinnedToCore(controllerTask, "ctrl",     4096, nullptr, 2, nullptr, 1);
  xTaskCreatePinnedToCore(mqttTask,       "mqtt",     4096, nullptr, 2, nullptr, 0);

  mostrar(LADO_INTERNO, "Sistema de", "Presenca pronto", 3000);  // permanente
  //mostrar(LADO_EXTERNO, "Sistema de", "Presenca pronto", 3000);  // permanente
}

void loop() { vTaskDelete(NULL); }