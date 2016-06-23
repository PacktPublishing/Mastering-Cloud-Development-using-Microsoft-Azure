using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using OR;

namespace DefaultWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource
                = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent
                = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("DefaultWorker is running");

            try
            {
                var receiver = new OrderReceiver();
                receiver.Run(cancellationTokenSource.Token);
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("DefaultWorker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("DefaultWorker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("DefaultWorker has stopped");
        }

    }
}
