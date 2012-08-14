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
    class LetterQueueWorker : ThreadWorker
    {
        bool RepeatMode { get; set; }

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
                CloudQueue letterQueue = queueClient.GetQueueReference("letters");

                if (letterQueue.Exists())
                {
                    CloudQueueMessage message = letterQueue.GetMessage();

                    if (message != null)
                    {
                        RepeatMode = false;

                        if (message.AsString.Equals("e"))
                        {
                            // throw an exception
                            letterQueue.DeleteMessage(message);
                            Trace.WriteLine("Exception requested", "[Letter Queue]");
                            throw new Exception("Received an exception request");
                        }
                        else if (message.AsString.Equals("x"))
                        {
                            // repeat mode
                            RepeatMode = true;
                            letterQueue.DeleteMessage(message);
                        }
                        else
                        {
                            Trace.WriteLine(String.Format("Found message.  Moving {0} to the output queue", message.AsString), "[Letter Queue]");

                            // move the message to the output queue
                            CloudQueue outputQueue = queueClient.GetQueueReference("output");
                            outputQueue.CreateIfNotExist();
                            outputQueue.AddMessage(message);
                            letterQueue.DeleteMessage(message);
                        }
                    }
                }

                if (RepeatMode)
                {
                    CloudQueueMessage repeatMsg = new CloudQueueMessage("repeat");
                    CloudQueue outputQueue = queueClient.GetQueueReference("output");
                    outputQueue.AddMessage(repeatMsg);
                }
                
                Thread.Sleep(2000);
            }
        }
    }
}
