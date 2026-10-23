#ifndef TYPES_H
#define TYPES_H

#include <Arduino.h>

constexpr size_t UID_LEN = 21;   // UID do MFRC522: até 10 bytes -> 20 hex + '\0'
constexpr size_t MSG_LEN = 17;   // LCD 16 colunas + '\0'

// Leitor e display andam em pares (interno/externo), então um enum serve aos dois.
enum Lado : uint8_t { LADO_INTERNO, LADO_EXTERNO };

// ---------- HTTP: controller -> httpTask ----------
enum HttpAction : uint8_t {
  HTTP_VERIFICAR_ACESSO,
  HTTP_REGISTRAR_ABERTURA,
  HTTP_REGISTRAR_FECHAMENTO,
  HTTP_REGISTRAR_PRESENCA
};

struct HttpRequest {
  uint32_t   requestId;          // gerado pelo controller, ecoado na resposta
  HttpAction action;
  char       uid[UID_LEN];
  uint32_t   aulaId;             // só usado em HTTP_REGISTRAR_PRESENCA
};

// ---------- HTTP: httpTask -> controller (vai dentro do AppEvent) ----------
struct HttpResultado {
  uint32_t   requestId;
  HttpAction action;             // ecoa a ação, para o controller conferir
  bool       falhaDeRede;        // timeout, sem WiFi, DNS... (diferente de "negado")
  int16_t    httpStatus;         // 200, 403, 500... (0 se falhaDeRede)
  bool       autorizado;         // campo do JSON de resposta
  char       mensagem[MSG_LEN];  // texto curto para o display (ex.: "Sem permissao")
};

// ---------- Evento geral: tudo que chega ao controller ----------
enum EventType : uint8_t {
  EVT_TAG,
  EVT_BTN_CONFIRMA,
  EVT_BTN_CANCELA,
  EVT_AULA_INICIOU,
  EVT_AULA_FINALIZOU,
  EVT_HTTP_RESULTADO,
  EVT_TIMEOUT                    // disparado por um software timer
};

struct AppEvent {
  EventType type;
  union {                        // só um é válido, conforme o type
    struct { Lado lado; char uid[UID_LEN]; } tag;   // EVT_TAG
    struct { uint32_t aulaId; }              aula;  // EVT_AULA_INICIOU
    HttpResultado                            http;  // EVT_HTTP_RESULTADO
  };
};

// ---------- Display: controller -> displayTask ----------
struct DisplayMsg {
  Lado     lado;
  char     linha1[MSG_LEN];
  char     linha2[MSG_LEN];
  uint16_t duracaoMs;            // 0 = permanente; >0 = temporária
};

static_assert(sizeof(AppEvent) <= 64, "AppEvent grande demais para a fila");

#endif