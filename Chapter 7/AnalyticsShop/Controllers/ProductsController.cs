using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AnalyticsShop.Models;

namespace AnalyticsShop.Controllers
{
    public class ProductsController : ServiceBusApiController
    {
        private AnalyticsShopContext db = new AnalyticsShopContext();

        public async Task<IList<vProductModelCatalogDescription>> GetProducts()
        {
            var result = await db.vProductModelCatalogDescriptions.ToListAsync();
            return result;
        }

        public async Task<vProductModelCatalogDescription> GetProduct(int id)
        {
            var result = await db.vProductModelCatalogDescriptions.SingleOrDefaultAsync(xx => xx.ProductModelID == id);
            await SendEventHubAsync("Navigation", new {
                LocalTime = DateTime.Now.ToUniversalTime(),
                UserId = User.Identity.Name,
                Type = "ProductView",
                ProductId = result.ProductModelID,
                ProductName = result.Name,
                ProductUrl = result.ProductURL
            });
            return result;
        }
    }
}