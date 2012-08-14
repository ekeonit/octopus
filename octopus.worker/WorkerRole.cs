using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        readonly List<Thread> _threads = new List<Thread>();
        readonly List<ThreadWorker> _workers = new List<ThreadWorker>();
        EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

        public override void Run()
        {
            Trace.WriteLine("Worker running ...");

            foreach (var worker in _workers)
            {
                _threads.Add(new Thread(worker.RunInternal));
            }

            foreach (var thread in _threads)
            {
                thread.Start();
            }

            while (!EventWaitHandle.WaitOne(500))
            {  
                for (var i = 0; i < _threads.Count; i++)
                {
                    if (_threads[i].IsAlive)
                    {
                        continue;
                    }

                    _threads[i] = new Thread(_workers[i].RunInternal);
                    _threads[i].Start();
                }

                EventWaitHandle.WaitOne(2000);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            DiagnosticMonitor.Start("StorageConnectionString");

            _workers.Add(new LetterQueueWorker());
            _workers.Add(new NumberQueueWorker());
            _workers.Add(new PunctuationQueueWorker());

            return base.OnStart();
        }
    }
}
