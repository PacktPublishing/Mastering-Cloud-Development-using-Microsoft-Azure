using AnalyticsShop.Models;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(AnalyticsShop.Startup))]
namespace AnalyticsShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // argumens to be saved in config file
            ServiceBusTopic.Subscribe("events", "WebApplication", message =>
            {
                var dto = JsonConvert.DeserializeObject<EventDto>(
                    message.GetBody<string>());
                NotificationHub.Notify(dto);
            });

            // argumens to be saved in config file
            ServiceBusTopic.Subscribe("feedback", "WebApplication", message =>
            {
                var dto = JsonConvert.DeserializeObject<FeedbackDto>(
                    message.GetBody<string>());
                if (dto.Message == $"UserId {dto.UserId} has failed login more than 3 times in the last 10 minutes")
                {
                    var context = new ApplicationDbContext();
                    var user = context.Users.Single(xx => xx.UserName == dto.UserId);
                    user.LockoutEnabled = true;
                    context.SaveChanges();
                }
            });

            app.MapSignalR();
        }
    }
}
