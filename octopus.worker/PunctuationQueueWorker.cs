using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace WorkerRole
{
    class PunctuationQueueWorker : ThreadWorker
    {
        public override void Run()
        {
            // Retrieve storage account from connection-string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue clients
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            while (true)
            {
                // Retrieve a reference to a queue
                CloudQueue punctuationQueue = queueClient.GetQueueReference("puncs");

                if (punctuationQueue.Exists())
                {
                    CloudQueueMessage message = punctuationQueue.GetMessage();

                    if (message != null)
                    {
                        Trace.WriteLine(String.Format("Found message.  Moving {0} to the output queue", message.AsString), "[Punctuation Queue]");

                        // move the message to the output queue
                        CloudQueue outputQueue = queueClient.GetQueueReference("output");
                        outputQueue.CreateIfNotExist();
                        outputQueue.AddMessage(message);

                        punctuationQueue.DeleteMessage(message);
                    }
                }

                Thread.Sleep(2000);
            }
        }
    }
}
