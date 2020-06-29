namespace ReadMQTTMessageSendPushNotification.Model
{
    class PushingBox
    {
        public static string DEVICE_ID { get { return "vC5BDEC64025A849"; } }
        public static string URI { get { return "http://api.pushingbox.com"; } }
        public static string PUSH_NOTIFICATION_ENDPOINT { get { return "/pushingbox?devid={0}"; } }
    }
}
