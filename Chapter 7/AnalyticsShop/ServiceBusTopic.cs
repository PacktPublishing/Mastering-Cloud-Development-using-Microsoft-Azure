using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AnalyticsShop
{
    public static class ServiceBusTopic
    {
        public static void Subscribe(string topic, string subscription, Action<BrokeredMessage> handle)
        {
            var client =
                SubscriptionClient.CreateFromConnectionString
                        (ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString
                            , topic, subscription);

            var options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            client.OnMessage((message) =>
            {
                try
                {
                    handle(message);
                    // Remove message from subscription.
                    message.Complete();
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    message.Abandon();
                }
            }, options);
        }
    }
}