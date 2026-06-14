#ifndef HTTP_CLIENT_H
#define HTTP_CLIENT_H

#include <HTTPClient.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>
#include <WiFi.h>

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
};

struct EndSessionResponse {
  bool isSuccess;
  String errorCode;
};

struct RegisterAttendanceResponse {
  bool isSuccess;
  String errorCode;
};

extern const String WIFI_NOT_CONNECTED_ERROR = "WIFI_NOT_CONNECTED";
extern const String REQUEST_ERROR = "REQUEST_ERROR";

const int MAX_SUBJECTS = 20;
extern Subject subjects[];
extern int totalSubjects;

Subject subjects[MAX_SUBJECTS];
int totalSubjects = 0;

void httpInit(const char* ssid, const char* password) {
  WiFi.begin(ssid, password);
  Serial.print("Conectando ao Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
      delay(500);
      Serial.print(".");
  }
  Serial.println("\nConectado! IP: " + WiFi.localIP().toString());
}

bool isWifiConnected() {
  if (WiFi.status() != WL_CONNECTED) {
      Serial.println("Wi-Fi desconectado");
      return false;
  }
  return true;
}

bool fetchSubjects(const char* baseUrl, const String& professorUuid) {
  if (!isWifiConnected()) return false;

  String encodedProfessorId = professorUuid;
  encodedProfessorId.replace(":", "%3A");

  char url[250];
  snprintf(url, sizeof(url), "%s/subjects?professorTagId=%s", baseUrl, encodedProfessorId.c_str());

  WiFiClientSecure client;
  client.setInsecure();

  HTTPClient http;
  http.begin(client, url);
  http.addHeader("ngrok-skip-browser-warning", "true");
  http.addHeader("Content-Type", "application/json");

  int httpResponseCode = http.GET();

  if (httpResponseCode > 0) {
    Serial.printf("HTTP Status: %d\n", httpResponseCode);

    if (httpResponseCode == HTTP_CODE_OK) {
      String payload = http.getString();
      //Serial.println("Resposta:");
      //Serial.println(payload);

      JsonDocument doc;
      DeserializationError erro = deserializeJson(doc, payload);
      if (erro) {
        Serial.print("Erro ao parsear JSON: ");
        Serial.println(erro.c_str());
        http.end();
        return false;
      }

      JsonArray arr = doc.as<JsonArray>();
      totalSubjects = 0;

      // carrega a lista de matérias do professor no array global de matérias
      for (JsonObject subject : arr) {
        if (totalSubjects >= MAX_SUBJECTS) break;  // evita overflow

        subjects[totalSubjects].id   = subject["id"].as<String>();
        subjects[totalSubjects].name = subject["name"].as<String>();
        totalSubjects++;
      }

      Serial.printf("%d subjects carregados.\n", totalSubjects);
      http.end();
      return true;
    }
    else if (httpResponseCode == HTTP_CODE_NOT_FOUND){
      Serial.println("Professor não encontrado!");
      http.end();
      return false;
    }
    else {
      Serial.println("Erro na requisição: " + String(httpResponseCode));
      http.end();
      return false;
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
    http.end();
    return false;
  }
}

StartSessionResponse startSessionWithProfessorAndSubject(const char* baseUrl, const String& professorUuid, const String& subjectUuid, const int& numberOfClasses) {
  if (!isWifiConnected()) return StartSessionResponse{false, "", REQUEST_ERROR};

  Serial.println("startSession: professor=" + professorUuid + " subject=" + subjectUuid + " numberOfClasses=" + numberOfClasses);

  char url[250];
  snprintf(url, sizeof(url), "%s/sessions", baseUrl);

  WiFiClientSecure client;
  client.setInsecure();

  HTTPClient http;
  http.begin(client, url);
  http.addHeader("ngrok-skip-browser-warning", "true");
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["professorTagId"] = professorUuid;
  doc["subjectId"]  = subjectUuid;
  doc["numberOfClasses"] = numberOfClasses;

  String body;
  serializeJson(doc, body);  // converte para string JSON

  int httpResponseCode = http.POST(body);

  Serial.printf("startSession — HTTP code: %d", httpResponseCode);

  if (httpResponseCode > 0) {
    //Serial.printf("HTTP Status: %d\n", httpResponseCode);

    String payload = http.getString();
    //Serial.println("Resposta:");
    //Serial.println(payload);

    JsonDocument respDoc;
    DeserializationError errorDoc = deserializeJson(respDoc, payload);

    if (httpResponseCode == HTTP_CODE_OK) {
      String sessionId = respDoc["sessionId"].as<String>();

      http.end();
      return StartSessionResponse{true, sessionId, ""};
    }
    else {
      String errorCode = respDoc["code"].as<String>();
      String errorMessage = respDoc["description"].as<String>();

      Serial.println(errorMessage);

      http.end();
      return StartSessionResponse{false, "", errorCode};
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
    http.end();
    return StartSessionResponse{false, "", REQUEST_ERROR};
  }
}

ContinueSessionResponse continueSessionByProfessor(const char* baseUrl, const String& sessionId, const String& professorUuid) {
  if (!isWifiConnected()) return ContinueSessionResponse{false, WIFI_NOT_CONNECTED_ERROR};

  char url[150];
  snprintf(url, sizeof(url), "%s/sessions/%s/continue", baseUrl, sessionId.c_str());

  WiFiClientSecure client;
  client.setInsecure();

  HTTPClient http;
  http.begin(client, url);
  http.addHeader("ngrok-skip-browser-warning", "true");
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["professorTagId"] = professorUuid;

  String body;
  serializeJson(doc, body);  // converte para string JSON

  int httpResponseCode = http.POST(body);

  Serial.printf("continueSession — HTTP code: %d", httpResponseCode);

  if (httpResponseCode > 0) {
    String payload = http.getString();
    // Serial.println("Resposta:");
    // Serial.println(payload);

    if (httpResponseCode == HTTP_CODE_OK) {
      http.end();
      return ContinueSessionResponse{true, ""};
    }
    else {
      JsonDocument errorDoc;
      DeserializationError erro = deserializeJson(errorDoc, payload);

      String errorCode = errorDoc["code"].as<String>();
      String errorMessage = errorDoc["description"].as<String>();

      Serial.println(errorMessage);

      http.end();
      return ContinueSessionResponse{false, errorCode};
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
    http.end();
    return ContinueSessionResponse{false, REQUEST_ERROR};
  }
}

EndSessionResponse endSessionByProfessor(const char* baseUrl, const String& sessionId, const String& professorUuid) {
  if (!isWifiConnected()) return EndSessionResponse{false, WIFI_NOT_CONNECTED_ERROR};

  char url[150];
  snprintf(url, sizeof(url), "%s/sessions/%s/end", baseUrl, sessionId.c_str());

  WiFiClientSecure client;
  client.setInsecure();

  HTTPClient http;
  http.begin(client, url);
  http.addHeader("ngrok-skip-browser-warning", "true");
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["professorTagId"] = professorUuid;

  String body;
  serializeJson(doc, body);  // converte para string JSON

  int httpResponseCode = http.POST(body);

  Serial.printf("endSession — HTTP code: %d", httpResponseCode);

  if (httpResponseCode > 0) {
    String payload = http.getString();
    // Serial.println("Resposta:");
    // Serial.println(payload);

    if (httpResponseCode == HTTP_CODE_OK) {
      http.end();
      return EndSessionResponse{true, ""};
    }
    else {
      JsonDocument errorDoc;
      DeserializationError erro = deserializeJson(errorDoc, payload);

      String errorCode = errorDoc["code"].as<String>();
      String errorMessage = errorDoc["description"].as<String>();

      Serial.println(errorMessage);

      http.end();
      return EndSessionResponse{false, errorCode};
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
    http.end();
    return EndSessionResponse{false, REQUEST_ERROR};
  }
}

RegisterAttendanceResponse registerAttendanceByStudent(const char* baseUrl, const String& sessionId, const String& studentUuid) {
  if (!isWifiConnected()) return RegisterAttendanceResponse{false, WIFI_NOT_CONNECTED_ERROR};

  char url[150];
  snprintf(url, sizeof(url), "%s/sessions/%s/attendance", baseUrl, sessionId.c_str());

  WiFiClientSecure client;
  client.setInsecure();

  HTTPClient http;
  http.begin(client, url);
  http.addHeader("ngrok-skip-browser-warning", "true");
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["studentTagId"] = studentUuid;

  String body;
  serializeJson(doc, body);  // converte para string JSON
  
  int httpResponseCode = http.POST(body);

  Serial.printf("registerAttendance — HTTP code: %d", httpResponseCode);

  if (httpResponseCode > 0) {
    String payload = http.getString();
    // Serial.println("Resposta:");
    // Serial.println(payload);

    if (httpResponseCode == HTTP_CODE_OK) {
      http.end();
      return RegisterAttendanceResponse{true, ""};
    }
    else{
      JsonDocument errorDoc;
      DeserializationError erro = deserializeJson(errorDoc, payload);

      String errorCode = errorDoc["code"].as<String>();
      String errorMessage = errorDoc["description"].as<String>();

      Serial.println(errorMessage);

      http.end();
      return RegisterAttendanceResponse{false, errorCode};
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
    http.end();
    return RegisterAttendanceResponse{false, REQUEST_ERROR};
  }
}

#endif