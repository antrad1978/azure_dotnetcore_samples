using System;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.Azure.Relay;

namespace ConsoleApp1
{
    class Program
    {
        private const string RelayNamespace = "ngtrelay1.servicebus.windows.net";
        private const string ConnectionName = "hybridconnection1";
        private const string KeyName = "RootManageSharedAccessKey";
        private const string Key = "IcbImRDKU175/Hr0E6fs1CC64UcOd/CQ05QSE9XxqVw=";
    static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        private static async void ProcessMessagesOnConnection(HybridConnectionStream relayConnection,
            CancellationTokenSource cts)
        {
            Console.WriteLine("New	session");
            //	The	connection	is	a	fully	bidrectional	stream, enabling	the	Listener	
            //to	echo	the	text	from	the	Sender.									
            var reader = new StreamReader(relayConnection);
            var writer = new StreamWriter(relayConnection)
                {AutoFlush = true};
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    //	Read	a	line	of	input	until	a	newline	is encountered
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        await relayConnection.ShutdownAsync(cts.Token);
                        break;
                    }

                    Console.WriteLine(line);
                    //	Echo	the	line	back	to	the	client	
                    await writer.WriteLineAsync($"Echo:	{line}");

                }
                catch (IOException)
                {
                    Console.WriteLine("Client	closed	connection");
                    break;
                }
            }

            Console.WriteLine("End	session");
            //	Close	the	connection												
            await relayConnection.CloseAsync(cts.Token);
        }

        private static async Task RunAsync()
        {
            var cts = new CancellationTokenSource();
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
            var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);
            // Subscribe to the status events
            listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
            listener.Online += (o, e) => { Console.WriteLine("Online"); };
            // Establish the control channel to the Azure Relay service
            await listener.OpenAsync(cts.Token);
            Console.WriteLine("Server listening");
            // Providing callback for cancellation token that will close the listener.
            cts.Token.Register(() => listener.CloseAsync(CancellationToken.None));
            // Start a new thread that will continuously read the console.
            new Task(() =>
                Console.In.ReadLineAsync().ContinueWith((s) => { cts.Cancel(); })).Start();
            // Accept the next available, pending connection request.
            while (true)
            {
                var relayConnection = await listener.AcceptConnectionAsync();
                if (relayConnection == null)
                {
                    break;
                }

                ProcessMessagesOnConnection(relayConnection, cts);
            }

// Close the listener after we exit the processing loop
            await listener.CloseAsync(cts.Token);
        }
    }
}


