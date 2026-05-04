#include "nvs_storage.h"

/* Instancia para lidar com operações de persistência de dados (NVS) */
Preferences preferences;

void saveClassIdToNVS(String classId) {
  preferences.begin("class", false);
  preferences.putString("id", classId);
  preferences.end();
}

String getClassIdFromNVS() {
  preferences.begin("class", true);
  String classId = preferences.getString("id", "");
  preferences.end();
  return classId;
}