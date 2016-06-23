using System;

namespace OrderGenerator
{
    public class Order
    {
        public double Amount { get; set; }
        public string ClientName { get; set; }
        public int ID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}