using System;
using System.Collections.Generic;
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
