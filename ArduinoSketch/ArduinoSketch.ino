//pins to control 3-colors led
#define redComponentPin 9
#define greenComponentPin 10
#define blueComponentPin 11
//pin connected to buzzer
#define buzzerPin 6
//analog pin to control the speed of the music in case of an incoming call
#define songSpeedPin 0


#include <CmdMessenger.h>
#include "SuperMarioMelody.h"

CmdMessenger cmdMessenger = CmdMessenger(Serial);

//command types (pc to arduino)
enum Commands
{
	Ack = 1,
	Error,
	Availability,
	IncomingCalls,
};

//availabilities (pc to arduino)
enum AvailabilityStatus
{
	Available = 1,
	Busy = 2,
	Away = 3,
	DoNotDisturb = 4
};

//true if there is an incoming call and buzzer should play music
bool areThereIncomingCalls = false;
int sumOfDelaysForSerial = 0; //used play music without blocking serial reading
float nextPause = 1000; //wait time before playing next note.

void setup()
{
	//start serial communication
	Serial.begin(9600);

	//attach command messenger to events
	cmdMessenger.attach(OnUnknownCommand);
	cmdMessenger.attach(Availability, UpdateAvailability);
	cmdMessenger.attach(IncomingCalls, UpdateIncomingCalls);
	cmdMessenger.sendCmd(Ack, "Arduino has started!");

	//define pins mode
	pinMode(redComponentPin, OUTPUT);
	pinMode(greenComponentPin, OUTPUT);
	pinMode(blueComponentPin, OUTPUT);
	pinMode(buzzerPin, OUTPUT);
}

// Arduino received an unknown command from pc. Arduino replies with and error message
void OnUnknownCommand()
{
	cmdMessenger.sendCmd(Error, "Command without attached callback");
}

//set new color
void setColor(byte redValue, byte greenValue, byte blueValue) {
	analogWrite(redComponentPin, redValue);
	analogWrite(greenComponentPin, greenValue);
	analogWrite(blueComponentPin, blueValue);
}

//lync availability chaned => change led's color accordingly
void UpdateAvailability()
{
	int availabilityStatus = cmdMessenger.readInt16Arg();
	switch (availabilityStatus) {
	case Available:
		setColor(0, 255, 0); //green
		break;
	case Busy:
		setColor(255, 0, 0); //red
		break;
	case Away:
		setColor(255, 50, 0); //yellow (sort of)
		break;
	case DoNotDisturb:
		setColor(255, 0, 10); // dark red (sor of)
		break;
	}
}

// number of incoming calls changed (0, 1)
// change the value of the variable areThereIncomingCalls, used in Loop to play music if true.
void UpdateIncomingCalls()
{
	int callsQuantity = cmdMessenger.readInt16Arg();
	if (callsQuantity == 0) {
		areThereIncomingCalls = false;
	}
	else {
		areThereIncomingCalls = true;
	}
}


void loop()
{
	//read from serial every 2 seconds
	if (sumOfDelaysForSerial > 2000) {
		cmdMessenger.feedinSerialData();
		sumOfDelaysForSerial -= 2000;
	}

	//read speed
	float songSpeed = 1.3 + (float)analogRead(songSpeedPin) / 512.0;


	if (areThereIncomingCalls) {
		nextPause = moveToNextStepSong(songSpeed);
	}
	else {
		nextPause = 1000;
		noTone(buzzerPin);
	}


	delay(nextPause);
	sumOfDelaysForSerial += nextPause;
}
