using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using QueueClient = Microsoft.Azure.ServiceBus.QueueClient;
class Program
{
    static void Main(string[] args)
    {
        string queueName = "queue1";
        string connection = "Endpoint=sb://ngtesb1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3s4PaCNkfQeuQ/YkxO2Z69riVAsdbBhZNK1Eo/2+Hrw=;TransportType=Amqp";
        MessagingFactory factory = MessagingFactory.CreateFromConnectionString(connection);
        var queue = factory.CreateQueueClient(queueName);
        while (true)
        {
            BrokeredMessage message = queue.Receive();
            if (message != null)
            {
                try
                {
                    Console.WriteLine("MessageId	{0}", message.MessageId);
                    Console.WriteLine("Delivery	{0}", message.DeliveryCount);
                    Console.WriteLine("Size	{0}", message.Size);
                    Console.WriteLine(message.GetBody<string>());
                    message.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    message.Abandon();
                }
            }
        }
    }
}