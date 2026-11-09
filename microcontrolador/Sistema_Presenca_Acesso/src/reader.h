#ifndef READER_H
#define READER_H

#include <Arduino.h>
#include <SPI.h>
#include <MFRC522.h>
#include "types.h"

extern QueueHandle_t eventQueue;   // definida no main.cpp principal
static TaskHandle_t  readerTaskHandle = nullptr;

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
#define SS_INTERNO_PIN  32
#define SS_EXTERNO_PIN  23
#define SCK_PIN         25
#define MISO_PIN        27
#define MOSI_PIN        26
#define RST_PIN         33

#define REARM_INTERVAL_MS 100

constexpr uint8_t  PIN_IRQ_INTERNO = 13;
constexpr uint8_t  PIN_IRQ_EXTERNO = 14;
constexpr uint32_t BIT_INTERNO = 1 << 0;
constexpr uint32_t BIT_EXTERNO = 1 << 1;

static MFRC522 rfidInterno(SS_INTERNO_PIN, RST_PIN);
static MFRC522 rfidExterno(SS_EXTERNO_PIN, RST_PIN);

void IRAM_ATTR internalISR() {
  BaseType_t woken = pdFALSE;
  xTaskNotifyFromISR(readerTaskHandle, BIT_INTERNO, eSetBits, &woken);
  if (woken) portYIELD_FROM_ISR();
}

void IRAM_ATTR externalISR() {
  BaseType_t woken = pdFALSE;
  xTaskNotifyFromISR(readerTaskHandle, BIT_EXTERNO, eSetBits, &woken);
  if (woken) portYIELD_FROM_ISR();
}

static void clearIrq(MFRC522& leitor) {
  leitor.PCD_WriteRegister(MFRC522::ComIrqReg, 0x7F);
}

// Rearma o receptor: manda um REQA e deixa o chip esperando a resposta (ATQA).
// Quando uma tag responder, o bit RxIRq sobe e o pino IRQ cai.
static void armReceiver(MFRC522& leitor) {
  leitor.PCD_WriteRegister(MFRC522::CommandReg, MFRC522::PCD_Idle);   // para qualquer comando
  leitor.PCD_WriteRegister(MFRC522::FIFOLevelReg, 0x80);              // limpa o FIFO
  clearIrq(leitor);                                                       // limpa flags antigas
  leitor.PCD_WriteRegister(MFRC522::FIFODataReg, MFRC522::PICC_CMD_REQA);
  leitor.PCD_WriteRegister(MFRC522::CommandReg, MFRC522::PCD_Transceive);
  leitor.PCD_WriteRegister(MFRC522::BitFramingReg, 0x87);             // StartSend + 7 bits (frame curto do REQA)
}

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
  clearIrq(leitor);

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[reader] eventQueue cheia, evento descartado.");
  }

  return true;
}

void readerTask(void*) {
  armReceiver(rfidInterno);
  armReceiver(rfidExterno);

  for (;;) {
    uint32_t bits = 0;
    // Dorme até uma ISR avisar. Se passar 100 ms sem aviso, segue para o rearme.
    xTaskNotifyWait(0, UINT32_MAX, &bits, pdMS_TO_TICKS(REARM_INTERVAL_MS));

    if (bits & BIT_INTERNO) publicarTag(rfidInterno, LADO_INTERNO);
    if (bits & BIT_EXTERNO) publicarTag(rfidExterno, LADO_EXTERNO);

    uint32_t descarte;
    xTaskNotifyWait(0, UINT32_MAX, &descarte, 0);   // zera bits pendentes, sem bloquear

    armReceiver(rfidInterno);   // REQA + Transceive
    armReceiver(rfidExterno);
  }
}

static void readersInit() {
  pinMode(SS_INTERNO_PIN, OUTPUT);  digitalWrite(SS_INTERNO_PIN, HIGH);
  pinMode(SS_EXTERNO_PIN, OUTPUT);  digitalWrite(SS_EXTERNO_PIN, HIGH);

  SPI.begin(SCK_PIN, MISO_PIN, MOSI_PIN);         // Init SPI bus
  
  rfidInterno.PCD_Init();
  rfidExterno.PCD_Init();
  
  rfidInterno.PCD_DumpVersionToSerial();
  rfidExterno.PCD_DumpVersionToSerial();
  
  // ComIEnReg: bit7 (IRqInv) = 1 -> pino IRQ ativo em nível baixo
  //            bit5 (RxIEn)  = 1 -> interrupção de recepção habilitada
  rfidInterno.PCD_WriteRegister(MFRC522::ComIEnReg, 0xA0);
  rfidExterno.PCD_WriteRegister(MFRC522::ComIEnReg, 0xA0);

  clearIrq(rfidInterno);
  clearIrq(rfidExterno);
}

static void configureReadersIRQs() {
  pinMode(PIN_IRQ_INTERNO, INPUT_PULLUP);
  pinMode(PIN_IRQ_EXTERNO, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(PIN_IRQ_INTERNO), internalISR, FALLING);
  attachInterrupt(digitalPinToInterrupt(PIN_IRQ_EXTERNO), externalISR, FALLING);
}

#endif