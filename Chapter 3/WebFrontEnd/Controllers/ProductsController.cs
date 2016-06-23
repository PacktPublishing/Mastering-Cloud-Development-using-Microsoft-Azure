using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebFrontEnd.Models;
using WebModel;
using System.Configuration;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Web.Security;

namespace WebFrontEnd.Controllers
{
    public class ProductsController : Controller
    {
        private DocumentClient _client;

        protected DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    var endpoint = ConfigurationManager.AppSettings["DocumentDbEndPoint"];
                    var endpointUri = new Uri(endpoint);
                    var authKey = ConfigurationManager.AppSettings["DocumentDbAuthKey"];
                    _client = new DocumentClient(endpointUri, authKey);
                }
                return _client;
            }
        }

        private SearchIndexClient _search;
        protected SearchIndexClient Search
        {
            get
            {
                if (_search == null)
                {
                    var serviceName = ConfigurationManager.AppSettings["SearchServiceName"];
                    var queryKey = ConfigurationManager.AppSettings["SearchQueryKey"];
                    _search = new SearchIndexClient(serviceName, "products", new SearchCredentials(queryKey));
                }
                return _search;
            }
        }

        private CloudBlobClient _blobClient;
        protected CloudBlobClient BlobClient
        {
            get
            {
                if (_blobClient == null)
                {
                    var cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["cloudStorageAccount"]);
                    _blobClient = cloudStorageAccount.CreateCloudBlobClient();
                }
                return _blobClient;
            }
        }

        private CloudTableClient _tableClient;
        protected CloudTableClient TableClient
        {
            get
            {
                if (_tableClient == null)
                {
                    var cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["cloudStorageAccount"]);
                    _tableClient = cloudStorageAccount.CreateCloudTableClient();
                }
                return _tableClient;
            }
        }

        private IDatabase _redisDatabase;
        protected IDatabase RedisDatabase
        {
            get
            {
                if (_redisDatabase == null)
                {
                    var mux = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisConnectionString"]);
                    _redisDatabase = mux.GetDatabase();
                }
                return _redisDatabase;
            }
        }


        protected IOrderedQueryable<ProductDetailDTO> Products()
        {
            return Client.CreateDocumentQuery<ProductDetailDTO>("/dbs/AdventureWorksLT/colls/documents");
        }

        public async Task<JsonResult> Suggest(string text, int pageNumber = 1, int pageSize = 20, string orderBy = "Name")
        {
            var parameters = new SuggestParameters
            {
            };
            var result = await Search.Documents.SuggestAsync(text, "text", parameters);
            var items = result.Select(xx => xx.Text).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Index(string text, int pageNumber = 1, int pageSize = 20, string orderBy = "Name")
        {
            var parameters = new SearchParameters {
            };
            parameters.OrderBy.Add(orderBy);
            parameters.Skip = (pageNumber - 1) * pageSize;
            parameters.Top = pageSize;
            var result = await Search.Documents.SearchAsync<ProductIndexDTO>(text, parameters);
            var items = result.Select(xx => xx.Document).ToList();
            return View(items);
        }

        public async Task<ActionResult> IndexDocumentDb(Guid? categoryId)
        {
            var categories = Products()
                .Select(xx => new { Id = xx.ProductCategory.Id, Name = xx.ProductCategory.Name })
                .ToList()
                .Distinct()
                .Select(xx => new ProductCategoryDTO { Id = xx.Id, Name = xx.Name })
                .ToList()
            ;
            ViewBag.categories = categories;

            var query = Products()
                .Where(xx => !categoryId.HasValue || categoryId.HasValue && xx.ProductCategory.Id == categoryId)
                .Select(xx => new { id = xx.id, Name = xx.Name, ProductNumber = xx.ProductNumber })
                .AsDocumentQuery()
            ;

            var response = await query.ExecuteNextAsync();

            var items =
                response
                .ToList()
                .Select(xx => new ProductIndexDTO { id = Guid.Parse(xx.id), Name = xx.Name, ProductNumber = xx.ProductNumber });
            return View(items);
        }

        // GET: Products/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var key = string.Format("product_detail_{0}", id);
            if (!RedisDatabase.KeyExists(key))
            {
                // ...
                var productDetailDTO = Products().Where(xx => xx.id == id).ToList();
                if (productDetailDTO == null)
                {
                    return HttpNotFound();
                }
                var json = JsonConvert.SerializeObject(productDetailDTO);
                RedisDatabase.StringSet(key, json, TimeSpan.FromHours(4));
                RedisDatabase.KeyDelete(key);
                return View(productDetailDTO[0]);
            }
            else
            {
                // ...
                var json = RedisDatabase.StringGet(key);
                var productDetailDto = JsonConvert.DeserializeObject<ProductDetailDTO>(json);
                return View(productDetailDto);
            }
        }


        public async Task<ActionResult> Images(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // ...
            var productDetailDTO = Products().Where(xx => xx.id == id).ToList();
            // ...
            if (productDetailDTO == null)
            {
                return HttpNotFound();
            }
            var container = BlobClient.GetContainerReference("webfrontendimages");
            var blob = container.GetBlobReference(productDetailDTO[0].ThumbnailPhotoFileName);
            Response.ContentType = blob.Properties.ContentType;
            await blob.DownloadToStreamAsync(Response.OutputStream);
            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(string productId, decimal unitPrice, int quantity)
        {
            var shoppingCart = TableClient.GetTableReference("shoppingcart");
            shoppingCart.CreateIfNotExists();

            var userSession = Membership.GetUser().UserName;

            var retrieve = TableOperation.Retrieve<ProductCartEntity>(userSession, productId);
            var result = shoppingCart.Execute(retrieve);
            var item = (ProductCartEntity)result.Result;
            if (item != null)
            {
                item.UnitPrice = unitPrice;
                item.Quantity = quantity;
            }
            else
            {
                item = new ProductCartEntity
                {
                    PartitionKey = userSession
                    ,
                    RowKey = productId
                    ,
                    CreateDate = DateTime.Now
                    ,
                    Currency = "EUR"
                    ,
                    UnitPrice = unitPrice
                    ,
                    Quantity = quantity
                };
            }
            item.TotalPrice = item.UnitPrice * item.Quantity;

            var upsert = TableOperation.InsertOrReplace(item);
            shoppingCart.Execute(upsert);

            return RedirectToAction("Index");
        }

        private IEnumerable<ProductCartDTO> ShoppingCart()
        {
            var shoppingCart = TableClient.GetTableReference("shoppingcart");
            shoppingCart.CreateIfNotExists();

            var userSession = Membership.GetUser().UserName;

            var query = (TableQuery<ProductCartEntity>) shoppingCart.CreateQuery<ProductCartEntity>()
                .Where(xx => xx.PartitionKey == userSession);

            var shoppingCartItems = shoppingCart.ExecuteQuery(query)
                .Select(xx => new ProductCartDTO {
                    Currency = xx.Currency
                    , ProductDescription = xx.ProductDescription
                    , ProductId = xx.RowKey
                    , Quantity = xx.Quantity
                    , TotalPrice = xx.TotalPrice
                    , UnitPrice = xx.UnitPrice
                }).ToList();

            return shoppingCartItems;
        }


        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,SellStartDate,SellEndDate,DiscontinuedDate,ThumbnailPhotoFileName,ModifiedDate,Type")] ProductDetailDTO productDetailDTO)
        {
            if (ModelState.IsValid)
            {
                await Client.CreateDocumentAsync("/dbs/AdventureWorksLT/colls/documents", productDetailDTO);
                return RedirectToAction("Index");
            }

            return View(productDetailDTO);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetailDTO = Products().Where(xx => xx.id == id).ToList();
            if (productDetailDTO == null)
            {
                return HttpNotFound();
            }
            return View(productDetailDTO[0]);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,SellStartDate,SellEndDate,DiscontinuedDate,ThumbnailPhotoFileName,ModifiedDate,Type")] ProductDetailDTO productDetailDTO)
        {
            if (ModelState.IsValid)
            {
                await Client.UpsertDocumentAsync("/dbs/AdventureWorksLT/colls/documents", productDetailDTO);
                return RedirectToAction("Index");
            }
            return View(productDetailDTO);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productDetailDTO = Products().Where(xx => xx.id == id).ToList();
            if (productDetailDTO == null)
            {
                return HttpNotFound();
            }
            return View(productDetailDTO[0]);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await Client.DeleteDocumentAsync("/dbs/AdventureWorksLT/colls/documents/docs/" + id.ToString());
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
