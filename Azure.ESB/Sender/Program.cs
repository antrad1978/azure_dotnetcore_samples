using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;

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

    private static async Task RunAsync()
    {
        Console.WriteLine("Enter lines of text to send to the server with ENTER");
        // Create a new hybrid connection client
        var tokenProvider =
            TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
        var client = new HybridConnectionClient(new
            Uri(String.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);
        // Initiate the connection
        var relayConnection = await client.CreateConnectionAsync();
        var reads = Task.Run(async () =>
        {
            var reader = new StreamReader(relayConnection);
            var writer = Console.Out;
            do
            {
                // Read a full line of UTF-8 text up to newline
                string line = await reader.ReadLineAsync();
                // if the string is empty or null, we are done.
                if (String.IsNullOrEmpty(line))
                    break;
                // Write to the console
                await writer.WriteLineAsync(line);
            } while (true);
        });
        // Read from the console and write to the hybrid connection
        var writes = Task.Run(async () =>
        {
            var reader = Console.In;
            var writer = new StreamWriter(relayConnection)
            {
                AutoFlush = true
            };
            do
            {
                // Read a line form the console
                string line = await reader.ReadLineAsync();
                await writer.WriteLineAsync(line);
                if (String.IsNullOrEmpty(line))
                    break;
            } while (true);
        });
        await Task.WhenAll(reads, writes);
        await
            relayConnection.CloseAsync(CancellationToken.None);
    }
}
