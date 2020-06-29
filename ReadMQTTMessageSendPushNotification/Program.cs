using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;
using ReadMQTTMessageSendPushNotification.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadMessageSendPushNotification
{
    class Program
    {
        private static IMqttClient _client;
        private static MqttFactory _factory;
        private static IMqttClientOptions _options;

        private static readonly List<string> MESSAGES = new List<string>();

        static async Task Main(string[] args)
        {
            LogStartupMessage();
            StartMqttClient();
            await LogStartMqttConnection();

            while (true)
                await GetMessages();
        }

        private static void LogStartupMessage()
        {
            Console.WriteLine($"########## CAMPAINHA INTELIGENTE - MQTT LISTENER ##########\n");
        }

        public static void StartMqttClient()
        {
            var messageBuilder = new MqttClientOptionsBuilder()
                .WithClientId(MQTTProperties.CLIENT_ID)
                .WithTcpServer(MQTTProperties.HOST_NAME, MQTTProperties.PORT)
                .WithCleanSession();

            _options = messageBuilder.Build();

            _factory = new MqttFactory();

            LogMqtt("Instanciando client MQTT...", ConsoleColor.Yellow);
            LogMqtt($"{Indent.ESCAPE}ClientID = {MQTTProperties.CLIENT_ID}", ConsoleColor.Yellow);
            LogMqtt($"{Indent.ESCAPE}Hostname = {MQTTProperties.HOST_NAME}", ConsoleColor.Yellow);

            _client = _factory.CreateMqttClient();
        }

        public static async Task SubscribeAsync(string topic, int qos = 2)
        {
            await _client.SubscribeAsync(MQTTProperties.TOPIC_NAME, MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private static void LogMQTT(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"{textParameter}");
            Console.ResetColor();
        }

        private static async Task LogStartMqttConnection()
        {
            while (!_client.IsConnected)
            {
                try
                {
                    await _client.ConnectAsync(_options, CancellationToken.None);

                    await LogIniciandoConexaoAsync($"{Indent.ESCAPE}Iniciando conexao...", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    LogMqtt($"Erro ao estabelecer conexão!!!", ConsoleColor.DarkRed);
                    LogMqtt($"ErrorMessage: {ex.Message}", ConsoleColor.DarkRed);
                    LogMqtt($"InnerException: {ex.InnerException.Message} \n", ConsoleColor.DarkRed);
                };
            }
        }

        private static void LogMqtt(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"{textParameter}");
            Console.ResetColor();
        }

        private static async Task LogIniciandoConexaoAsync(string textParameter, ConsoleColor cor)
        {
            Console.ForegroundColor = cor;
            Console.WriteLine($"{textParameter}");
            Console.ResetColor();

            for (int i = 0; i < 3; i++)
            {
                LogMqtt($"{Indent.ESCAPE}...", ConsoleColor.Yellow);
                Task.Delay(500).Wait();
            }

            if (_client.IsConnected)
            {
                await SubscribeAsync(MQTTProperties.TOPIC_NAME);

                LogMqtt("Conexao bem sucedida!", ConsoleColor.DarkGreen);
                LogMqtt($"Subscriber: {MQTTProperties.TOPIC_NAME}", ConsoleColor.DarkGreen);
            }
        }

        private static async Task GetMessages()
        {
            if (!_client.IsConnected)
                await TryToReconnectAsync();

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    Console.WriteLine($"\nNOVA MENSAGEM: {message}");
                    Send3Notifications().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });
        }

        static async Task TryToReconnectAsync()
        {
            try
            {
                LogMQTT($"Reconectando...", ConsoleColor.Yellow);

                await _client.ConnectAsync(_options, CancellationToken.None);

                if (_client.IsConnected)
                    LogMQTT("Sucesso!", ConsoleColor.DarkGreen);
            }
            catch (Exception ex)
            {
                LogMQTT($"Erro ao estabelecer conexão!!!", ConsoleColor.DarkRed);
                LogMQTT($"ErrorMessage: {ex.Message}", ConsoleColor.DarkRed);
                LogMQTT($"InnerException: {ex.InnerException?.Message} \n", ConsoleColor.DarkRed);
            };
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
