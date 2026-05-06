#ifndef HTTP_CLIENT_H
#define HTTP_CLIENT_H

#include <HTTPClient.h>
#include <ArduinoJson.h>

struct Subject {
  String id;
  String name;
};

struct ContinueClassResponse {
  bool isSuccess;
  String errorCode;
};

struct StartClassResponse {
  bool isSuccess;
  String classId;
  String errorCode;
}

extern const String WIFI_NOT_CONNECTED_ERROR = "WIFI_NOT_CONNECTED";
extern const String REQUEST_ERROR = "REQUEST_ERROR";

extern Subject subjects[];  // expõe o array para outros arquivos
extern int totalSubjects;

void httpInit(const char* ssid, const char* password);
bool fetchSubjects(const char* baseUrl, const String& professorUuid);
StartClassResponse startClassWithProfessorAndSubject(const char* baseUrl, const String& professorUuid, const String& subjectUuid);
ContinueClassResponse continueClassByProfessor(const char* baseUrl, const String& classId, const String& professorUuid);

#endif