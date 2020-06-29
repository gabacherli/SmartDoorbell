#include <Firmata.h>

int pinCampainha = 3;

void setup()
{
  Firmata.setFirmwareVersion(FIRMATA_FIRMWARE_MAJOR_VERSION, FIRMATA_FIRMWARE_MINOR_VERSION);
  Firmata.begin(9600);
}

void loop()
{
  while (Firmata.available()) {
    Firmata.processInput();
  }
  
  if (digitalRead(pinCampainha) == HIGH)
  {
    Serial.println(digitalRead(pinCampainha));
    delay (2000);
  }
}