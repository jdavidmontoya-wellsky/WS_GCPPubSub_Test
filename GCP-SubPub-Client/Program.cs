using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GCP_SubPub_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json"));

            Console.WriteLine("GCP Sub/Pub Test...");
            bool flagContinue = true;
            do
            {
                Console.WriteLine("Press INTRO to send (or type e to exit):");
                string textSend = Console.ReadLine();
                if (textSend.ToUpper().Equals("E"))
                {
                    flagContinue = false;
                }
                JObject data = JObject.Parse(File.ReadAllText(@"C:\Users\jesusdavid.montoya\Documents\WellSky\UserStory7292\GCP-PubSub\test.json"));
                textSend = data.ToString(Newtonsoft.Json.Formatting.None);
                PublishMessageWithCustomAttributesAsync("peak-system-321913", "Consumer_Update", textSend).GetAwaiter().GetResult();
            } while (flagContinue);
            Console.WriteLine("Exit application...");
        }

        public static async Task PublishMessageWithCustomAttributesAsync(string projectId, string topicId, string messageText)
        {
            TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            var pubsubMessage = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(messageText),
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
            {
                { "year", "2020" },
                { "author", "unknown" }
            }
            };
            string message = await publisher.PublishAsync(pubsubMessage);
            Console.WriteLine($"Published message {message}");
        }
    }
}
