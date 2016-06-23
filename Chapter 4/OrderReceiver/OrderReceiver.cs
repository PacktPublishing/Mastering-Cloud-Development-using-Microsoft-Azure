using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using OrderGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OR
{
    public class OrderReceiver
    {
       

        public void Run(CancellationToken token)
        {
            var connStr = "[Storage Connection String]";
            var queueClient = CloudStorageAccount
                .Parse(connStr).CreateCloudQueueClient();
            var queue = queueClient
                .GetQueueReference("orders-queue");
            queue.CreateIfNotExists();

            var localQueue = new Queue<CloudQueueMessage>();
            var lastSecond = DateTime.Now.Second;
            var batchSize = 250;
            while (!token.IsCancellationRequested)
            {
                var msgs = queue.GetMessages(
                    messageCount: 32,
                    visibilityTimeout:
                        TimeSpan.FromMinutes(1)).ToList();
                msgs.ForEach(p => localQueue.Enqueue(p));
                var currentSecond = DateTime.Now.Second;
                if (currentSecond != lastSecond)
                {
                    for (int i = 0; i < batchSize
                        && i < localQueue.Count; i++)
                    {
                        var msg = localQueue.Dequeue();
                        var payload = JsonConvert
                            .DeserializeObject<Order>(msg.AsString);
                        WriteOrderToDB(payload);
                        queue.DeleteMessageAsync(msg);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void WriteOrderToDB(Order payload)
        {
            //throw new NotImplementedException();
        }
    }
}
