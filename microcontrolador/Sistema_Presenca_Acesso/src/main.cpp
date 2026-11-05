#include <Arduino.h>
#include <Wire.h>
#include <SPI.h>
#include <MFRC522.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include "types.h" 
#include "functions.h"
#include "display.h"

#define SS_INTERNO_PIN  32
#define SCK_PIN         25
#define MISO_PIN        27
#define MOSI_PIN        26
#define RST_PIN         33

MFRC522 rfidInterno(SS_INTERNO_PIN, RST_PIN);   // Create MFRC522 instance.

QueueHandle_t eventQueue, displayQueue;

WiFiClient espClient;
PubSubClient client(espClient);

// WiFi
const char *ssid = "iPhone de Miguel "; // Enter your WiFi name
const char *password = "987654321";  // Enter WiFi password

// MQTT Broker
const char *mqtt_broker = "172.20.10.8";
const char *topic = "esp-room-b102";
const char *mqtt_username = "esp32";
const char *mqtt_password = "1234";
const int mqtt_port = 1883;

String client_id = "esp32-client-";

// ============= FUNÇÕES DE LEITURA DE TAGS =============

// Compartilhada com a futura versão por IRQ: lê o UID e publica o evento.
static bool publicarTag(MFRC522& leitor, Lado lado) {
  if (!leitor.PICC_ReadCardSerial()) return false;

  AppEvent ev{};
  ev.type = EVT_TAG;
  ev.tag.lado = lado;
  uidParaHex(leitor.uid, ev.tag.uid);

  leitor.PICC_HaltA();          // tag para de responder até sair do campo
  leitor.PCD_StopCrypto1();     // TALVEZ REMOVER

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[reader] eventQueue cheia, evento descartado.");
  }
  else{
    Serial.println("[reader] evento publicado.");
  }
  return true;
}

void readerTask(void*) {
  MFRC522* leitores[1]  = { &rfidInterno };
  uint32_t bloqueadoAte[1] = { 0 };

  for (;;) {
    for (uint8_t i = 0; i < 1; i++) {
      if ((int32_t)(bloqueadoAte[i] - millis()) > 0) continue;

      if (leitores[i]->PICC_IsNewCardPresent() && publicarTag(*leitores[i], (Lado)i)) {
        bloqueadoAte[i] = millis() + 1500;   // ignora releituras por 1,5 s
      }
    }
    vTaskDelay(pdMS_TO_TICKS(50));           // cede a CPU; sem isso a task ocuparia o núcleo
  }
}

// =================================================

// ================ FUNÇÕES DE WIFI ================

void connectToWiFi() {
  Serial.print("Connecting to WiFi..");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.printf("\nConnected to WiFi | IP: %s\n", WiFi.localIP().toString().c_str());
}

// =================================================

// ============= FUNÇÕES DE MENSAGERIA =============
bool parseEventType(const char* value, EventType& type) {
  if (strcmp(value, "EVT_AULA_INICIOU") == 0) {
    type = EVT_AULA_INICIOU;
  }
  else if (strcmp(value, "EVT_AULA_FINALIZOU") == 0) {
    type = EVT_AULA_FINALIZOU;
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

  if (ev.type == EVT_AULA_INICIOU || ev.type == EVT_AULA_FINALIZOU) {
    const char* aulaId = doc["aulaId"];
    const char* professorId = doc["professorId"];

    if (aulaId == nullptr) {
      Serial.println("aulaId não informado.");
      return;
    }

    if (professorId == nullptr) {
      Serial.println("professorId não informado.");
      return;
    }

    strlcpy(ev.aula.aulaId, aulaId, sizeof(ev.aula.aulaId));
    strlcpy(ev.aula.professorId, professorId, sizeof(ev.aula.professorId));
  }

  if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
    Serial.println("[mqtt] eventQueue cheia, evento descartado.");
  }
}

void connectToBroker() {
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

// =================================================

void controllerTask(void*) {
  mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente

  AppEvent ev;
  for (;;) {
    if (xQueueReceive(eventQueue, &ev, portMAX_DELAY) == pdTRUE) {
      switch (ev.type) {
        case EVT_TAG:
          mostrar(LADO_INTERNO, "Presenca OK",   "Maria Silva", 2000);     // volta sozinho
          Serial.printf("[ctrl] tag %s no leitor %s\n", ev.tag.uid, ev.tag.lado == LADO_INTERNO ? "interno" : "externo");
          break;

        case EVT_BTN_CONFIRMA:
          break;

        case EVT_BTN_CANCELA:
          break;

        case EVT_AULA_INICIOU:
          mostrar(LADO_INTERNO, "Aula iniciada", "Aproxime a tag");        // permanente
          Serial.printf("[ctrl] aula com id %s iniciada para o professor %s.\n", ev.aula.aulaId, ev.aula.professorId);
          break;

        case EVT_AULA_FINALIZOU:
          mostrar(LADO_INTERNO, "Aula finalizada", "Obrigado!", 3000);     // temporaria
          Serial.printf("[ctrl] aula com id %s finalizada para o professor %s.\n", ev.aula.aulaId, ev.aula.professorId);
          break;

        case EVT_HTTP_RESULTADO:
          break;

        default:
          break;
      }
    }

    Serial.println("[ctrl] aguardando evento...");
  }
}

void setup() {
  Serial.begin(115200);  
  
  eventQueue    = xQueueCreate(16, sizeof(AppEvent));
  displayQueue  = xQueueCreate(8,  sizeof(DisplayMsg));

  SPI.begin(SCK_PIN, MISO_PIN, MOSI_PIN);         // Init SPI bus
  rfidInterno.PCD_Init();                         // Init MFRC522 reader interno
  rfidInterno.PCD_DumpVersionToSerial();          // Show details of PCD - MFRC522 Card Reader details

  displayInit();

  connectToWiFi();

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

  xTaskCreatePinnedToCore(readerTask,     "reader",   4096, nullptr, 3, nullptr, 1);
  xTaskCreatePinnedToCore(displayTask,    "display",  4096, nullptr, 1, nullptr, 1);
  xTaskCreatePinnedToCore(controllerTask, "ctrl",     4096, nullptr, 2, nullptr, 1);
  xTaskCreatePinnedToCore(mqttTask,       "mqtt",     4096, nullptr, 2, nullptr, 0);
}

void loop() { vTaskDelete(NULL); }