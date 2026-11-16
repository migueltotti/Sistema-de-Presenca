#ifndef HTTP_H
#define HTTP_H

#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>
#include "types.h"

extern QueueHandle_t httpQueue;
extern QueueHandle_t eventQueue;

const char* API_BASE_URL       = "https://dizygotic-ethelene-elvishly.ngrok-free.dev";
const char* API_KEY            = "API_DA_SILVA_JR";
const char* MICROCONTROLLER_ID = "esp-room-b102";

static void enviarResultadoHttp(const HttpResultado& res) {
  AppEvent ev{};
  ev.type = EVT_HTTP_RESULTADO;
  ev.http = res;

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[http] eventQueue cheia, resultado HTTP descartado.");
  }
}

static void montarUrlEBody(const HttpRequest& req, char* urlOut, size_t urlSize, String& bodyOut) {
  JsonDocument doc;

  switch (req.action) {
    case HTTP_VERIFICAR_ACESSO:
    case HTTP_REGISTRAR_ABERTURA:
      snprintf(urlOut, urlSize, "%s/microcontroller/%s/open/tag-id/%s",
               API_BASE_URL, MICROCONTROLLER_ID, req.uid);
      if (req.sessionId[0] != '\0') {
        doc["sessionId"] = req.sessionId;
      } else {
        doc["sessionId"] = nullptr;
      }
      break;

    case HTTP_REGISTRAR_FECHAMENTO:
      snprintf(urlOut, urlSize, "%s/microcontroller/%s/close/tag-id/%s",
               API_BASE_URL, MICROCONTROLLER_ID, req.uid);
      if (req.sessionId[0] != '\0') {
        doc["sessionId"] = req.sessionId;
      } else {
        doc["sessionId"] = nullptr;
      }
      break;

    case HTTP_REGISTRAR_PRESENCA:
      snprintf(urlOut, urlSize, "%s/api/v1/sessions/%s/attendance",
               API_BASE_URL, req.sessionId);
      doc["studentTagId"] = req.uid;
      break;
  }

  serializeJson(doc, bodyOut);
}

static void executarRequisicaoHttp(const HttpRequest& req) {
  HttpResultado res{};
  res.requestId   = req.requestId;
  res.action      = req.action;
  res.falhaDeRede = false;
  res.httpStatus  = 0;

  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("[http] Sem conexao Wi-Fi.");
    res.falhaDeRede = true;
    strlcpy(res.motivo, "Sem Wi-Fi", sizeof(res.motivo));
    strlcpy(res.mensagem, "Sem Wi-Fi", sizeof(res.mensagem));
    enviarResultadoHttp(res);
    return;
  }

  char url[256];
  String body;
  montarUrlEBody(req, url, sizeof(url), body);

  Serial.printf("[http] POST %s | body: %s\n", url, body.c_str());

  HTTPClient http;
  http.setTimeout(TIMEOUT_HTTP_MS);

  WiFiClientSecure secureClient;
  WiFiClient plainClient;

  bool iniciou = false;
  if (strncmp(API_BASE_URL, "https://", 8) == 0) {
    secureClient.setInsecure();
    iniciou = http.begin(secureClient, url);
  } else {
    iniciou = http.begin(plainClient, url);
  }

  if (!iniciou) {
    Serial.println("[http] Falha ao iniciar cliente HTTP.");
    res.falhaDeRede = true;
    strlcpy(res.motivo, "Erro conexao", sizeof(res.motivo));
    strlcpy(res.mensagem, "Erro conexao", sizeof(res.mensagem));
    enviarResultadoHttp(res);
    return;
  }

  http.addHeader("Content-Type", "application/json");
  http.addHeader("X-Api-Key", API_KEY);
  http.addHeader("ngrok-skip-browser-warning", "true");

  int statusCode = http.POST(body);

  if (statusCode <= 0) {
    Serial.printf("[http] Falha de rede/timeout: %s\n", http.errorToString(statusCode).c_str());
    res.falhaDeRede = true;
    strlcpy(res.motivo, "Sem resposta", sizeof(res.motivo));
    strlcpy(res.mensagem, "Sem resposta", sizeof(res.mensagem));
    http.end();
    enviarResultadoHttp(res);
    return;
  }

  res.httpStatus = (int16_t)statusCode;
  String payload = http.getString();
  http.end();

  Serial.printf("[http] Status %d | Resp: %s\n", statusCode, payload.c_str());

  if (statusCode >= 200 && statusCode < 300) {
    strlcpy(res.motivo, "OK", sizeof(res.motivo));
    strlcpy(res.mensagem, "OK", sizeof(res.mensagem));
  } else {
    JsonDocument errDoc;
    if (!deserializeJson(errDoc, payload)) {
      const char* desc = errDoc["description"] | errDoc["message"] | "Acesso negado";
      strlcpy(res.motivo, desc, sizeof(res.motivo));
      strlcpy(res.mensagem, desc, sizeof(res.mensagem));
    } else {
      strlcpy(res.motivo, "Erro servidor", sizeof(res.motivo));
      strlcpy(res.mensagem, "Erro servidor", sizeof(res.mensagem));
    }
  }

  enviarResultadoHttp(res);
}

void httpTask(void*) {
  HttpRequest req{};

  for (;;) {
    if (xQueueReceive(httpQueue, &req, portMAX_DELAY) == pdTRUE) {
      executarRequisicaoHttp(req);
    }
  }
}

#endif