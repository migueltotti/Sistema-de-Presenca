#include <Arduino.h>
#include "types.h" 
#include "reader.h"
#include "wifi_connection.h"
#include "mqtt.h"
#include "display.h"

QueueHandle_t eventQueue, displayQueue;

void controllerTask(void*) {
  mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente

  AppEvent ev;
  for (;;) {
    if (xQueueReceive(eventQueue, &ev, portMAX_DELAY) == pdTRUE) {
      switch (ev.type) {
        case EVT_TAG:
          mostrar(LADO_INTERNO, "Presenca OK",   "Maria Silva", 2000);     // volta sozinho
          Serial.printf("[ctrl] tag %s no leitor %s\n", ev.tag.uid, ev.tag.lado == LADO_INTERNO ? "interno" : "externo");
          break;

        case EVT_BTN_CONFIRMA:
          break;

        case EVT_BTN_CANCELA:
          break;

        case EVT_AULA_INICIOU:
          mostrar(LADO_INTERNO, "Aula iniciada", "Aproxime a tag");        // permanente
          Serial.printf("[ctrl] aula com id %s iniciada para o professor %s.\n", ev.aula.aulaId, ev.aula.professorId);
          break;

        case EVT_AULA_FINALIZOU:
          mostrar(LADO_INTERNO, "Aula finalizada", "Obrigado!", 3000);     // temporaria
          Serial.printf("[ctrl] aula com id %s finalizada para o professor %s.\n", ev.aula.aulaId, ev.aula.professorId);
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

  displayInit();
  readersInit();

  connectToWiFi();
  mqttInit();

  xTaskCreatePinnedToCore(readerTask,     "reader",   4096, nullptr, 3, nullptr, 1);
  xTaskCreatePinnedToCore(displayTask,    "display",  4096, nullptr, 1, nullptr, 1);
  xTaskCreatePinnedToCore(controllerTask, "ctrl",     4096, nullptr, 2, nullptr, 1);
  xTaskCreatePinnedToCore(mqttTask,       "mqtt",     4096, nullptr, 2, nullptr, 0);
}

void loop() { vTaskDelete(NULL); }