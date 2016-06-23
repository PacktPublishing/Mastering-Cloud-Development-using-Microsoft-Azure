using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AnalyticsShop.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnalyticsShop.Controllers
{
    public class CommandsController : ServiceBusApiController
    {
        [HttpPost]
        public async Task PostAsync(string id)
        {
            var command = JsonConvert.DeserializeObject<JObject>(await Request.Content.ReadAsStringAsync());
            switch (id) // id is the command to be sent
            {
                case "addtobasket":
                    await SendQueueAsync("basket", new
                    {
                        CommandType = "AddToBasket",
                        ProductId = command["productId"].Value<string>(),
                        Quantity = command["quantity"].Value<int>(),
                        UserId = User.Identity.Name
                    });
                    break;
                default:
                    throw new NotSupportedException("Command not supported");
            }
        }
    }
}