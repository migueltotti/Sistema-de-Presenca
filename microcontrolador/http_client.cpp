#include "http_client.h"
#include <WiFi.h>

const int MAX_SUBJECTS = 20;
Subject subjects[MAX_SUBJECTS];
int totalSubjects = 0;

const String WIFI_NOT_CONNECTED_ERROR = "WIFI_NOT_CONNECTED";
const String REQUEST_ERROR = "REQUEST_ERROR";

void httpInit(const char* ssid, const char* password){
    WiFi.begin(ssid, password);
    Serial.print("Conectando ao Wi-Fi");
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    Serial.println("\nConectado! IP: " + WiFi.localIP().toString());
}

bool isWifiConnected(){
    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("Wi-Fi desconectado");
        return false;
    }
    return true;
}

bool fetchSubjects(const char* baseUrl, const String& professorUuid){
    if (!isWifiConnected()) return false;

    char url[150];
    snprintf(url, sizeof(url), "%s/professor/%s/subjects", baseUrl, professorUuid.c_str());
    
    HTTPClient http;
    http.begin(url);

    int httpResponseCode = http.GET();

    if (httpResponseCode > 0) {
        Serial.printf("HTTP Status: %d\n", httpResponseCode);

        if (httpResponseCode == HTTP_CODE_OK) {
            String payload = http.getString();
            Serial.println("Resposta:");
            Serial.println(payload);

            JsonDocument doc;
            DeserializationError erro = deserializeJson(doc, payload);
            if (erro) {
                Serial.print("Erro ao parsear JSON: ");
                Serial.println(erro.c_str());
                http.end();
                return false;
            }

            JsonArray arr = doc["subjects"].as<JsonArray>();
            totalSubjects = 0;

            // carrega a lista de matérias do professor no array global de matérias
            for (JsonObject subject : arr) {
                if (totalSubjects >= MAX_SUBJECTS) break;  // evita overflow

                subjects[totalSubjects].id   = subject["id"].as<String>();
                subjects[totalSubjects].name = subject["Name"].as<String>();
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

StartClassResponse startClassWithProfessorAndSubject(const char* baseUrl, const String& professorUuid, const String& subjectUuid){
    if (!isWifiConnected()) return StartClassResponse{false, "", REQUEST_ERROR};

    char url[150];
    snprintf(url, sizeof(url), "%s/class", baseUrl);

    HTTPClient http;
    http.begin(url);
    http.addHeader("Content-Type", "application/json");

    JsonDocument doc;
    doc["professorId"] = professorUuid;
    doc["subjectId"]  = subjectUuid;

    String body;
    serializeJson(doc, body);  // converte para string JSON
    
    int httpResponseCode = http.POST(body);

    if (httpResponseCode > 0) {
        Serial.printf("HTTP Status: %d\n", httpResponseCode);

        String payload = http.getString();
        Serial.println("Resposta:");
        Serial.println(payload);

        JsonDocument doc;
        DeserializationError erro = deserializeJson(doc, payload);

        if (httpResponseCode == HTTP_CODE_CREATED) {
            String classId = doc["classId"].as<String>();

            http.end();
            return StartClassResponse{true, classId, ""};
        }
        else {
            String errorCode = errorDoc["code"].as<String>();
            String errorMessage = errorDoc["description"].as<String>();

            Serial.println(errorMessage);

            http.end();
            return StartClassResponse{false, "", errorCode};
        }
    } else {
        Serial.println("Erro na requisição: " + String(httpResponseCode));
        http.end();
        return StartClassResponse{false, "", REQUEST_ERROR};
    }
}

ContinueClassResponse continueClassByProfessor(const char* baseUrl, const String& classUuid, const String& professorUuid){    
    if (!isWifiConnected()) return ContinueClassResponse{false, WIFI_NOT_CONNECTED_ERROR};

    char url[150];
    snprintf(url, sizeof(url), "%s/class/%s/continue", baseUrl, classUuid.c_str());

    HTTPClient http;
    http.begin(url);
    http.addHeader("Content-Type", "application/json");

    JsonDocument doc;
    doc["professorId"] = professorUuid;

    String body;
    serializeJson(doc, body);  // converte para string JSON
    
    int httpResponseCode = http.POST(body);

    if (httpResponseCode > 0) {
        Serial.printf("HTTP Status: %d\n", httpResponseCode);

        if (httpResponseCode == HTTP_CODE_OK) {
            String payload = http.getString();
            Serial.println("Resposta:");
            Serial.println(payload);

            http.end();
            return ContinueClassResponse{true, ""};
        }
        else{
            JsonDocument errorDoc;
            DeserializationError erro = deserializeJson(doc, payload);

            String errorCode = errorDoc["code"].as<String>();
            String errorMessage = errorDoc["description"].as<String>();

            Serial.println(errorMessage);

            http.end();
            return ContinueClassResponse{false, errorCode};
        }
    } else {
        Serial.println("Erro na requisição: " + String(httpResponseCode));
        http.end();
        return ContinueClassResponse{false, REQUEST_ERROR};
    }
}