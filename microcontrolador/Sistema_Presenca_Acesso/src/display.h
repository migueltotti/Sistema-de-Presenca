#pragma once

#include <Arduino.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include "types.h"

extern QueueHandle_t displayQueue;   // definida no .ino principal

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
constexpr uint8_t LCD_ADDR_INTERNO = 0x26;
constexpr uint8_t LCD_ADDR_EXTERNO = 0x27;
constexpr uint8_t LCD_COLS = 16;
constexpr uint8_t LCD_ROWS = 2;

constexpr uint8_t PIN_SDA = 16; // RX2
constexpr uint8_t PIN_SCL = 17; // TX2

static LiquidCrystal_I2C lcdInterno(LCD_ADDR_INTERNO, LCD_COLS, LCD_ROWS);
static LiquidCrystal_I2C lcdExterno(LCD_ADDR_EXTERNO, LCD_COLS, LCD_ROWS);

static LiquidCrystal_I2C* const lcds[2] = { &lcdInterno, &lcdExterno };

struct EstadoLado {
  DisplayMsg permanente;        // estado de repouso (ex.: "Aproxime a tag")
  bool       temPermanente;     // já recebeu alguma permanente?
  DisplayMsg temporaria;        // mensagem de duração limitada
  bool       temporariaAtiva;
  uint32_t   expiraEmMs;        // instante absoluto (millis) em que a temporária vence
};

static EstadoLado estado[2] = {};

// ----------------------------------------------------------------------------
// Inicialização do hardware (chamar no setup(), antes de criar as tasks)
// ----------------------------------------------------------------------------
static void displayInit() {
  Wire.begin(PIN_SDA, PIN_SCL);

  for (uint8_t i = 0; i < 2; i++) {
    lcds[i]->init();
    lcds[i]->backlight();
    lcds[i]->clear();
  }
}

// ----------------------------------------------------------------------------
// Desenho
// ----------------------------------------------------------------------------

static void escreverLinha(LiquidCrystal_I2C& lcd, uint8_t linha, const char* texto) {
  char buf[LCD_COLS + 1];
  snprintf(buf, sizeof(buf), "%-*.*s", LCD_COLS, LCD_COLS, texto);
  lcd.setCursor(0, linha);
  lcd.print(buf);
}

static void desenhar(const DisplayMsg& m) {
  LiquidCrystal_I2C& lcd = *lcds[m.lado];
  escreverLinha(lcd, 0, m.linha1);
  escreverLinha(lcd, 1, m.linha2);
}

static void limparLado(Lado lado) {
  lcds[lado]->clear();
}

// ----------------------------------------------------------------------------
// Funções usadas pela displayTask
// ----------------------------------------------------------------------------

// true se o instante 'expiraEm' já passou. A subtração com sinal funciona
// corretamente mesmo quando o millis() estoura (a cada ~49 dias).
// static inline bool venceu(uint32_t expiraEm, uint32_t agora) {
//   return (int32_t)(expiraEm - agora) <= 0;
// }

// Tempo (em ticks) até a temporária mais próxima de vencer.
// portMAX_DELAY se nenhum lado tem temporária ativa (a task dorme até chegar mensagem).
static TickType_t tempoAteProximaExpiracao() {
  const uint32_t agora = millis();
  bool     achou = false;
  uint32_t menor = 0;

  for (uint8_t i = 0; i < 2; i++) {
    if (!estado[i].temporariaAtiva) continue;

    int32_t resta = (int32_t)(estado[i].expiraEmMs - agora);
    if (resta < 0) resta = 0;                    // já venceu: acordar já

    if (!achou || (uint32_t)resta < menor) {
      menor = (uint32_t)resta;
      achou = true;
    }
  }
  return achou ? pdMS_TO_TICKS(menor) : portMAX_DELAY;
}

// static void guardarPermanente(const DisplayMsg& m) {
//   estado[m.lado].permanente    = m;
//   estado[m.lado].temPermanente = true;
// }

// static bool temTemporariaAtiva(Lado lado) {
//   return estado[lado].temporariaAtiva;
// }

// Se já havia uma temporária ativa neste lado, ela é substituída
// e o prazo recomeça a contar a partir de agora.
// static void iniciarTemporaria(const DisplayMsg& m) {
//   estado[m.lado].temporaria      = m;
//   estado[m.lado].temporariaAtiva = true;
//   estado[m.lado].expiraEmMs      = millis() + m.duracaoMs;
// }

// Para cada lado com temporária vencida: desativa e volta à permanente.
static void expirarTemporarias() {
  const uint32_t agora = millis();

  for (uint8_t i = 0; i < 2; i++) {
    if (!estado[i].temporariaAtiva) continue;
    if (!((int32_t)(estado[i].expiraEmMs - agora) <= 0)) continue;

    estado[i].temporariaAtiva = false;

    if (estado[i].temPermanente){
        desenhar(estado[i].permanente);
    } else {
        limparLado((Lado)i);
    }
  }
}

// ----------------------------------------------------------------------------
// A task
// ----------------------------------------------------------------------------
void displayTask(void*) {
  for (;;) {
    // espera até chegar mensagem OU até a temporária mais próxima expirar
    TickType_t espera = tempoAteProximaExpiracao();

    DisplayMsg m;
    if (xQueueReceive(displayQueue, &m, espera) == pdTRUE) {
      if (m.duracaoMs == 0) {
        estado[m.lado].permanente = m;
        estado[m.lado].temPermanente = true;

        if (!estado[m.lado].temporariaAtiva) {
          desenhar(m);
        }
      } else {
        estado[m.lado].temporaria = m;
        estado[m.lado].temporariaAtiva = true;
        estado[m.lado].expiraEmMs = millis() + m.duracaoMs;
        desenhar(m);
      }
    }
    expirarTemporarias();
  }
}

static void mostrar(Lado lado, const char* l1, const char* l2, uint16_t duracaoMs = 0) {
  DisplayMsg m{};
  m.lado = lado;
  strlcpy(m.linha1, l1, MSG_LEN);
  strlcpy(m.linha2, l2, MSG_LEN);
  m.duracaoMs = duracaoMs;
  if (xQueueSend(displayQueue, &m, 0) != pdTRUE) {
    Serial.println("[display] displayQueue cheia, mensagem descartada");
  }
}