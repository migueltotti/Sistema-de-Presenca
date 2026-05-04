#ifndef NVS_STORAGE_H
#define NVS_STORAGE_H

#include <Preferences.h>

String getClassIdFromNVS();
void saveClassIdToNVS(String classId);
void removeClassIdToNVS();

#endif