#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN  32
#define RST_PIN 33   
#define IRQ_PIN 13       

MFRC522 mfrc522(SS_PIN, RST_PIN);   // Create MFRC522 instance.

bool flag = false;

void IRAM_ATTR irqReader() {
  flag = true;
}

void setup() {
  Serial.begin(115200);               // Initialize serial communications with the PC
  SPI.begin(25, 27, 26, SS_PIN);      // Init SPI bus
  mfrc522.PCD_Init();                 // Init MFRC522 card
  mfrc522.PCD_DumpVersionToSerial();  // Show details of PCD - MFRC522 Card Reader details
  pinMode(IRQ_PIN, INPUT_PULLUP);     // Set IRQ pin as input with pull-up resistor

  mfrc522.PCD_WriteRegister(mfrc522.ComIEnReg, 0xA0); // Enable interrupts for Rx and Idle
  attachInterrupt(digitalPinToInterrupt(IRQ_PIN), irqReader, FALLING); // Attach interrupt to IRQ pin, trigger on falling edge
}

int count = 0;
void loop() {

  if (flag) {

    if (!mfrc522.PICC_IsNewCardPresent())
      return;
    
    if (!mfrc522.PICC_ReadCardSerial())
      return;

    // Serial.print(F("Card UID:"));
    // for (byte i = 0; i < mfrc522.uid.size; i++) {
    //   Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
    //   Serial.print(mfrc522.uid.uidByte[i], HEX);
    // }
    // Serial.println();

    mfrc522.PICC_DumpToSerial(&(mfrc522.uid));          // Dump UID to serial
    mfrc522.PCD_WriteRegister(mfrc522.ComIEnReg, 0x7f);
    mfrc522.PICC_HaltA();
    flag = false;
  } else {
    Serial.printf("Counter: %d\n", count);
    count += 1;
    delay(1000);
  }

  mfrc522.PCD_WriteRegister(mfrc522.FIFODataReg, mfrc522.PICC_CMD_REQA);
  mfrc522.PCD_WriteRegister(mfrc522.CommandReg, mfrc522.PCD_Transceive);
  mfrc522.PCD_WriteRegister(mfrc522.BitFramingReg, 0x87);
}