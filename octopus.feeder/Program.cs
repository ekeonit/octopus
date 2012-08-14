using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace octopus.feeder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Retrieve storage account from connection-string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Properties.Settings.Default.StorageConnectionString);

            // Create the queue clients
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Create the queues
            CloudQueue letterQueue = queueClient.GetQueueReference("letters");
            letterQueue.CreateIfNotExist();

            CloudQueue numberQueue = queueClient.GetQueueReference("numbers");
            numberQueue.CreateIfNotExist();

            CloudQueue punctuationQueue = queueClient.GetQueueReference("puncs");
            punctuationQueue.CreateIfNotExist();

            while (true)
            {
                Console.WriteLine("Enter a letter, number of punctuation ('q' or 'Q' to quit)");
                Console.WriteLine("Enter 'e', '1', '?' to generate exception in the respective worker threads");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();

                switch (key.KeyChar)
                {
                    case 'q':
                    case 'Q':
                        return;
                }

                string input = new string(key.KeyChar, 1);

                if (Regex.IsMatch(input, "[a-zA-Z]"))
                {
                    // letter
                    letterQueue.AddMessage(new CloudQueueMessage(input));
                    Console.WriteLine("Added {0} to the letter queue", input);
                }
                else if (Regex.IsMatch(input, "[0-9]"))
                {
                    // number
                    numberQueue.AddMessage(new CloudQueueMessage(input));
                    Console.WriteLine("Added {0} to the number queue", input);
                }
                else if (Regex.IsMatch(input, "[^\\w\\s.]"))
                {
                    // punctuation
                    punctuationQueue.AddMessage(new CloudQueueMessage(input));
                    Console.WriteLine("Added {0} to the punctuation queue", input);
                }
            }
        }
    }
}
