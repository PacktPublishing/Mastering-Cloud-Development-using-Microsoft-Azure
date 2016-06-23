using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnalyticsShop.Filters
{
    public class LogExceptionAttribute: ServiceBusHttpFilterAttribute, IExceptionFilter
    {
        public async void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            if (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            var category = "Generic";
            var stackTrace = exception.StackTrace;
            if (exception is NullReferenceException)
            {
                category = "Code";
            }

            await SendEventHubAsync("Exceptions", new
            {
                Category = category,
                LocalTime = DateTime.Now,
                Message = exception.Message,
                StackTrace = stackTrace
            });
        }
    }
}