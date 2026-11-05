#pragma once
// ============================================================================
// display_task.h
// Tudo que a displayTask precisa: estado, funções auxiliares e a própria task.
// Pré-requisito: tipos.h (Lado, DisplayMsg, MSG_LEN) e a fila displayQueue
// criada no setup() ANTES de a task iniciar.
// ============================================================================
#include <Arduino.h>
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include "types.h"

extern QueueHandle_t displayQueue;   // definida no .ino principal

// ----------------------------------------------------------------------------
// Configuração
// ----------------------------------------------------------------------------
constexpr uint8_t LCD_ADDR = 0x27;
constexpr uint8_t LCD_COLS = 16;
constexpr uint8_t LCD_ROWS = 2;

constexpr uint8_t PIN_SDA_EXTERNO = 21;   // Wire  (barramento 0)
constexpr uint8_t PIN_SCL_EXTERNO = 22;
constexpr uint8_t PIN_SDA_INTERNO = 16;   // Wire1 (barramento 1)
constexpr uint8_t PIN_SCL_INTERNO = 17;

// ----------------------------------------------------------------------------
// ÚNICO trecho dependente da biblioteca de LCD.
// Os dois LCDs têm o mesmo endereço, mas ficam em barramentos diferentes,
// então a biblioteca precisa permitir escolher o TwoWire (Wire / Wire1).
// Ajuste o construtor e o método de inicialização (init() ou begin())
// conforme a biblioteca que vocês usarem.
// ----------------------------------------------------------------------------
static LiquidCrystal_I2C lcdInterno(LCD_ADDR, LCD_COLS, LCD_ROWS);
//static LiquidCrystal_I2C lcdExterno(&Wire,  LCD_ADDR, LCD_COLS, LCD_ROWS);

// Índice = valor do enum Lado (LADO_INTERNO = 0, LADO_EXTERNO = 1)
static LiquidCrystal_I2C* const lcds[1] = { &lcdInterno, /*&lcdExterno*/ };

// ----------------------------------------------------------------------------
// Estado (acessado SOMENTE pela displayTask, por isso não precisa de mutex)
// ----------------------------------------------------------------------------
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
  //Wire.begin(PIN_SDA_EXTERNO, PIN_SCL_EXTERNO);
  Wire.begin(PIN_SDA_INTERNO, PIN_SCL_INTERNO);

  for (uint8_t i = 0; i < 1; i++) {
    lcds[i]->init();
    lcds[i]->backlight();
    lcds[i]->clear();
  }
}

// ----------------------------------------------------------------------------
// Desenho
// ----------------------------------------------------------------------------

// Escreve uma linha completa (preenche com espaços até o fim).
// Isso evita lcd.clear() a cada mensagem, que causa "piscada" no display.
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

  for (uint8_t i = 0; i < 1; i++) {
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

// ----------------------------------------------------------------------------
// API para as OUTRAS tasks (usada principalmente pela controllerTask)
// ----------------------------------------------------------------------------
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

// ----------------------------------------------------------------------------
// Roteiro de teste (cole no setup(), depois de criar a displayTask):
//
//   displayInit();                                         // antes das tasks
//   displayQueue = xQueueCreate(8, sizeof(DisplayMsg));    // antes das tasks
//   xTaskCreatePinnedToCore(displayTask, "display", 4096, nullptr, 1, nullptr, 1);
//
//   mostrar(LADO_INTERNO, "Nenhuma aula",  "em andamento");          // permanente
//   delay(2000);
//   mostrar(LADO_INTERNO, "Aula iniciada", "Aproxime a tag");        // permanente
//   delay(2000);
//   mostrar(LADO_INTERNO, "Presenca OK",   "Maria Silva", 2000);     // volta sozinho
//   mostrar(LADO_EXTERNO, "Porta trancada","Aproxime a tag");        // outro display
// ----------------------------------------------------------------------------