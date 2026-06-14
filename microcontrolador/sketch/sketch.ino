#include "lcd_display.h"
#include "http_client.h"
#include "nvs_storage.h"
#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN  32
#define RST_PIN 33

#define BTN_UP 17
#define BTN_DOWN 16
#define BTN_CONFIRM 18
#define BTN_CANCEL 19

#define LED_BLUE 15
#define LED_GREEN 2
#define LED_RED 4

/* Leitor RFID */
MFRC522 rfid(SS_PIN, RST_PIN);

enum AccessState {
  VALIDATING,
  GRANTED,
  DENIED
};

const char* ssid     = "MIGUEL";  // rede virtual do Wokwi
const char* password = "miguel2005";              // sem senha
const char* apiUrl = "https://dizygotic-ethelene-elvishly.ngrok-free.dev/api/v1";

bool sessionStarted = false;
String sessionId = "";

// Variáveis globais para edge detection
bool lastConfirmState = HIGH;
bool confirmJustPressed = false;

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

bool hasSessionStartedBefore() {
  String sessionId = getSessionIdFromNVS();
  if (sessionId != "")
    return true;
  else
    return false;
}
// ====== end: Utils Methods ======


// ====== begin: Console Print Methods ======
void printTagUuidToConsole(){
  Serial.print("UID:");
  Serial.println(getUuidFromRfidReader());
  Serial.println();
}
// ====== end: Console Print Methods ======

// ====== begin: LEDs Methods ======
void turnOffAllLeds() {
  digitalWrite(LED_BLUE, LOW);
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_RED, LOW);
}

