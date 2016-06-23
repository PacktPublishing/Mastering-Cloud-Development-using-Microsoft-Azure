using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalyticsShop.Models
{
    public class EventDto
    {
        public string UserId { get; set; }
        public EventType Type { get; set; }
        public DateTimeOffset LocalTime { get; set; }
        public int ProductId { get; set; }
        public int ItemsInBasket { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
