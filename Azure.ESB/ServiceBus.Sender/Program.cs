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
        string message = "queue	message	over amqp";
        BrokeredMessage bm = new BrokeredMessage(message);
        queue.Send(bm);
    }
}