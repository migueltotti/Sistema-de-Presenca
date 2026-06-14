#ifndef NVS_STORAGE_H
#define NVS_STORAGE_H

#include <Preferences.h>

/* Instancia para lidar com operações de persistência de dados (NVS) */
Preferences preferences;

String getSessionIdFromNVS() {
  preferences.begin("session", true);
  String sessionId = preferences.getString("id", "");
  preferences.end();
  return sessionId;
}

void saveSessionIdToNVS(String sessionId) {
  preferences.begin("session", false);
  preferences.putString("id", sessionId);
  preferences.end();
}

void removeSessionIdToNVS() {
  preferences.begin("session", false);
  preferences.remove("id");
  preferences.end();
}

#endif