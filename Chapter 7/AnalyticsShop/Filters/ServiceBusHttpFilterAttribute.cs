using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Threading;
using System.Web.Http.Filters;

namespace AnalyticsShop.Filters
{
    public abstract class ServiceBusHttpFilterAttribute : ActionFilterAttribute
    {
        protected async Task SendEventHubAsync<T>(string name, T target)
        {
            var eventHub = EventHubClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString, name);
            var json = JsonConvert.SerializeObject(target);
            var bytes = Encoding.UTF8.GetBytes(json);
            var data = new EventData(bytes);
            await eventHub.SendAsync(data);
        }

        protected async Task SendQueueAsync<T>(string name, T target)
        {
            var queue = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString, name);
            var message = new BrokeredMessage(target);
            await queue.SendAsync(message);
        }
    }
}
