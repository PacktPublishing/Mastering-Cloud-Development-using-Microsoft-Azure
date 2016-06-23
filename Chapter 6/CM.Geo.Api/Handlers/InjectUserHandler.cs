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

    public class InjectUserHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage>
                SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("x-cm-identity"))
            {
                GeoUser user = UserManagement.FromIdentity(
                    request.Headers.GetValues("x-cm-identity").FirstOrDefault());
                request.Properties["USER"] = user;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}