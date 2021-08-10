using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GCP_SubPub_ClientConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GCP Sub/Pub Subscriber Client Test...");
            PullMessagesWithCustomAttributesAsync("peak-system-321913", "Consumer_Update", false).GetAwaiter().GetResult();
           // PullMessagesAsync("peak-system-321913", "Consumer_Update", true).GetAwaiter().GetResult();
            Console.WriteLine("Press INTRO to continue...");
            Console.ReadLine();
        }

        public static async Task<int> PullMessagesAsync(string projectId, string subscriptionId, bool acknowledge)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            // SubscriberClient runs your message handle function on multiple
            // threads to maximize throughput.
            int messageCount = 0;
            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                Console.WriteLine($"Message {message.MessageId}: {text}");
                Interlocked.Increment(ref messageCount);
                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
            });
            // Run for 5 seconds.
            await Task.Delay(5000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;
            return messageCount;
        }

        public static async Task<List<PubsubMessage>> PullMessagesWithCustomAttributesAsync(string projectId, string subscriptionId, bool acknowledge)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);

            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);
            var messages = new List<PubsubMessage>();
            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                messages.Add(message);
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                Console.WriteLine($"Message {message.MessageId}: {text}");
                if (message.Attributes != null)
                {
                    foreach (var attribute in message.Attributes)
                    {
                        Console.WriteLine($"{attribute.Key} = {attribute.Value}");
                    }
                }
                return Task.FromResult(acknowledge ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack);
            });
            // Run for 7 seconds.
            await Task.Delay(7000);
            await subscriber.StopAsync(CancellationToken.None);
            // Lets make sure that the start task finished successfully after the call to stop.
            await startTask;
            return messages;
        }
    }
}
