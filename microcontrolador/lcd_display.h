#ifndef LCD_DISPLAY_H
#define LCD_DISPLAY_H

#include <LiquidCrystal_I2C.h>

void lcdInit();
void showStartupMessageToDisplay();
void showStartOrContinueClassMessageToDisplay();
void showApproachTagMessageToDisplay();
void showReadingTagMessageToDisplay();
void printTagUuidToDisplay(String uuid);
void printConsultingServerToDisplay();
void printAccessGrantedToDisplay();
void printAccessDeniedToDisplay();
void showSelectionMethodsMessageToDisplay();
void showProfessorNotFoundErrorMessageToDisplay();
void showSubjectNotFoundErrorMessageToDisplay();
void printSubjectNameToDisplay(String subjectName);
void showConfirmActionMessageToDisplay();
void showClassContinuedSuccessfullyMessageToDisplay();
void showWifiNotConnectedErrorMessageToDisplay();
void showRequestErrorMessageToDisplay();
void showClassNotFoundErrorMessageToDisplay();
void showProfessorNotFoundErrorMessageToDisplay();
void showClassProfessorMismatchErrorMessageToDisplay();
void showClassAlreadyFinishedErrorMessageToDisplay();

#endif