using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var order = new Order()
            {
                ID=1,
                Amount=34.5,
                ClientName="Client",
                OrderDate=DateTime.UtcNow
            };
            var controller = new OrderController();
            controller.ReceiveOrder(order);
        }
    }
}
