#include "nvs_storage.h"

/* Instancia para lidar com operações de persistência de dados (NVS) */
Preferences preferences;

String getClassIdFromNVS() {
  preferences.begin("class", true);
  String classId = preferences.getString("id", "");
  preferences.end();
  return classId;
}

void saveClassIdToNVS(String classId) {
  preferences.begin("class", false);
  preferences.putString("id", classId);
  preferences.end();
}

void removeClassIdToNVS() {
  preferences.begin("class", false);
  preferences.remove("id");
  preferences.end();
}