#ifndef HTTP_CLIENT_H
#define HTTP_CLIENT_H

#include <HTTPClient.h>
#include <ArduinoJson.h>

struct Subject {
  String id;
  String name;
};

extern Subject subjects[];  // expõe o array para outros arquivos
extern int totalSubjects;

void httpInit(const char* ssid, const char* password);
bool fetchSubjects(const char* baseUrl, const String& professorUuid);
bool startClassWithProfessorAndSubject(const char* baseUrl, const String& professorUuid, const String& subjectUuid);

#endif