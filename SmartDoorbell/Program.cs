using System;
using System.IO.Ports;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartDoorbell
{
    class Program
    {
        private static SerialPort _porta;
        private static string OUTPUT;
        private static int BAUD_RATE = 9600;
        private static string INDENT_3_SPACES = "   ";
        private static string INDENT_6_SPACES = INDENT_3_SPACES + INDENT_3_SPACES;
        private static string PORT_NAME = "COM3";
        private static string DEVICE_ID = "vC5BDEC64025A849";
        private static string CAMPAINHA_PRESSIONADA = "1";
        private static string URI = "http://api.pushingbox.com";
        private static string PUSH_NOTIFICATION_ENDPOINT = "/pushingbox?devid={0}";

        static async Task Main(string[] args)
        {
            StartPort();

            while (true)
                await Run();
        }

        private static void StartPort()
        {
            _porta = new SerialPort();
            _porta.PortName = PORT_NAME;
            _porta.BaudRate = BAUD_RATE;
            _porta.Open();
        }

        private static async Task Run()
        {
            if (ReadCampainha() == CAMPAINHA_PRESSIONADA)
            {
                Console.WriteLine($"{INDENT_3_SPACES}Iniciando requisicao para enviar notificacao push... \n");
                await SendNotification();
                Console.WriteLine($"{INDENT_3_SPACES}Requisicao finalizada... \n");
            }

            Console.WriteLine("Leitura finalizada. \n");

            for (int i = 0; i < 3; i++)
                Console.WriteLine(".");
        }

        private static string ReadCampainha()
        {
            Console.WriteLine($"\nIniciando a leitura da porta {PORT_NAME}. \n");

            OUTPUT = _porta.ReadLine();

            FormatarRetornoDaPorta();

            if (OUTPUT == CAMPAINHA_PRESSIONADA)
                Console.WriteLine($"{INDENT_3_SPACES}A campainha foi pressionada! \n");

            return OUTPUT;
        }

        private static void FormatarRetornoDaPorta()
        {
            OUTPUT = Regex.Replace(OUTPUT, "(\\r)*", "");
        }

        private static async Task SendNotification()
        {
            LogRequisicaoHttp("Instanciando HTTPClient...", ConsoleColor.Yellow);

            var client = new HttpClient();

            var pushNotificationUri = URI + PUSH_NOTIFICATION_ENDPOINT;

            var uri = string.Format(pushNotificationUri, DEVICE_ID);

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
                Console.WriteLine($"{INDENT_6_SPACES}{textParameter} \n");

            else
                Console.WriteLine($"{INDENT_6_SPACES}{textParameter}");

            Console.ResetColor();
        }
    }
}
