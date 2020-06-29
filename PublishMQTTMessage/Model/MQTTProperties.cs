using System;

namespace PublishMQTTMessage.Model
{
    class MQTTProperties
    {
        public static string CLIENT_ID { get { return Guid.NewGuid().ToString(); } }
        public static string HOST_NAME { get { return "localhost"; } }
        public static string TOPIC_NAME { get { return "campainha/"; } }
        public static string MESSAGE_STRING { get { return "CAMPAINHA PRESSIONADA"; } }
    }
}