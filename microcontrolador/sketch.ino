#include <LiquidCrystal_I2C.h>
#include <SPI.h>
#include <MFRC522.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h> 

#define SS_PIN  32
#define RST_PIN 33

#define BTN_UP 17
#define BTN_DOWN 16
#define BTN_CONFIRM 18
#define BTN_CANCEL 19

#define LED_BLUE 0
#define LED_GREEN 2
#define LED_RED 4

#define I2C_ADDR 0x27
#define LCD_COLUMNS 16
#define LCD_LINES 2
/* Display */
LiquidCrystal_I2C lcd(I2C_ADDR, LCD_COLUMNS, LCD_LINES);

/* Leitor RFID */
MFRC522 rfid(SS_PIN, RST_PIN);

enum AccessState {
  VALIDATING,
  GRANTED,
  DENIED
};

struct Subject {
  String id;
  String name;
};

const int MAX_SUBJECTS = 20; // mudar para dinamico depois
Subject subjects[MAX_SUBJECTS];
int totalSubjects = 0;

const char* ssid     = "Wokwi-GUEST";  // rede virtual do Wokwi
const char* password = "";              // sem senha

const char* apiUrl = "https://jsonplaceholder.typicode.com/todos/1";

bool classStarted = false;

// ====== begin: Display Print Methods ======

void showStartOrContinueClassMessageToDisplay(){
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(" Aproxime a tag ");
  lcd.setCursor(0, 1);
  lcd.print("  de professor  ");

  delay(1500);
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("       OU       ");

  delay(1500);
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Press. Confirmar");
  lcd.setCursor(0, 1);
  lcd.print(" continuar aula ");
}

void showApproachTagMessageToDisplay(){
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Aproxime uma tag");
  lcd.setCursor(1, 1);
  lcd.print(">>");
  lcd.setCursor(13, 1);
  lcd.print("<<");
}

void showStartupMessageToDisplay() {
  lcd.setCursor(2, 0);
  lcd.print("Iniciando...");
  delay(1000);
  lcd.clear();

  lcd.setCursor(3, 0);
  String message = "Sistema de";
  for (byte i = 0; i < message.length(); i++) {
    lcd.print(message[i]);
    delay(100);
  }

  lcd.setCursor(4, 1);
  message = "presenca";
  for (byte i = 0; i < message.length(); i++) {
    lcd.print(message[i]);
    delay(100);
  }

  delay(3000);
}

void printTagUuidToDisplay(){
  String uid = getUuidFromRfidReader();

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("UID detectado:");
  lcd.setCursor(0, 1);
  lcd.print(uid);
}

void printConsultingServerToDisplay(){
  lcd.clear();
  lcd.setCursor(2, 0);
  lcd.print("Consultando");
  lcd.setCursor(4, 1);
  lcd.print("Servidor");
}

void printAccessGrantedToDisplay(){
  lcd.clear();
  lcd.setCursor(5, 0);
  lcd.print("Acesso");
  lcd.setCursor(4, 1);
  lcd.print("Liberado");
}

void printAccessDeniedToDisplay(){
  lcd.clear();
  lcd.setCursor(5, 0);
  lcd.print("Acesso");
  lcd.setCursor(5, 1);
  lcd.print("Negado");
}

void showProfessorNotFoundErrorMessageToDisplay(){
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(" Professor não  ");
  lcd.setCursor(0, 1);
  lcd.print("   encontrado   ");

  delay(1500);
  
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Tente novamente ");
}

Subject getProfessorSubjectSelection(){
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("  Selecione uma ");
  lcd.setCursor(0, 1);
  lcd.print("   disciplina   ");

  delay(500);

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("  Utilizando os ");
  lcd.setCursor(0, 1);
  lcd.print("     botões     ");

  delay(500);

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(" >>   ↑  ↓   << ");
  lcd.setCursor(0, 1);
  lcd.print("Confim. / Cancel");

  delay(500);
  
  int index = 0;
  Subject selectedSubject;
  while (index < totalSubjects) {
    String subjectName = subjects[index].name.c_str();
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Disciplina:");
    lcd.setCursor(0, 1);
    lcd.print(subjectName);

    while (!isButtonPressed(BTN_CONFIRM) && !isButtonPressed(BTN_CANCEL) && !isButtonPressed(BTN_UP) && !isButtonPressed(BTN_DOWN)) {}
      // espera o usuário pressionar um botão para navegar ou confirmar a seleção}

    if (isButtonPressed(BTN_CONFIRM)) {
      // lógica para confirmar a seleção da disciplina
      selectedSubject = subjects[index];

      Serial.println("Disciplina selecionada: " + subjectName);
      delay(200);
      break;
    }

    if (isButtonPressed(BTN_CANCEL)) {
      // lógica para cancelar escolha e voltar ao inicio de tudo.
      Serial.println("Operação cancelada. Voltando ao início.");
      delay(200);
      break;
    }

    if (isButtonPressed(BTN_UP)) {
      index = (index - 1 + totalSubjects) % totalSubjects; // navegação para cima
      delay(200);
    }

    if (isButtonPressed(BTN_DOWN)) {
      index = (index + 1) % totalSubjects; // navegação para baixo
      delay(200);
    }
  }

  return selectedSubject;
}
// ====== end: Display Print Methods ======

