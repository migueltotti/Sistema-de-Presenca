#ifndef NVS_STORAGE_H
#define NVS_STORAGE_H

#include <Preferences.h>

String getSessionIdFromNVS();
void saveSessionIdToNVS(String sessionId);
void removeSessionIdToNVS();

#endif