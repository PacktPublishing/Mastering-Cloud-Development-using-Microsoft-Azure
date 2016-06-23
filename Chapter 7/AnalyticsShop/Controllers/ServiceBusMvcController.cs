using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace AnalyticsShop.Controllers
{
    public abstract class ServiceBusMvcController: Controller
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
