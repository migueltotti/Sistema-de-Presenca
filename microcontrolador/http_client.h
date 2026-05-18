#ifndef HTTP_CLIENT_H
#define HTTP_CLIENT_H

#include <HTTPClient.h>
#include <ArduinoJson.h>

struct Subject {
  String id;
  String name;
};

struct ContinueSessionResponse {
  bool isSuccess;
  String errorCode;
};

struct StartSessionResponse {
  bool isSuccess;
  String sessionId;
  String errorCode;
}

struct EndSessionResponse {
  bool isSuccess;
  String errorCode;
}

extern const String WIFI_NOT_CONNECTED_ERROR = "WIFI_NOT_CONNECTED";
extern const String REQUEST_ERROR = "REQUEST_ERROR";

extern Subject subjects[];  // expõe o array para outros arquivos
extern int totalSubjects;

void httpInit(const char* ssid, const char* password);
bool fetchSubjects(const char* baseUrl, const String& professorUuid);
StartSessionResponse startSessionWithProfessorAndSubject(const char* baseUrl, const String& professorUuid, const String& subjectUuid, const int& numberOfClasses);
ContinueSessionResponse continueSessionByProfessor(const char* baseUrl, const String& sessionId, const String& professorUuid);
EndSessionResponse endSessionByProfessor(const char* baseUrl, const String& sessionId, const String& professorUuid);

#endif