using ReadMQTTMessageSendPushNotification.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ReadMessageSendPushNotification
{
    class Program
    {
        private static MqttClient _mqttClient;
        private static readonly List<string> MESSAGES = new List<string>();

        static void Main(string[] args)
        {
            LogStartupMessage();
            StartMqttClient();

            while (true)
                SendPushNotification();
        }

        private static void LogStartupMessage()
        {
            Console.WriteLine($"########## CAMPAINHA INTELIGENTE - MQTT LISTENER ##########\n");
        }

        private static void StartMqttClient()
        {
            LogMQTT("Instanciando client MQTT...", ConsoleColor.Yellow);

            _mqttClient = new MqttClient(MQTTProperties.HOST_NAME);

            LogMQTT($"{Indent.ESCAPE}ClientID = {MQTTProperties.CLIENT_ID}", ConsoleColor.Yellow);
            LogMQTT($"{Indent.ESCAPE}Hostname = {MQTTProperties.HOST_NAME}", ConsoleColor.Yellow);

            while (!_mqttClient.IsConnected)
            {
                try
                {
                    LogMQTTIniciandoConexao($"{Indent.ESCAPE}Iniciando conexao...", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    LogMQTT($"Erro ao estabelecer conexão!!!", ConsoleColor.DarkRed);
                    LogMQTT($"ErrorMessage: {ex.Message}", ConsoleColor.DarkRed);
                    LogMQTT($"InnerException: {ex.InnerException.Message} \n", ConsoleColor.DarkRed);
                };
            }
        }

        private static void LogMQTT(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"{textParameter}");
            Console.ResetColor();
        }

        private static void LogMQTTIniciandoConexao(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"{textParameter}");
            Console.ResetColor();

            for (int i = 0; i < 3; i++)
            {
                LogMQTT($"{Indent.ESCAPE}...", ConsoleColor.Yellow);
                Task.Delay(500).Wait();
            }

            _mqttClient.Connect(MQTTProperties.CLIENT_ID);

            if (_mqttClient.IsConnected)
            {
                _mqttClient.Subscribe(new String[] { MQTTProperties.TOPIC_NAME }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                LogMQTT("Conexao bem sucedida!", ConsoleColor.DarkGreen);
                LogMQTT($"Subscriber: {MQTTProperties.TOPIC_NAME}", ConsoleColor.DarkGreen);
            }
        }

        private static void SendPushNotification()
        {
            _mqttClient.MqttMsgPublishReceived += ReadMessage;
        }

        private static void ReadMessage(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);

            if (!MESSAGES.Contains(message))
            {
                MESSAGES.Add(message);
                Console.WriteLine($"\nNOVA MENSAGEM: {message}");

                Send3Notifications().Wait();
            }

            else
                return;
        }

        private static async Task Send3Notifications()
        {
            for (int i = 0; i < 1; i++)
            {
                await SendNotificationAsync();
                await Task.Delay(2500);
            }
        }

        private static async Task SendNotificationAsync()
        {
            LogRequisicaoHttp("Instanciando HTTP Client...", ConsoleColor.Yellow);

            var client = new HttpClient();

            var pushNotificationUri = PushingBox.URI + PushingBox.PUSH_NOTIFICATION_ENDPOINT;

            var uri = string.Format(pushNotificationUri, PushingBox.DEVICE_ID);

            LogRequisicaoHttp($"Enviando requisicao para {uri}...", ConsoleColor.Yellow);

            var result = await client.GetAsync(uri);

            if (result.IsSuccessStatusCode)
                LogRequisicaoHttp($"NOTIFICACAO ENVIADA!", ConsoleColor.DarkGreen);

            else
                LogRequisicaoHttp($"ERRO: {result.StatusCode}!", ConsoleColor.DarkRed);
        }

        private static void LogRequisicaoHttp(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;

            if (cor != ConsoleColor.Yellow)
                Console.WriteLine($"{Indent.ESCAPEx2}{textParameter} \n");

            else
                Console.WriteLine($"{Indent.ESCAPEx2}{textParameter}");

            Console.ResetColor();
        }
    }
}
