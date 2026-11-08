#ifndef MQTT_H
#define MQTT_H

#include <Arduino.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include "types.h"
#include "wifi_connection.h"

extern QueueHandle_t eventQueue;   // definida no main.cpp principal
extern WiFiClient espClient;       // definida no wifi_connection.h

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
static PubSubClient client(espClient);

// MQTT Broker
const char *mqtt_broker = "192.168.15.3";
const char *topic = "esp-room-b102";
const char *mqtt_username = "esp32";
const char *mqtt_password = "1234";
const int mqtt_port = 1883;

String client_id = "esp32-client-";

static bool parseEventType(const char* value, EventType& type) {
  if (strcmp(value, "EVT_INICIAR_REGISTRO_AULA") == 0) {
    type = EVT_INICIAR_REGISTRO_AULA;
  }
  else if (strcmp(value, "EVT_FINALIZAR_REGISTRO_AULA") == 0) {
    type = EVT_FINALIZAR_REGISTRO_AULA;
  }
  else {
    return false;
  }

  return true;
}

void callback(char *topic, byte *payload, unsigned int length) {
  JsonDocument doc;

  DeserializationError error = deserializeJson(doc, payload, length);

  if (error) {
    Serial.print("Erro ao parsear JSON: ");
    Serial.println(error.c_str());
    return;
  }

  const char* sessionEvent = doc["sessionEvent"];

  if (sessionEvent == nullptr) {
    Serial.println("Evento não informado no payload.");
    return;
  }

  AppEvent ev{};

  if (!parseEventType(sessionEvent, ev.type)) {
    Serial.printf("Evento desconhecido: %s\n", sessionEvent);
    return;
  }

  if (ev.type == EVT_INICIAR_REGISTRO_AULA || ev.type == EVT_FINALIZAR_REGISTRO_AULA) {
    const char* aulaId = doc["aulaId"];
    const char* professorTagId = doc["professorId"];

    if (aulaId == nullptr) {
      Serial.println("aulaId não informado.");
      return;
    }

    if (professorTagId == nullptr) {
      Serial.println("professorId não informado.");
      return;
    }

    strlcpy(ev.aula.aulaId, aulaId, sizeof(ev.aula.aulaId));
    strlcpy(ev.aula.professorTagId, professorTagId, sizeof(ev.aula.professorTagId));
  }

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[mqtt] eventQueue cheia, evento descartado.");
  }
}

static void connectToBroker() {
  Serial.printf("Trying to connect client %s to the public MQTT broker\n", client_id.c_str());

  if (client.connect(client_id.c_str(), mqtt_username, mqtt_password)) {
    Serial.println("Local broker connected");
  } else {
    Serial.print("failed with state ");
    Serial.println(client.state());
  }
}

void mqttTask(void*) {
  uint32_t ultimaTentativa = 0;

  for (;;) {
    // 1) Wi-Fi
    if (WiFi.status() != WL_CONNECTED) {
      Serial.println("[mqtt] WiFi desconectado, tentando reconectar...");
      connectToWiFi();
      vTaskDelay(pdMS_TO_TICKS(500));
      continue;
    }

    // 2) Reconexão MQTT com backoff simples (sem bloquear em while)
    if (!client.connected()) {
      Serial.println("[mqtt] Broker desconectado, tentando reconectar...");
      if (millis() - ultimaTentativa > 5000) {
        ultimaTentativa = millis();
        connectToBroker();
      }
      vTaskDelay(pdMS_TO_TICKS(100));
      continue;
    }

    // 3) Mantém a conexão e processa recebidos
    client.loop();

    vTaskDelay(pdMS_TO_TICKS(10));  // cede CPU às outras tasks
  }
}

static void mqttInit() {
  client.setServer(mqtt_broker, mqtt_port);
  client.setCallback(callback);

  // connect to the broker
  client_id += String(WiFi.macAddress());
  while (!client.connected()) {
    connectToBroker();
    delay(2000);
  }

  // Publish and subscribe
  client.publish(topic, "Hi, I'm ESP32 ^^");
  client.subscribe(topic);
}

#endif