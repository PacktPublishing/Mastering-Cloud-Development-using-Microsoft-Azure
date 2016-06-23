using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AnalyticsShop.Controllers
{
    public abstract class ServiceBusApiController: ApiController
    {
        protected async Task SendEventHubAsync<T>(string name, T target)
        {
            var eventHub = EventHubClient.CreateFromConnectionString(
                ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString, name);
            var json = JsonConvert.SerializeObject(target);
            var bytes = Encoding.UTF8.GetBytes(json);
            var data = new EventData(bytes);
            await eventHub.SendAsync(data);
        }

        protected async Task SendQueueAsync<T>(string name, T target)
        {
            var queue = QueueClient.CreateFromConnectionString(
                ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString, name);
            var json = JsonConvert.SerializeObject(target);
            var message = new BrokeredMessage(json);
            await queue.SendAsync(message);
        }
    }
}
