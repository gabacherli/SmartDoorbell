namespace PublishMQTTMessage.Model
{
    class Port
    {
        public static string OUTPUT { get; set; }
        public static int BAUD_RATE { get { return 9600; } }
        public static string PORT_NAME { get { return "COM3"; } }
    }
}