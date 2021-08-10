using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GCP_SubPub_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable(
        "GOOGLE_APPLICATION_CREDENTIALS",
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json"));

            bool flagContinue = true;
            do
            {
                Console.WriteLine("GCP Sub/Pub Test...\n Write message to send(or write exit):");
                string textSend = Console.ReadLine();
                if (string.IsNullOrEmpty(textSend) || textSend.ToUpper().Equals("EXIT"))
                    flagContinue = false;
                PublishMessageWithCustomAttributesAsync("peak-system-321913", "Consumer_Update", textSend).GetAwaiter().GetResult();
            } while (flagContinue);
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
