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
void showSelectNumberOfClassesMessageToDisplay();
void printNumberOfClassesToDisplay(int numberOfClasses);
void showProfessorNotFoundErrorMessageToDisplay();
void showSubjectNotFoundErrorMessageToDisplay();
void printSubjectNameToDisplay(String subjectName);
void showConfirmContinueClassActionMessageToDisplay();
void showClassContinuedSuccessfullyMessageToDisplay();
void showConfirmEndClassActionMessageToDisplay();
void showClassEndedSuccessfullyMessageToDisplay();
void showWifiNotConnectedErrorMessageToDisplay();
void showRequestErrorMessageToDisplay();
void showClassNotFoundErrorMessageToDisplay();
void showProfessorNotFoundErrorMessageToDisplay();
void showClassProfessorMismatchErrorMessageToDisplay();
void showClassAlreadyFinishedErrorMessageToDisplay();

#endif