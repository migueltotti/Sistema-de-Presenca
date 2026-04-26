#include "lcd_display.h"
#include "http_client.h"
#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN  32
#define RST_PIN 33

#define BTN_UP 17
#define BTN_DOWN 16
#define BTN_CONFIRM 18
#define BTN_CANCEL 19

#define LED_BLUE 0
#define LED_GREEN 2
#define LED_RED 4

/* Leitor RFID */
MFRC522 rfid(SS_PIN, RST_PIN);

enum AccessState {
  VALIDATING,
  GRANTED,
  DENIED
};

const char* ssid     = "Wokwi-GUEST";  // rede virtual do Wokwi
const char* password = "";              // sem senha
const char* apiUrl = "https://jsonplaceholder.typicode.com/todos/1";

bool classStarted = false;

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

// ====== begin: UseCase Methods ======
int getProfessorSubjectSelection(){
  if (totalSubjects == 0) return -1;

  showSelectionMethodsMessageToDisplay();
  
  int index = 0;

  while (true) {
    String subjectName = subjects[index].name.c_str();
    printSubjectNameToDisplay(subjectName);

    while (!isButtonPressed(BTN_CONFIRM) && !isButtonPressed(BTN_CANCEL) && !isButtonPressed(BTN_UP) && !isButtonPressed(BTN_DOWN)) {}

    delay(10);

    if(isButtonPressed(BTN_CONFIRM)){
      delay(10);
      if(isButtonPressed(BTN_CONFIRM)){
        while(isButtonPressed(BTN_CONFIRM)){}

        Serial.println("Disciplina selecionada: " + subjectName);
        delay(200);
        break;
      }
    }

    if(isButtonPressed(BTN_CANCEL)){
      delay(10);
      if(isButtonPressed(BTN_CANCEL)){
        while(isButtonPressed(BTN_CANCEL)){}
        index = -1;

        Serial.println("Operação cancelada. Voltando ao início.");
        delay(200);
        break;
      }
    }

    if(isButtonPressed(BTN_UP)){
      delay(10);
      if(isButtonPressed(BTN_UP)){
        while(isButtonPressed(BTN_UP)){}

        index = (index - 1 + totalSubjects) % totalSubjects; // navegação para cima
        delay(200);
      }
    }

    if(isButtonPressed(BTN_DOWN)){
      delay(10);
      if(isButtonPressed(BTN_DOWN)){
        while(isButtonPressed(BTN_DOWN)){}

        index = (index + 1) % totalSubjects; // navegação para baixo
        delay(200);
      }
    }
  }

  return index;
}

void startNewClass(){
  String professorUuid = getUuidFromRfidReader();

  printConsultingServerToDisplay();
  indicateStateWithLEDS(VALIDATING);

  bool fetchResult = fetchSubjects(apiUrl, professorUuid);
  if (!fetchResult) {
    // apresentar mensagem de erro no LCD e voltar para tela inicial
    indicateStateWithLEDS(DENIED);  
    showProfessorNotFoundErrorMessageToDisplay();
    return;
  }

  delay(1500); // simulando tempo de resposta do servidor

  // apresenta as matérias no LCD e retorna escolha
  int selectedIndex  = getProfessorSubjectSelection();

  if (selectedIndex  == -1) {
    return; // professor cancelou a escolha da matéria
  }

  Subject selectedSubject = subjects[selectedIndex];

  bool startClassResult = startClassWithProfessorAndSubject(apiUrl, professorUuid, selectedSubject.id.c_str());

  if (startClassResult){
    classStarted = true;
    indicateStateWithLEDS(GRANTED);
    delay(1500);
    turnOfAllLeds();
  }
  else{
    classStarted = false;
    indicateStateWithLEDS(DENIED);
    showSubjectNotFoundErrorMessageToDisplay();
    delay(1500);
    turnOfAllLeds();
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
  pinMode(BTN_CONFIRM, INPUT); // pull-up externo
  pinMode(BTN_CANCEL, INPUT); // pull-up externo
  // botão pressionado = LOW
  // botão normal = HIGH

  turnOfAllLeds();

  lcdInit();

  Serial.begin(115200);
  Serial.println("Hello, ESP32!");

  SPI.begin(25, 27, 26, 32);
  rfid.PCD_Init();
  Serial.println("MFRC522 Ready");

  httpInit(ssid, password);

  showStartupMessageToDisplay();
}

void loop() {
  delay(10);

  // processo que inicia aula se não existir uma ativa no microcontrolador.
  if (!classStarted){
    showStartOrContinueClassMessageToDisplay();

    // incluir rotina de apertar botão para continuar aula.

    // tag ainda não aproximada do leitor, volta para mostrar a messagem de aproximação
    if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
      return;
    }

    indicateStateWithLEDS(VALIDATING);

    showReadingTagMessageToDisplay();
    printTagUuidToConsole();

    startNewClass();

    if (classStarted)
      showApproachTagMessageToDisplay();
  }

  // verifica se quer terminar a aula pressionando botão CONFIRM

  // verifica se quer cancelar a aula pressionando botão CANCEL

  // tentar ler a tag do aluno
  if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
    return;
  }

  indicateStateWithLEDS(VALIDATING);

  showReadingTagMessageToDisplay();

  printTagUuidToConsole();
  printTagUuidToDisplay(getUuidFromRfidReader());

  rfid.PICC_HaltA();
  delay(1000);

  showApproachTagMessageToDisplay();
  turnOfAllLeds();
}