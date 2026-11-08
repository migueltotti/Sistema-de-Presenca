#ifndef WIFI_CONNECTION_H
#define WIFI_CONNECTION_H

#include <Arduino.h>
#include <WiFi.h>

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
WiFiClient espClient;

const char *ssid = "MIGUEL"; // Enter your WiFi name
const char *password = "miguel2005";  // Enter WiFi password

static void connectToWiFi() {
  Serial.print("Connecting to WiFi..");
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.printf("\nConnected to WiFi | IP: %s\n", WiFi.localIP().toString().c_str());
}

#endif