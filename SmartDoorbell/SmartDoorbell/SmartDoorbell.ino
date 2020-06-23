#include <SPI.h>
#include <Ethernet.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

uint8_t pinCampainha = 3;

char clientUri[] = "api.pushingbox.com";
char deviceId[] = "vC5BDEC64025A849";

boolean pressionada = false;
boolean lastConnected = false;

EthernetClient client;

void setup()
{
  Serial.begin(9600);

  pinMode(pinCampainha, INPUT);

  while (Serial.available() <= 0)
  {
    Serial.println("Conectando...");
    delay(3000);
  }

  if (Ethernet.begin(mac) == 0)
  {
    Serial.println("Falha ao conectar.");
    while (true);
  }
  
  else
  {
    Serial.println("Ethernet ready");
    Serial.print("My IP address: ");
    Serial.println(Ethernet.localIP());
  }
  
  delay(1000); //delay para inicializar o shield
}

void loop()
{
  if (digitalRead(pinCampainha) == HIGH && pressionada == false)
  {
    Serial.println("Campainha pressionada!");

    pressionada = true;

    enviarNotificacao(deviceId);
  }

  if (digitalRead(pinCampainha) == LOW && pressionada == true)
    pressionada = false; //campainha deixou de ser pressionada

  if (client.available())
  {
    char c = client.read();
    Serial.print(c);
  }

  if (!client.connected() && lastConnected)
  {
    Serial.println("\n disconnecting.");
    client.stop();
  }

  lastConnected = client.connected();
}

void enviarNotificacao(char devid[]) {

  client.stop();

  Serial.println("connecting...");

  if (client.connect(clientUri, 80))
  {
    Serial.println("connected");
    Serial.println("sendind request");

    client.print("GET /pushingbox?devid=");
    client.print(devid);
    client.println(" HTTP/1.1");
    client.print("Host: ");
    client.println(clientUri);
    client.println("User-Agent: Arduino");
    client.println();
  }

  else
    Serial.println("connection failed");
}