void indicateStateWithLEDS(AccessState state){
  turnOffAllLeds();

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

int getNumberOfClassesFromProfessor() {
  int numberOfClasses = 1;

  showSelectNumberOfClassesMessageToDisplay();
  printNumberOfClassesToDisplay(numberOfClasses);

  while (true) {
    while (!isButtonPressed(BTN_CONFIRM) && !isButtonPressed(BTN_CANCEL) && !isButtonPressed(BTN_UP) && !isButtonPressed(BTN_DOWN)) {}

    delay(10);

    if(isButtonPressed(BTN_CONFIRM)){
      delay(10);
      if(isButtonPressed(BTN_CONFIRM)){
        while(isButtonPressed(BTN_CONFIRM)){}
        delay(200);
        break;
      }
    }

    if(isButtonPressed(BTN_CANCEL)){
      delay(10);
      if(isButtonPressed(BTN_CANCEL)){
        while(isButtonPressed(BTN_CANCEL)){}
        numberOfClasses = -1;
        delay(200);
        break;
      }
    }

    if(isButtonPressed(BTN_UP)){
      delay(10);
      if(isButtonPressed(BTN_UP)){
        while(isButtonPressed(BTN_UP)){}

        numberOfClasses++;
        printNumberOfClassesToDisplay(numberOfClasses);
        delay(200);
      }
    }

    if(isButtonPressed(BTN_DOWN)){
      delay(10);
      if(isButtonPressed(BTN_DOWN)){
        while(isButtonPressed(BTN_DOWN)){}

        if (numberOfClasses > 1) {
          numberOfClasses--;
          printNumberOfClassesToDisplay(numberOfClasses);
        }
        delay(200);
      }
    }
  }

  return numberOfClasses;
}

void startNewSession(){
  showReadingTagMessageToDisplay();
  printTagUuidToConsole();

  String professorUuid = getUuidFromRfidReader();

  printConsultingServerToDisplay();
  indicateStateWithLEDS(VALIDATING);

  bool fetchResult = fetchSubjects(apiUrl, professorUuid);
  if (!fetchResult) {
    indicateStateWithLEDS(DENIED);  
    showProfessorNotFoundErrorMessageToDisplay();
    return;
  }

  turnOffAllLeds();

  // apresenta as matérias no LCD e retorna escolha
  int selectedIndex = getProfessorSubjectSelection();

  if (selectedIndex == -1) {
    return; // professor cancelou a escolha da matéria
  }

  // pergunta qual o numero de aulas que serão dadas
  int numberOfClasses = getNumberOfClassesFromProfessor();

  if (numberOfClasses == -1) {
    lcd.clear();
    return; // professor cancelou a escolha do número de aulas
  }

  Subject selectedSubject = subjects[selectedIndex];

  StartSessionResponse classResponse = startSessionWithProfessorAndSubject(apiUrl, professorUuid, selectedSubject.id.c_str(), numberOfClasses);

  if (classResponse.isSuccess) {
    sessionStarted = true;
    sessionId = classResponse.sessionId;
    saveSessionIdToNVS(sessionId);
    showSessionStartedSuccessfullyMessageToDisplay();
    indicateStateWithLEDS(GRANTED);
    delay(1500);
    turnOffAllLeds();
  }
  else{
    sessionStarted = false;
    sessionId = "";
    showSubjectNotFoundErrorMessageToDisplay();
    indicateStateWithLEDS(DENIED);
    delay(1500);
    turnOffAllLeds();
  }
}

bool getProfessorConfirmationToContinueSession() {
  delay(500);
  showConfirmContinueSessionActionMessageToDisplay();

  // Aguarda qualquer um dos dois botões ser pressionado
  while (!isButtonPressed(BTN_CONFIRM) && !isButtonPressed(BTN_CANCEL)) {}

  // Identifica qual foi pressionado AGORA (ainda pressionado)
  bool confirmed = isButtonPressed(BTN_CONFIRM);

  // Aguarda soltar o botão antes de continuar (evita propagação)
  while (isButtonPressed(BTN_CONFIRM) || isButtonPressed(BTN_CANCEL)) {}

  delay(50); // debounce após soltar

  return confirmed;
}

void continueSession() {
  // mostrar mensagem de confirmação da ação (CONFIRM - Continuar, CANCEL - Cancelar)
  // aguardar confirmação no botão
  bool continueSession = getProfessorConfirmationToContinueSession();

  if (!continueSession){
    lcd.clear();
    return;
  }

  // mostra mensagem para professor aproximar a tag
  // aguardar aproximação da tag
  while (true) {
    showApproachProfessorTagMessageToDisplay();

    while(!isButtonPressed(BTN_CANCEL) && (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial())) {}

    if(isButtonPressed(BTN_CANCEL)){
      delay(10);
      if(isButtonPressed(BTN_CANCEL)){
        while(isButtonPressed(BTN_CANCEL)){}
        lcd.clear();
        return; // professor cancelou a ação de continuar a aula - volta para o loop principal
      }
    }

    // obter uuid da tag do professor
    // envia requisição para o servidor para continuar a aula com base no classId salvo no NVS e no uuid do professor
    indicateStateWithLEDS(VALIDATING);
    showReadingTagMessageToDisplay();
    printTagUuidToConsole();

    String professorUuid = getUuidFromRfidReader();
    sessionId = getSessionIdFromNVS();

    ContinueSessionResponse continueSessionResponse = continueSessionByProfessor(apiUrl, sessionId, professorUuid);

    // mostra mensagem de sucesso
    // volta para o loop principal
    if (continueSessionResponse.isSuccess) {
      sessionStarted = true;
      indicateStateWithLEDS(GRANTED);
      showSessionContinuedSuccessfullyMessageToDisplay();
      turnOffAllLeds();
      return;
    }
    else {
      sessionStarted = false;
      indicateStateWithLEDS(DENIED);

      if (continueSessionResponse.errorCode == WIFI_NOT_CONNECTED_ERROR) {
        showWifiNotConnectedErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (continueSessionResponse.errorCode == REQUEST_ERROR) {
        showRequestErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (continueSessionResponse.errorCode == "SessionErrors.NotFound"){ // Aula não encontrada - volta para o loop inicial
        showSessionNotFoundErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (continueSessionResponse.errorCode == "ProfessorErrors.NotFound"){ // Professor não encontrado - aguarda tag novamente e mostra mensagem de erro no LCD
        showProfessorNotFoundErrorMessageToDisplay();
        turnOffAllLeds();
        // aguarda tag novamente e mostra mensagem de erro no LCD
      }
      else if (continueSessionResponse.errorCode == "SessionErrors.ProfessorMismatch"){ // Professor não iniciou a aula - aguarda tag novamente e mostra mensagem de erro no LCD
        showSessionProfessorMismatchErrorMessageToDisplay();
        turnOffAllLeds();
        // aguarda tag novamente e mostra mensagem de erro no LCD
      }
      else if (continueSessionResponse.errorCode == "SessionErrors.AlreadyFinished"){ // Aula já finalizada - volta para o loop inicial
        showSessionAlreadyFinishedErrorMessageToDisplay();
        turnOffAllLeds();
        return;// volta para o loop inicial
      }
    }
  }
}

bool getProfessorConfirmationToEndSession() {
  delay(500);
  showConfirmEndSessionActionMessageToDisplay();

  while (!isButtonPressed(BTN_CONFIRM) && !isButtonPressed(BTN_CANCEL)) {}

  if(isButtonPressed(BTN_CONFIRM)){
    delay(10);
    if(isButtonPressed(BTN_CONFIRM)){
      while(isButtonPressed(BTN_CONFIRM)){}
      return true;
    }
  }

  if(isButtonPressed(BTN_CANCEL)){
    delay(10);
    if(isButtonPressed(BTN_CANCEL)){
      while(isButtonPressed(BTN_CANCEL)){}
      return false;
    }
  }

  return false;
}

void endSession(){
  // confirma ação da ação de finalizar a aula (CONFIRM - Finalizar, CANCEL - Cancelar)
  bool endSessionConfirmation = getProfessorConfirmationToEndSession();

  if (!endSessionConfirmation){
    lcd.clear();
    return;
  }
    
  while (true) {
    showApproachProfessorTagMessageToDisplay();

    while(!isButtonPressed(BTN_CANCEL) && (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial())) {}

    if(isButtonPressed(BTN_CANCEL)){
      delay(10);
      if(isButtonPressed(BTN_CANCEL)){
        while(isButtonPressed(BTN_CANCEL)){}
        lcd.clear();
        return; // professor cancelou a ação de finalizar a aula - volta para o loop principal
      }
    }

    indicateStateWithLEDS(VALIDATING);
    showReadingTagMessageToDisplay();
    printTagUuidToConsole();

    // pega tag uuid do professor
    // id da aula está na variavel classId
    String professorUuid = getUuidFromRfidReader();

    // envia requisição para o servidor para finalizar a aula
    EndSessionResponse endSessionResponse = endSessionByProfessor(apiUrl, sessionId, professorUuid);

    // mostra mensagem de sucesso
    // volta para o loop inicial com a aula finalizada
    if (endSessionResponse.isSuccess) {
      sessionStarted = false;
      removeSessionIdToNVS();
      indicateStateWithLEDS(GRANTED);
      showSessionEndedSuccessfullyMessageToDisplay();
      turnOffAllLeds();
      return;
    }
    else {
      indicateStateWithLEDS(DENIED);

      if (endSessionResponse.errorCode == WIFI_NOT_CONNECTED_ERROR) {
        showWifiNotConnectedErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (endSessionResponse.errorCode == REQUEST_ERROR) {
        showRequestErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (endSessionResponse.errorCode == "SessionErrors.NotFound"){ // Aula não encontrada - volta para o loop inicial
        showSessionNotFoundErrorMessageToDisplay();
        turnOffAllLeds();
        return; // volta para o loop inicial
      }
      else if (endSessionResponse.errorCode == "UserErrors.ProfessorNotFound"){ // Professor não encontrado - aguarda tag novamente e mostra mensagem de erro no LCD
        showProfessorNotFoundErrorMessageToDisplay();
        turnOffAllLeds();
        // aguarda tag novamente e mostra mensagem de erro no LCD
      }
      else if (endSessionResponse.errorCode == "SessionErrors.ProfessorMismatch"){ // Professor não iniciou a aula - aguarda tag novamente e mostra mensagem de erro no LCD
        showSessionProfessorMismatchErrorMessageToDisplay();
        turnOffAllLeds();
        // aguarda tag novamente e mostra mensagem de erro no LCD
      }
    }
  }
}

void registerStudentAttendance() {
  indicateStateWithLEDS(VALIDATING);
  showReadingTagMessageToDisplay();
  printTagUuidToConsole();

  // pega tag uuid do aluno
  // id da aula está na variavel classId
  String studentUuid = getUuidFromRfidReader();

  RegisterAttendanceResponse registerAttendanceResponse = registerAttendanceByStudent(apiUrl, sessionId, studentUuid);

  if (registerAttendanceResponse.isSuccess) {
    indicateStateWithLEDS(GRANTED);
    showAttendanceRegisteredSuccessfullyMessageToDisplay();
  }
  else {
    indicateStateWithLEDS(DENIED);

    if (registerAttendanceResponse.errorCode == WIFI_NOT_CONNECTED_ERROR) {
      showWifiNotConnectedErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == REQUEST_ERROR) {
      showRequestErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == "SessionErrors.NotFound"){
      showSessionNotFoundErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == "SessionErrors.AlreadyFinished"){
      showSessionAlreadyFinishedErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == "UserErrors.StudentNotFound"){
      showStudentNotFoundErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == "SessionErrors.StudentNotFound"){
      showStudentNotFoundInSessionErrorMessageToDisplay();
    }
    else if (registerAttendanceResponse.errorCode == "SessionErrors.StudentAttendanceAlreadyRegistered"){
      showStudentAttendanceAlreadyRegisteredErrorMessageToDisplay();
    }
  }

  turnOffAllLeds();
}
// ====== end: UseCase Methods ======

void setup() {
  pinMode(LED_BLUE, OUTPUT);
  pinMode(LED_GREEN, OUTPUT);
  pinMode(LED_RED, OUTPUT);

  pinMode(BTN_UP, INPUT_PULLUP); // pull-up externo
  pinMode(BTN_DOWN, INPUT_PULLUP); // pull-up externo
  pinMode(BTN_CONFIRM, INPUT_PULLUP); // pull-up externo
  pinMode(BTN_CANCEL, INPUT_PULLUP); // pull-up externo
  // botão pressionado = LOW
  // botão normal = HIGH

  turnOffAllLeds();

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

  bool currentConfirmState = digitalRead(BTN_CONFIRM);
  confirmJustPressed = (lastConfirmState == HIGH && currentConfirmState == LOW);
  lastConfirmState = currentConfirmState;
  
  bool newCard = rfid.PICC_IsNewCardPresent() && rfid.PICC_ReadCardSerial();

  if (isButtonPressed(BTN_CONFIRM)){
    Serial.println("Botão confirm pressionado!");
  }

  if (isButtonPressed(BTN_CANCEL)){
    Serial.println("Botão cancel pressionado!");
    indicateStateWithLEDS(DENIED); 
    delay(500);
    turnOffAllLeds();
  }

  if (isButtonPressed(BTN_UP)){
    Serial.println("Botão up pressionado!");
  }

  if (isButtonPressed(BTN_DOWN)){
    Serial.println("Botão down pressionado!");
  }

  // processo que inicia aula se não existir uma ativa no microcontrolador.
  if (!sessionStarted){
    showStartOrContinueSessionMessageToDisplay();

    // incluir rotina de apertar botão para continuar aula.
    if (hasSessionStartedBefore() && confirmJustPressed){
      continueSession();

      if (sessionStarted){
        showApproachTagMessageToDisplay();
      }
    }

    // tag aproximada do leitor para iniciar nova aula
    if (newCard) {
      startNewSession();
    }

    if (sessionStarted){
      showApproachTagMessageToDisplay();
    }
  }
  else{ // Aula já em andamento
    // verifica se quer terminar a aula pressionando botão CONFIRM
    if (sessionStarted && confirmJustPressed){
      endSession();

      if (sessionStarted){
        showApproachTagMessageToDisplay();
      }
    }

    // tentar ler a tag do aluno
    if (newCard) {
      registerStudentAttendance();
      
      showApproachTagMessageToDisplay();
    }
  }

  rfid.PICC_HaltA();
  turnOffAllLeds();
}