using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PublishClient
{
    class Program
    {

        private const string MyTopic = "Topic1";
        private const string MQTTServerUrl = "localhost";  
        private const int MQTTPort = 1883;
        static async Task Main(string[] args)
        {
            MqttFactory factory = new MqttFactory();           
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithTcpServer(MQTTServerUrl, MQTTPort).WithCredentials("mqtt", "matkhau123").Build();


            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(options, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            mqttClient.UseConnectedHandler( e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                Console.WriteLine("Publish to {0}", MyTopic);
                Console.WriteLine();
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);
            string mess;

            // Publishing messages  
            while (true)
            {
                mess = Console.ReadLine();
                var message = new MqttApplicationMessageBuilder().WithTopic(MyTopic).WithPayload(mess).Build();
                await mqttClient.PublishAsync(message);
                Console.WriteLine($"### message: {Encoding.UTF8.GetString(message.Payload)}");
            }


        }
    }
}
