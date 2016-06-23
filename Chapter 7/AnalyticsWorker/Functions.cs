using AnalyticsWorker.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsWorker
{
    public class Functions
    {
        //public async static void HandleFeedback([ServiceBusTrigger("feedback")] string feedbackMessage)
        //{
        //    var client = new HttpClient();
        //    var feedbackUrl = ConfigurationManager.AppSettings["feedbackUrl"];
        //    var content = new StringContent(feedbackMessage);
        //    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/json");
        //    await client.PostAsync(feedbackUrl, content);
        //}

        public async static void HandleBasket(
            [ServiceBusTrigger("basket")] BrokeredMessage message
            , [ServiceBus("events")] ICollector<BrokeredMessage> events)
        {
            var command = JsonConvert.DeserializeObject<BasketCommand>(message.GetBody<string>());

            // connect to the CloudStorageAccount
            var storageConnectionString = 
                AmbientConnectionStringProvider.Instance
                .GetConnectionString(ConnectionStringNames.Storage);
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var basketContainer = blobClient.GetContainerReference("baskets");
            var basketBlob = basketContainer.GetBlockBlobReference(command.UserId);

            // deserialize object
            var json = string.Empty;
            Basket basket = null;
            if (basketBlob.Exists())
            {
                json = await basketBlob.DownloadTextAsync();
                basket = JsonConvert.DeserializeObject<Basket>(json);
            }
            else
            {
                basket = Activator.CreateInstance<Basket>();
                basket.UserId = command.UserId;
            }

            // execute command
            switch (command.Type)
            {
                case CommandType.AddToBasket:
                    var unitPrice = 1m; // lookup ProductId on a PriceList service
                    basket.AddProduct(command.ProductId, command.Quantity, unitPrice);
                    break;
                default:
                    throw new NotSupportedException("Command not supported");
            }

            // serialize back the object
            json = JsonConvert.SerializeObject(basket);
            await basketBlob.UploadTextAsync(json);

            // send
            events.Add(new BrokeredMessage(
                JsonConvert.SerializeObject(new {
                    UserId = command.UserId,
                    LocalTime = DateTime.Now,
                    Type = EventType.ProductAddedToBasket,
                    ProductId = command.ProductId,
                    ItemsInBasket = basket.Products.Count(),
                    TotalPrice = basket.TotalPrice
                })
            ));
        }
    }
}
