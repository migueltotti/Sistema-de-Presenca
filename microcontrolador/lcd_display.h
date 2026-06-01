#ifndef LCD_DISPLAY_H
#define LCD_DISPLAY_H

#include <LiquidCrystal_I2C.h>

void lcdInit();
void showStartupMessageToDisplay();
void showStartOrContinueSessionMessageToDisplay();
void showApproachTagMessageToDisplay();
void showApproachProfessorTagMessageToDisplay();
void showReadingTagMessageToDisplay();
void printTagUuidToDisplay(String uuid);
void printConsultingServerToDisplay();
void printAccessGrantedToDisplay();
void printAccessDeniedToDisplay();
void showSelectionMethodsMessageToDisplay();
void showSelectNumberOfSessionesMessageToDisplay();
void printNumberOfSessionesToDisplay(int numberOfClasses);
void showProfessorNotFoundErrorMessageToDisplay();
void showSubjectNotFoundErrorMessageToDisplay();
void printSubjectNameToDisplay(String subjectName);
void showConfirmContinueSessionActionMessageToDisplay();
void showSessionContinuedSuccessfullyMessageToDisplay();
void showConfirmEndSessionActionMessageToDisplay();
void showSessionEndedSuccessfullyMessageToDisplay();
void showWifiNotConnectedErrorMessageToDisplay();
void showRequestErrorMessageToDisplay();
void showSessionNotFoundErrorMessageToDisplay();
void showProfessorNotFoundErrorMessageToDisplay();
void showSessionProfessorMismatchErrorMessageToDisplay();
void showSessionAlreadyFinishedErrorMessageToDisplay();
void showAttendanceRegisteredSuccessfullyMessageToDisplay();
void showStudentNotFoundErrorMessageToDisplay();
void showStudentAttendanceAlreadyRegisteredErrorMessageToDisplay();

#endif