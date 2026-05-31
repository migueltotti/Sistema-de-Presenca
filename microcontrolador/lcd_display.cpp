#include "lcd_display.h"

#define I2C_ADDR 0x27
#define LCD_COLUMNS 16
#define LCD_LINES 2

enum DisplayMsgState {
  MSG_APPROACH_TAG,
  MSG_OR,
  MSG_PRESS_CONFIRM
};

LiquidCrystal_I2C lcd(I2C_ADDR, LCD_COLUMNS, LCD_LINES);

DisplayMsgState currentMsgState = MSG_APPROACH_TAG;
unsigned long lastMsgChange = 0;
const unsigned long MSG_INTERVAL = 1500;

void lcdInit() {
    lcd.init();
    lcd.backlight();
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

void showStartOrContinueSessionMessageToDisplay() {
    unsigned long now = millis();

    if (now - lastMsgChange < MSG_INTERVAL) return; // ainda não é hora de trocar
    lastMsgChange = now;

    switch (currentMsgState) {
        case MSG_APPROACH_TAG:
            lcd.clear();
            lcd.setCursor(0, 0);
            lcd.print(" Aproxime a tag ");
            lcd.setCursor(0, 1);
            lcd.print("  de professor  ");
            currentMsgState = MSG_OR;
            break;

        case MSG_OR:
            lcd.clear();
            lcd.setCursor(0, 0);
            lcd.print("       OU       ");
            currentMsgState = MSG_PRESS_CONFIRM;
            break;

        case MSG_PRESS_CONFIRM:
            lcd.clear();
            lcd.setCursor(0, 0);
            lcd.print("Press. Confirmar");
            lcd.setCursor(0, 1);
            lcd.print(" continuar aula ");
            currentMsgState = MSG_APPROACH_TAG;
            break;
    }
}

void showApproachTagMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Aproxime uma tag");
    lcd.setCursor(1, 1);
    lcd.print(">>");
    lcd.setCursor(13, 1);
    lcd.print("<<");
}

void showReadingTagMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(2, 0);
    lcd.print("Lendo tag...");
}

void printTagUuidToDisplay(String uuid) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("UUID detectado:");
    lcd.setCursor(0, 1);
    lcd.print(uuid);
}

void printConsultingServerToDisplay() {
    lcd.clear();
    lcd.setCursor(2, 0);
    lcd.print("Consultando");
    lcd.setCursor(4, 1);
    lcd.print("Servidor");
}

void printAccessGrantedToDisplay() {
    lcd.clear();
    lcd.setCursor(5, 0);
    lcd.print("Acesso");
    lcd.setCursor(4, 1);
    lcd.print("Liberado");
}

void printAccessDeniedToDisplay() {
    lcd.clear();
    lcd.setCursor(5, 0);
    lcd.print("Acesso");
    lcd.setCursor(5, 1);
    lcd.print("Negado");
}

void showSelectionMethodsMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("  Selecione uma ");
    lcd.setCursor(0, 1);
    lcd.print("   disciplina   ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("  Utilizando os ");
    lcd.setCursor(0, 1);
    lcd.print("     botoes     ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("   Cima | Baixo ");
    lcd.setCursor(0, 1);
    lcd.print("Confim. | Cancel");

    delay(1500);
}

void showSelectNumberOfClassesMessageToDisplay(){
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("  Selecione o   ");
    lcd.setCursor(0, 1);
    lcd.print("numero de aulas ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("  Utilizando os ");
    lcd.setCursor(0, 1);
    lcd.print("     botoes     ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("   Cima | Baixo ");
    lcd.setCursor(0, 1);
    lcd.print("Confim. | Cancel");

    delay(1500);
}

void printNumberOfClassesToDisplay(int numberOfClasses) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Numero de aulas:");
    lcd.setCursor(0, 1);
    lcd.print(numberOfClasses);
}

void showProfessorNotFoundErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(" Professor não  ");
    lcd.setCursor(0, 1);
    lcd.print("   encontrado   ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showSubjectNotFoundErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(" Disciplina não ");
    lcd.setCursor(0, 1);
    lcd.print("   encontrada   ");

    delay(1500);

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void printSubjectNameToDisplay(String subjectName) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Disciplina:");
    lcd.setCursor(0, 1);
    lcd.print(subjectName);
}

void showConfirmContinueSessionActionMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Deseja continuar");
    lcd.setCursor(0, 1);
    lcd.print("     aula ?     ");
}

void showSessionContinuedSuccessfullyMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Aula continuada");
    lcd.setCursor(0, 1);
    lcd.print(" com sucesso ");

    delay(1500);
}

void showConfirmEndSessionActionMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Deseja finalizar");
    lcd.setCursor(0, 1);
    lcd.print("     aula ?     ");
}

void showSessionEndedSuccessfullyMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Aula finalizada");
    lcd.setCursor(0, 1);
    lcd.print(" com sucesso ");

    delay(1500);
}

void showWifiNotConnectedErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("   ERROR: Wifi  ");
    lcd.setCursor(0, 1);
    lcd.print("  NAO CONECTADO ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showRequestErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(" ERROR: Request ");
    lcd.setCursor(0, 1);
    lcd.print("     falhou     ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showSessionNotFoundErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("    Aula não    ");
    lcd.setCursor(0, 1);
    lcd.print("   encontrada   ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showSessionProfessorMismatchErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(" Professor não  ");
    lcd.setCursor(0, 1);
    lcd.print(" iniciou a aula ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showSessionAlreadyFinishedErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("    Aula já     ");
    lcd.setCursor(0, 1);
    lcd.print("   terminada    ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showAttendanceRegisteredSuccessfullyMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("   Presença do  ");
    lcd.setCursor(0, 1);
    lcd.print("aluno registrada");

    delay(1500);
}

void showStudentNotFoundErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("   Aluno não    ");
    lcd.setCursor(0, 1);
    lcd.print("   encontrado   ");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}

void showStudentAttendanceAlreadyRegisteredErrorMessageToDisplay() {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("    Aluno já    ");
    lcd.setCursor(0, 1);
    lcd.print(" possui presenca");

    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Tente novamente ");

    delay(1500);
}