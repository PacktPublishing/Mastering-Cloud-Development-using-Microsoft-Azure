using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderGenerator
{
    public class OrderController
    {

        public void ReceiveOrder(Order item)
        {
            //Comment ex-Saving logic
            //WriteOrderToDB(item);
            //Introduce new Queue-based logic
            WriteOrderInQueue(item);
        }

        private void WriteOrderInQueue(Order item)
        {
            var connStr = "[connStr]";
            var queueClient = CloudStorageAccount
                .Parse(connStr).CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("orders-queue");
            queue.CreateIfNotExists();
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(item)));
        }

       
    }
}
