using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace AnalyticsShop
{
    using System;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Models;

    public class NotificationHub : Hub
    {
        public static IHubContext Default
        {
            get
            {
                return
                    Microsoft.AspNet.SignalR
                    .GlobalHost
                    .ConnectionManager
                    .GetHubContext<NotificationHub>();
            }
        }

        private static SignalRConnectionMapping<string> __connections = new SignalRConnectionMapping<string>();

        private static void Broadcast(string userId, Action<dynamic> action)
        {
            foreach (var connectionId in __connections.GetConnections(userId))
            {
                action(Default.Clients.Client(connectionId));
            }
        }

        public static void Notify(EventDto dto)
        {
            foreach (var connectionId in __connections.GetConnections(dto.UserId))
            {
                dynamic client = Default.Clients.Client(connectionId);
                client.handle_event(dto);
            }
        }

        public static void Notify(FeedbackDto dto)  
        {
            Broadcast(dto.UserId, _ => { _.handle_feedback(dto); });
        }

        [HubMethodName("RegisterUser")]
        public void RegisterUser(string userId)
        {
            __connections.Add(userId, Context.ConnectionId);
        }

        [HubMethodName("UnregisterUser")]
        public void UnregisterUser(string userId)
        {
            __connections.Remove(userId, Context.ConnectionId);
        }
    }
}