// ====== begin: LEDs Methods ======
void turnOfAllLeds() {
  digitalWrite(LED_BLUE, LOW);
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_RED, LOW);
}

void indicateStateWithLEDS(AccessState state){
  turnOfAllLeds();

  switch (state) {
    case VALIDATING:
      digitalWrite(LED_BLUE, HIGH);
      break;

    case GRANTED:
      digitalWrite(LED_GREEN, HIGH);
      break;

    case DENIED:
      digitalWrite(LED_RED, HIGH);
      break;
  }
}
// ====== end: LEDs Methods ======

// ====== begin: Utils Methods ======
String getUuidFromRfidReader(){
  String uid = "";
  for (byte i = 0; i < rfid.uid.size; i++) {
    if (rfid.uid.uidByte[i] < 0x10) {
      uid += "0"; // Adiciona zero à esquerda para bytes menores que 0x10
    }
    uid += String(rfid.uid.uidByte[i], HEX);
    if (i < rfid.uid.size - 1) uid += ":"; // Separador opcional
  }
  uid.toUpperCase();

  return uid;
}

bool isButtonPressed(int btn) {
  return digitalRead(btn) == LOW;
}
// ====== end: Utils Methods ======

// ====== begin: Wifi Methods ======
void connectToWifi() {
  // Conecta ao Wi-Fi virtual
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
// ====== end: Wifi Methods ======

// ====== begin: API Methods ======
void getProfessorSubjects(String uuid){
  if (!isWifiConnected()) return;

  HTTPClient http;
  http.begin(apiUrl);

  int httpResponseCode = http.GET();

  if (httpResponseCode > 0) {
    Serial.printf("HTTP Status: %d\n", httpCode);

    if (httpCode == HTTP_CODE_OK) {
      String payload = http.getString();
      Serial.println("Resposta:");
      Serial.println(payload);

      DeserializationError erro = deserializeJson(doc, payload);
      if (erro) {
        Serial.print("Erro ao parsear JSON: ");
        Serial.println(erro.c_str());
        http.end();
        return;
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
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
  }

  http.end();
}

bool startClass(String professorUuid, String subjectUuid){
  if (!isWifiConnected()) return;

  HTTPClient http;
  http.begin(apiUrl);
  http.addHeader("Content-Type", "application/json");

  JsonDocument doc;
  doc["professorId"] = professorUuid;
  doc["subjectId"]  = subjectUuid;
  doc["active"] = true;

  String body;
  serializeJson(doc, body);  // converte para string JSON
  
  int httpCode = http.POST(body);

  if (httpResponseCode > 0) {
    Serial.printf("HTTP Status: %d\n", httpCode);

    if (httpCode == HTTP_CODE_OK) {
      String payload = http.getString();
      Serial.println("Resposta:");
      Serial.println(payload);
    }
    else if(httpCode == HTTP_CODE_BADREQUEST){
      Serial.println("Requisição inválida. Verifique os dados enviados.");

      http.end();
      return false;
    }
  } else {
    Serial.println("Erro na requisição: " + String(httpResponseCode));
  }

  http.end();
  return true;
}
// ====== end: API Methods ======

// ====== begin: UseCase Methods ======
void startNewClass(){
  // Trocar nome

  // pega uuid
  String professorUuid = getUuidFromRfidReader();
  // apresenta no LCD

  printConsultingServerToDisplay();
  indicateStateWithLEDS(VALIDATING);  
  // faz requisição pro servidor passando o uuid e pega as matérias do professor

  // JsonArray subjects = getProfessorSubjects(professorUuid);
  // if (totalSubjects == 0) {
  //   // apresentar mensagem de erro no LCD e voltar para tela inicial
  //   indicateStateWithLEDS(DENIED);  
  //   showProfessorNotFoundErrorMessageToDisplay();
  //   return;
  // }

  delay(1500); // simulando tempo de resposta do servidor

  // apresenta as matérias no LCD e retorna escolha
  Subject selectedSubject = getProfessorSubjectSelection();

  if (selectedSubject == null) {
    indicateStateWithLEDS(DENIED);  
    return; // professor cancelou a escolha da matéria
  }

  bool result = startClass(professorUuid, selectedSubject.id.c_str());

  if (result){
    classStarted = true;
    indicateStateWithLEDS(GRANTED);  
  }
}
// ====== end: UseCase Methods ======

// ====== begin: Console Print Methods ======
void printTagUuidToConsole(){
  Serial.print("UID:");
  Serial.println(getUuidFromRfidReader());
  Serial.println();
}
// ====== end: Console Print Methods ======

void setup() {
  pinMode(LED_BLUE, OUTPUT);
  pinMode(LED_GREEN, OUTPUT);
  pinMode(LED_RED, OUTPUT);

  pinMode(BTN_UP, INPUT); // pull-up externo
  pinMode(BTN_DOWN, INPUT); // pull-up externo
  pinMode(BTN_CONFIRM, INPUT_PULLUP);
  pinMode(BTN_CANCEL, INPUT_PULLUP);
  // botão pressionado = LOW
  // botão normal = HIGH

  turnOfAllLeds();

  //lcd.begin(16, 2);
  lcd.init();
  lcd.backlight();

  Serial.begin(115200);
  Serial.println("Hello, ESP32!");

  SPI.begin(25, 27, 26, 32);
  rfid.PCD_Init();
  Serial.println("MFRC522 Ready");

  connectToWifi();

  showStartupMessageToDisplay();
}

void loop() {
  delay(10);

  // verifica se o botão CONFIRMAR não foi pressionado
  // if(isButtonPressed(BTN_CONFIRM)){
  //   delay(10);
  //   if(isButtonPressed(BTN_CONFIRM)){
  //     while(isButtonPressed(BTN_CONFIRM)){}
  //     Serial.println("Botão CONFIRMAR Pressionado!");
  //   }
  // }

  // // verifica se o botão CANCELAR não foi pressionado
  // if(isButtonPressed(BTN_CANCEL)){
  //   delay(10);
  //   if(isButtonPressed(BTN_CANCEL)){
  //     while(isButtonPressed(BTN_CANCEL)){}
  //     Serial.println("Botão CANCELAR Pressionado!");
  //   }
  // }

  // // verifica se o botão ↑ não foi pressionado
  // if(isButtonPressed(BTN_UP)){
  //   delay(10);
  //   if(isButtonPressed(BTN_UP)){
  //     while(isButtonPressed(BTN_UP)){}
  //     Serial.println("Botão ↑ Pressionado!");
  //   }
  // }

  // // verifica se o botão ↓ não foi pressionado
  // if(isButtonPressed(BTN_DOWN)){
  //   delay(10);
  //   if(isButtonPressed(BTN_DOWN)){
  //     while(isButtonPressed(BTN_DOWN)){}
  //     Serial.println("Botão ↓ Pressionado!");
  //   }
  // }

  // processo que inicia aula se não existir uma ativa no microcontrolador.
  if (!classStarted){
    showStartOrContinueClassMessageToDisplay();

    // incluir rotina de apertar botão para continuar aula.

    // tag ainda não aproximada do leitor, volta para mostrar a messagem de aproximação
    if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
      return;
    }

    indicateStateWithLEDS(VALIDATING);

    lcd.clear();
    lcd.setCursor(2, 0);
    lcd.print("Lendo tag...");
    printTagUuidToConsole();

    startNewClass();
  }

  // verifica se quer terminar a aula pressionando botão CONFIRM

  // verifica se quer cancelar a aula pressionando botão CANCEL

  // tentar ler a tag do aluno
  if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
    return;
  }

  indicateStateWithLEDS(VALIDATING);

  lcd.clear();
  lcd.setCursor(2, 0);
  lcd.print("Lendo tag...");

  printTagUuidToConsole();
  printTagUuidToDisplay();

  rfid.PICC_HaltA();
  delay(1000);

  //showApproachTagMessageToDisplay();
  turnOfAllLeds();
}
