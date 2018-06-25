using System;
using Microsoft.ServiceBus.Messaging;
using QueueClient = Microsoft.Azure.ServiceBus.QueueClient;

namespace Azure.ServiceBus.QueueSample
{
    class Program
    {
        //private const string QueueConnectionString = "Endpoint=sb://ngtservicebus1.servicebus.windows.net/;SharedAccessKeyName=policy1;SharedAccessKey=HrvXLi0wiHnVh9nYLVFilOeGf5TXQ341ZNMr0pvt7Xk=;EntityPath=ngtqueue1";
        
        static void Main(string[] args)
        {
            /*var client = new QueueClient(QueueConnectionString);
            var message = new BrokeredMessage("Ciao");
            message.Label = "Sample";
            client.Send(message);*/
        }
    }
}