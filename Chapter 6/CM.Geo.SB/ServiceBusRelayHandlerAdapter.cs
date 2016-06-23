using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace CM.Geo.SB
{
    [ServiceContract(Namespace = "urn:CloudMakers.XYZ.APIs")]
    public interface IHandlerAdapter
    {
        [OperationContract, WebGet(UriTemplate = "/{*route}")]
        Stream Get(string route);

        [OperationContract, WebInvoke(Method = "POST",
                UriTemplate = "/{*route}")]
        Stream Post(Stream data, string route);
    }

    public interface IServiceBusRelayHandlerAdapterChannel : IHandlerAdapter, IClientChannel { }

    public class ServiceBusRelayHandlerAdapter : IHandlerAdapter
    {
        public Stream Get(string route)
        {
            return new MemoryStream();
        }

        public Stream Post(Stream data, string route)
        {
            return new MemoryStream();
        }
    }




}