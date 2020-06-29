# Smart Doorbell

Ao tocar uma campainha, o que geraria normalmente apenas um toque sonoro é convertido em notificações push para seus dispositivos eletrônicos.

## Configuração

Servidor MQTT utilizado para fazer a comunicação entre o publisher e o subscriber:

```bash
Mosquitto - https://mosquitto.org/download/
```

Pacotes Nuget utilizados nas aplicações escritas em .NET Framework para publicar e ler mensagens na fila:

```bash
M2Mqtt - dotnet add package M2Mqtt --version 4.3.0
MQTTnet.Extensions.ManagedClient - dotnet add package MQTTnet.Extensions.ManagedClient --version 3.0.11
```

## Uso

1. Fazer um clone do projeto para sua máquina.

git clone https://github.com/gabacherli/SmartDoorbell.git

2. Abaixo, há um código na extensão .INO. Para nosso desenvolvimento, utilizamos um Arduino Duemilanove com processador ATmega328P.
O pino que utilizamos para input foi o pino 3. Caso queira utilizar outro pino como input, altere o valor da variável pinCampainha. Este código deve ser sobreescrito em seu dispositivo IoT.

```bash
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
```

3. Vá até a pasta de destino da instalação do MQTT Server Mosquitto e execute o comando abaixo para iniciar uma instância de conexão com o servidor:
```
C:\Program Files\mosquitto>mosquitto.exe
```

4. Com um compilador da linguagem .NET de sua preferência, execute simultaneamente os projetos da solução SmartDoorbell.sln ou um após o outro.
```bash
ATENÇÃO - Caso a conexão com o servidor Mosquitto ainda não esteja instanciada, o seguinte erro irá ser exibido:

Erro ao estabelecer conexao!!!
ErrorMessage: Exception connecting to the broker
InnerException: No connection could be made because the target machine actively refused it [::1]:1883
```

5. Envie estímulos para o pino de leitura que foi configurado e veja as mensagens sendo publicadas e processadas.

## Contribuição
Este é um repositório público. Para sugestões de mudança ou dúvidas de implementação, entre em contato por e-mail em

```bash
abacherli@live.com
ou
cristianbroly@gmail.com
```

Visite para mais informações 
```bash
http://mqtt.org - sobre o protocolo de mensageria MQTT;
https://www.pushingbox.com/api.php - sobre a API Pushing Box (para envio de notificações push);
https://m2mqtt.wordpress.com/ e https://github.com/chkr1011/MQTTnet/wiki - sobre os pacotes Nuget;
```
