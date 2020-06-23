using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartDoorbell
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SendNotification();
        }

        private static async Task<HttpResponseMessage> SendNotification()
        {
            var client = new HttpClient();

            var pushNotificationUri = "http://api.pushingbox.com/pushingbox?devid={0}";

            var devId = "vC5BDEC64025A849";

            var uri = string.Format(pushNotificationUri, devId);

            var result = await client.GetAsync(uri);

            return result;
        }
    }
}
