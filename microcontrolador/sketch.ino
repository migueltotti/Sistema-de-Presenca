#include <LiquidCrystal_I2C.h>
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

int cont = 0;

void showApproachTagMessage(){
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Aproxime uma tag");
  lcd.setCursor(1, 1);
  lcd.print(">>");
  lcd.setCursor(13, 1);
  lcd.print("<<");
}

void showStartupMessage() {
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

  showApproachTagMessage();

  delay(500);
}

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

void printTagUuidToConsole(){
  Serial.print("UID:");
  Serial.println(getUuidFromRfidReader());
  Serial.println();
}

void printTagUuidToLcd(){
  String uid = getUuidFromRfidReader();

  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("UID detectado:");
  lcd.setCursor(0, 1);
  lcd.print(uid);
}

void printConsultingServerToLcd(){
  lcd.clear();
  lcd.setCursor(2, 0);
  lcd.print("Consultando");
  lcd.setCursor(4, 1);
  lcd.print("Servidor");
}

void printAccessGrantedToLcd(){
  lcd.clear();
  lcd.setCursor(5, 0);
  lcd.print("Acesso");
  lcd.setCursor(4, 1);
  lcd.print("Liberado");
}

void printAccessDeniedToLcd(){
  lcd.clear();
  lcd.setCursor(5, 0);
  lcd.print("Acesso");
  lcd.setCursor(5, 1);
  lcd.print("Negado");
}

void turnOfAllLeds() {
  digitalWrite(LED_BLUE, LOW);
  digitalWrite(LED_GREEN, LOW);
  digitalWrite(LED_RED, LOW);
}

void indicateState(AccessState state){
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

bool isButtonPressed(int btn) {
  return digitalRead(btn) == LOW;
}

void validateTagUuid(){
  // Trocar nome

  // pega uuid
  String uuid = getUuidFromRfidReader();
  // faz requisição pro servidor passando o uuid
  // retorna um true ou false (sucesso ou falha)
  // apresenta no LCD

  delay(500);
  printConsultingServerToLcd();

  delay(1500);

  if (cont % 2 == 0){
    indicateState(GRANTED);
    printAccessGrantedToLcd();
  }
  else{
    indicateState(DENIED);
    printAccessDeniedToLcd();
  }

  delay(1000);
  cont++;
}

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

  showStartupMessage();
}

void loop() {
  delay(10);

  // verifica se o botão CONFIRMAR não foi pressionado
  if(isButtonPressed(BTN_CONFIRM)){
    delay(10);
    if(isButtonPressed(BTN_CONFIRM)){
      while(isButtonPressed(BTN_CONFIRM)){}
      Serial.println("Botão CONFIRMAR Pressionado!");
    }
  }

  // verifica se o botão CONFIRMAR não foi pressionado
  if(isButtonPressed(BTN_CANCEL)){
    delay(10);
    if(isButtonPressed(BTN_CANCEL)){
      while(isButtonPressed(BTN_CANCEL)){}
      Serial.println("Botão CANCELAR Pressionado!");
    }
  }

  // verifica se o botão CONFIRMAR não foi pressionado
  if(isButtonPressed(BTN_UP)){
    delay(10);
    if(isButtonPressed(BTN_UP)){
      while(isButtonPressed(BTN_UP)){}
      Serial.println("Botão ↑ Pressionado!");
    }
  }

  // verifica se o botão CONFIRMAR não foi pressionado
  if(isButtonPressed(BTN_DOWN)){
    delay(10);
    if(isButtonPressed(BTN_DOWN)){
      while(isButtonPressed(BTN_DOWN)){}
      Serial.println("Botão ↓ Pressionado!");
    }
  }

  // tentar ler a tag
  if (!rfid.PICC_IsNewCardPresent() || !rfid.PICC_ReadCardSerial()) {
    return;
  }

  indicateState(VALIDATING);

  lcd.clear();
  lcd.setCursor(2, 0);
  lcd.print("Lendo tag...");

  printTagUuidToConsole();
  printTagUuidToLcd();

  validateTagUuid();

  rfid.PICC_HaltA();
  delay(1000);

  showApproachTagMessage();
  turnOfAllLeds();
}
