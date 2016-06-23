using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsWorker.Models
{
    public class Basket
    {
        public Basket() {
            Products = new List<BasketProduct>();
        }

        public string UserId { get; set; }
        public IList<BasketProduct> Products { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public decimal TotalPrice { get; set; }

        public void AddProduct(int productId, int quantity, decimal unitPrice)
        {
            var basketProduct = BasketProduct(productId);
            if (basketProduct == null)
            {
                basketProduct = new BasketProduct { ProductId = productId, QuantityRequested = quantity, CurrentUnitPrice = unitPrice, TotalPrice = unitPrice * quantity };
                Products.Add(basketProduct);
            }
            else
            {
                basketProduct.QuantityRequested += quantity;
                basketProduct.CurrentUnitPrice = unitPrice;
                basketProduct.TotalPrice = basketProduct.CurrentUnitPrice * basketProduct.QuantityRequested;
            }
            Complete();
        }

        public void UpdateProductUnitPrice(int productId, decimal newUnitPrice)
        {
            var basketProduct = BasketProduct(productId);
            basketProduct.CurrentUnitPrice = newUnitPrice;
            basketProduct.TotalPrice = basketProduct.CurrentUnitPrice * basketProduct.QuantityRequested;
            Complete();
        }

        private BasketProduct BasketProduct(int productId)
        {
            return Products.SingleOrDefault(xx => xx.ProductId == productId);
        }

        private void Complete()
        {
            TotalPrice = Products.Sum(xx => xx.TotalPrice);
            LastUpdated = DateTime.Now;
            if (!Created.HasValue) Created = LastUpdated;
        }
    }
}
