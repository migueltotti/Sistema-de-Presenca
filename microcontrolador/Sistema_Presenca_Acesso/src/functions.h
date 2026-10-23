#ifndef FUNCTIONS_H
#define FUNCTIONS_H

#include <Arduino.h>
#include <MFRC522.h>

static void uidParaHex(const MFRC522::Uid& uid, char* out) {
  for (byte i = 0; i < uid.size; i++) sprintf(out + i * 2, "%02X", uid.uidByte[i]);
  out[uid.size * 2] = '\0';
}

#endif