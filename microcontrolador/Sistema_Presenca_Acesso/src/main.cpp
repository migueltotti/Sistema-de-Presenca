#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include <MFRC522.h>
#include "types.h" 
#include "functions.h"

#define SS_INTERNO_PIN  32
#define SCK_PIN         25
#define MISO_PIN        27
#define MOSI_PIN        26
#define RST_PIN         33

MFRC522 rfidInterno(SS_INTERNO_PIN, RST_PIN);   // Create MFRC522 instance.

QueueHandle_t eventQueue;

// Compartilhada com a futura versão por IRQ: lê o UID e publica o evento.
static bool publicarTag(MFRC522& leitor, Lado lado) {
  if (!leitor.PICC_ReadCardSerial()) return false;

  AppEvent ev{};
  ev.type = EVT_TAG;
  ev.tag.lado = lado;
  uidParaHex(leitor.uid, ev.tag.uid);

  leitor.PICC_HaltA();          // tag para de responder até sair do campo
  leitor.PCD_StopCrypto1();     // TALVEZ REMOVER

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[reader] eventQueue cheia, evento descartado.");
  }
  else{
    Serial.println("[reader] evento publicado.");
  }
  return true;
}

void readerTask(void*) {
  MFRC522* leitores[1]  = { &rfidInterno };
  uint32_t bloqueadoAte[1] = { 0 };

  for (;;) {
    for (uint8_t i = 0; i < 1; i++) {
      if ((int32_t)(bloqueadoAte[i] - millis()) > 0) continue;

      if (leitores[i]->PICC_IsNewCardPresent() && publicarTag(*leitores[i], (Lado)i)) {
        bloqueadoAte[i] = millis() + 1500;   // ignora releituras por 1,5 s
      }
    }
    vTaskDelay(pdMS_TO_TICKS(50));           // cede a CPU; sem isso a task ocuparia o núcleo
  }
}

void controllerTask(void*) {
  AppEvent ev;
  for (;;) {
    if (xQueueReceive(eventQueue, &ev, portMAX_DELAY) == pdTRUE) {
      switch (ev.type) {
        case EVT_TAG:
          Serial.printf("[ctrl] tag %s no leitor %s\n", ev.tag.uid, ev.tag.lado == LADO_INTERNO ? "interno" : "externo");
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
  
  eventQueue = xQueueCreate(16, sizeof(AppEvent));

  SPI.begin(SCK_PIN, MISO_PIN, MOSI_PIN);         // Init SPI bus
  rfidInterno.PCD_Init();                         // Init MFRC522 reader interno
  rfidInterno.PCD_DumpVersionToSerial();          // Show details of PCD - MFRC522 Card Reader details

  xTaskCreatePinnedToCore(readerTask,     "reader", 4096, nullptr, 3, nullptr, 1);
  xTaskCreatePinnedToCore(controllerTask, "ctrl",   4096, nullptr, 2, nullptr, 1);
}

void loop() { vTaskDelete(NULL); }