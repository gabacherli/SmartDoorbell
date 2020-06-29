using PublishMQTTMessage.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace PublishMQTTMessage
{
    class Program
    {
        private static SerialPort _porta = new SerialPort();
        private static MqttClient _mqttClient;

        private static readonly string CAMPAINHA_PRESSIONADA = "1";
        private static byte[] MESSAGE_TO_BYTES;

        static void Main(string[] args)
        {
            LogStartupMessage();
            StartPort();
            StartMqttClient();

            while (true)
                Run();
        }

        private static void LogStartupMessage()
        {
            Console.WriteLine($"########## CAMPAINHA INTELIGENTE - MQTT PUBLISHER ##########\n");
        }

        private static void StartPort()
        {
            _porta.PortName = Port.PORT_NAME;
            _porta.BaudRate = Port.BAUD_RATE;
            _porta.Open();
        }

        private static void StartMqttClient()
        {
            LogMQTT("Instanciando client MQTT...", ConsoleColor.Yellow);

            _mqttClient = new MqttClient(MQTTProperties.HOST_NAME);

            LogMQTT($"{Indent.ESCAPE}ClientID = {MQTTProperties.CLIENT_ID}", ConsoleColor.Yellow);
            LogMQTT($"{Indent.ESCAPE}Hostname = {MQTTProperties.HOST_NAME}", ConsoleColor.Yellow);
            LogMQTT($"{Indent.ESCAPE}Topic = {MQTTProperties.TOPIC_NAME}", ConsoleColor.Yellow);

            while (!_mqttClient.IsConnected)
            {
                try
                {
                    LogMQTTIniciandoConexao($"{Indent.ESCAPE}Iniciando conexao...", ConsoleColor.Yellow);

                    _mqttClient.Connect(MQTTProperties.CLIENT_ID);

                    if (_mqttClient.IsConnected)
                        LogMQTT("Conexao bem sucedida!", ConsoleColor.DarkGreen);
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
        }

        private static void Run()
        {
            if (ReadCampainha() == CAMPAINHA_PRESSIONADA)
                PublishMessage();

            Console.WriteLine("\nLeitura finalizada.\n");

            for (int i = 0; i < 3; i++)
                Console.WriteLine(".");
        }

        private static string ReadCampainha()
        {
            Console.WriteLine($"\nIniciando a leitura da porta {Port.PORT_NAME}. \n");

            Port.OUTPUT = _porta.ReadLine();

            FormatarRetornoDaPorta();

            if (Port.OUTPUT == CAMPAINHA_PRESSIONADA)
                Console.WriteLine($"{Indent.ESCAPE}A campainha foi pressionada! \n");

            return Port.OUTPUT;
        }

        private static void FormatarRetornoDaPorta()
        {
            Port.OUTPUT = Regex.Replace(Port.OUTPUT, "(\\r)*", "");
        }

        static void PublishMessage()
        {
            MessageToBytes(out string message);

            LogMQTT($"{Indent.ESCAPE}Topic: {MQTTProperties.TOPIC_NAME}", ConsoleColor.Yellow);
            LogMQTT($"{Indent.ESCAPE}Publicando mensagem '{message}'...", ConsoleColor.Yellow);

            _mqttClient.Publish(MQTTProperties.TOPIC_NAME, MESSAGE_TO_BYTES, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            LogMQTT($@"{Indent.ESCAPE}Mensagem publicada!", ConsoleColor.DarkGreen);
        }

        static void MessageToBytes(out string message)
        {
            message = $"{Guid.NewGuid()} - {MQTTProperties.MESSAGE_STRING}";

            MESSAGE_TO_BYTES = Encoding.UTF8.GetBytes(message);
        }
    }
}
