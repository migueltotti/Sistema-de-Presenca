#ifndef BUTTON_H
#define BUTTON_H

#include <Arduino.h>
#include "types.h"

extern QueueHandle_t eventQueue; 

constexpr uint8_t PIN_BTN_CONFIRMA = 18;
constexpr uint8_t PIN_BTN_CANCELA  = 19;

constexpr uint32_t DEBOUNCE_MS      = 50;
constexpr uint32_t POLL_INTERVAL_MS = 10;

struct BotaoState {
  uint8_t   pin;
  EventType evento;
  bool      estadoEstavel;
  bool      ultimaLeitura;
  uint32_t  ultimoTempoMudanca;
};

static BotaoState botoes[2] = {
  { PIN_BTN_CONFIRMA, EVT_BTN_CONFIRMA, HIGH, HIGH, 0 },
  { PIN_BTN_CANCELA,  EVT_BTN_CANCELA,  HIGH, HIGH, 0 }
};

static void buttonsInit() {
  pinMode(PIN_BTN_CONFIRMA, INPUT_PULLUP);
  pinMode(PIN_BTN_CANCELA,  INPUT_PULLUP);
}

void buttonTask(void*) {
  for (;;) {
    const uint32_t agora = millis();

    for (uint8_t i = 0; i < 2; i++) {
      bool leituraAtual = digitalRead(botoes[i].pin);

      if (leituraAtual != botoes[i].ultimaLeitura) {
        botoes[i].ultimoTempoMudanca = agora;
        botoes[i].ultimaLeitura = leituraAtual;
      }

      if ((agora - botoes[i].ultimoTempoMudanca) >= DEBOUNCE_MS) {
        if (leituraAtual != botoes[i].estadoEstavel) {
          botoes[i].estadoEstavel = leituraAtual;

          // Dispara evento apenas quando o botão é pressionado (HIGH -> LOW)
          if (botoes[i].estadoEstavel == LOW) {
            AppEvent ev{};
            ev.type = botoes[i].evento;

            if (xQueueSend(eventQueue, &ev, 0) != pdTRUE) {
              Serial.println("[button] eventQueue cheia, clique descartado.");
            }
          }
        }
      }
    }

    vTaskDelay(pdMS_TO_TICKS(POLL_INTERVAL_MS));
  }
}

#endif