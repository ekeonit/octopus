using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace octopus.eater
{
    class Program
    {
        static void Main(string[] args)
        {
            // Retrieve storage account from connection-string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Properties.Settings.Default.StorageConnectionString);

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("output");
            queue.CreateIfNotExist();

            Console.WriteLine("Octopus eater started ...");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey().KeyChar == 'q')
                    {
                        return;
                    }
                }

                CloudQueueMessage message = queue.GetMessage();

                if (message != null)
                {
                    Console.WriteLine("Octopus is eating the message {0} on the output queue", message.AsString);
                    queue.DeleteMessage(message);
                }

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
