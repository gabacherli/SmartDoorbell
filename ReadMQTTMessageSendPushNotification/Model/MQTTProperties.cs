using System;

namespace ReadMQTTMessageSendPushNotification.Model
{
    class MQTTProperties
    {
        public static string CLIENT_ID { get { return Guid.NewGuid().ToString(); } }
        public static string HOST_NAME { get { return "localhost"; } }
        public static string TOPIC_NAME { get { return "campainha/"; } }
        public static int PORT { get { return 1883; } }
    }
}
