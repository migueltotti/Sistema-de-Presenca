#ifndef READER_H
#define READER_H

#include <Arduino.h>
#include <SPI.h>
#include <MFRC522.h>
#include "types.h"

extern QueueHandle_t eventQueue;   // definida no .ino principal

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
#define SS_INTERNO_PIN  32
#define SS_EXTERNO_PIN  23
#define SCK_PIN         25
#define MISO_PIN        27
#define MOSI_PIN        26
#define RST_PIN         33

static MFRC522 rfidInterno(SS_INTERNO_PIN, RST_PIN);   // Create MFRC522 instance.
static MFRC522 rfidExterno(SS_EXTERNO_PIN, RST_PIN);   // Create MFRC522 instance.

static MFRC522* leitores[1]  = { &rfidInterno, /*&rfidExterno*/ };

static void uidParaHex(const MFRC522::Uid& uid, char* out) {
  for (byte i = 0; i < uid.size; i++) sprintf(out + i * 2, "%02X", uid.uidByte[i]);
  out[uid.size * 2] = '\0';
}

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
  uint32_t bloqueadoAte[1] = { 0, /*0*/ };

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

static void readersInit() {
  SPI.begin(SCK_PIN, MISO_PIN, MOSI_PIN);         // Init SPI bus
  rfidInterno.PCD_Init();                         // Init MFRC522 reader interno
  rfidInterno.PCD_DumpVersionToSerial(); 
  
  for (uint8_t i = 0; i < 1; i++) {
    leitores[i]->PCD_Init();
    leitores[i]->PCD_DumpVersionToSerial();
  }// Show details of PCD - MFRC522 Card Reader details
}

#endif