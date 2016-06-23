using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Hosting;
using System.ServiceModel.Web;

namespace CM.Geo.SB
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //Start the Service Bus engine            
            var sh = new WebServiceHost(typeof(ServiceBusRelayHandlerAdapter));
            sh.AddServiceEndpoint(typeof(IHandlerAdapter),
                new WebHttpRelayBinding(EndToEndWebHttpSecurityMode.Transport,
                RelayClientAuthenticationType.None),
                ServiceBusEnvironment
                .CreateServiceUri("https", "[namespace]", "apis"))
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider
                    .CreateSharedAccessSignatureTokenProvider(
                        "[keyName]", "[keyValue]")
                });

            sh.Open();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}