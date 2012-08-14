using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WorkerRole
{
    public class ThreadWorker
    {
        internal void RunInternal()
        {
            try
            {
                Run();
            }
            catch (SystemException)
            {
                throw;
            }
            catch (Exception)
            {
                Trace.WriteLine("Unhandled exception caught by RunInternal", "[ThreadWorker]");
            }
        }

        public virtual void Run()
        {
        }

        public virtual void OnStop()
        {
        }
    }
}
