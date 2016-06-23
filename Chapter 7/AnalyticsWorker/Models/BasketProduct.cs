using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsWorker.Models
{
    public class BasketProduct
    {
        public int QuantityRequested { get; set; }
        public int ProductId { get; set; }
        public decimal CurrentUnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
