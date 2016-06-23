using CM.Geo.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CM.Geo.Api.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage>
            SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var logEntry = string.Format("Call to: {0} from {1}",
                request.RequestUri.ToString(),
                request.Properties.ContainsKey("USER") ?
                request.Properties["USER"] : "Unknown");
            GeoLogger.Log(logEntry);
            return base.SendAsync(request, cancellationToken);
        }
    }
}