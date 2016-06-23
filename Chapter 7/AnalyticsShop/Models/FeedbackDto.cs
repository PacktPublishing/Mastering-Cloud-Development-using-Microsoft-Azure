using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalyticsShop.Models
{
    public class FeedbackDto
    {
        public string UserId { get; set; }
        public FeedbackType Type { get; set; }
        public FeedbackSeverity Severity { get; set; }
        public DateTimeOffset LocalTime { get; set; }
        public string Message { get; set; }
    }
}