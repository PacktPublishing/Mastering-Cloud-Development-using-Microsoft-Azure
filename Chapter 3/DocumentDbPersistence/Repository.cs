using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDbPersistence
{
    public class Repository<T>
    {
        #region Client

        //Use the Database if it exists, if not create a new Database
        public Database ReadOrCreateDatabase(string databaseId)
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = databaseId }).Result;
            }

            return db;
        }

        public Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }


        //Use the DocumentCollection if it exists, if not create a new Collection
        public DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                var collectionSpec = new DocumentCollection { Id = CollectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        //Expose the "database" value from configuration as a property for internal use
        private string databaseId;
        private String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["DocumentDbDatabase"];
                }

                return databaseId;
            }
        }

        //Expose the "collection" value from configuration as a property for internal use
        private string collectionId;
        private String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["DocumentDbCollection"];
                }

                return collectionId;
            }
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private Database database;
        private Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private DocumentCollection collection;
        private DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private DocumentClient client;
        private DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["DocumentDbEndPoint"];
                    string authKey = ConfigurationManager.AppSettings["DocumentDbAuthKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        #endregion

        public IQueryable<T> GetItems()
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .AsQueryable();
        }

        public IQueryable<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(predicate);
        }

        public async Task<PageDTO<TResult>> QueryItemsAsync<TResult>(string sql, dynamic args = null, int maxItemCount = 10, string continuation = "")
        {
            var options = new FeedOptions
            {
                MaxItemCount = maxItemCount
            };
            if (!string.IsNullOrWhiteSpace(continuation)) options.RequestContinuation = continuation;

            IDocumentQuery<TResult> query = null;

            if (args != null)
            {
                SqlQuerySpec sqlQuery = new SqlQuerySpec(sql);
                sqlQuery.Parameters = new SqlParameterCollection();
                foreach (System.Reflection.PropertyInfo pi in args.GetType().GetProperties())
                {
                    sqlQuery.Parameters.Add(new SqlParameter("@" + pi.Name, pi.GetValue(args)));
                }
                query = Client.CreateDocumentQuery<TResult>(Collection.DocumentsLink, sqlQuery).AsDocumentQuery();
            }
            else
            {
                query = Client.CreateDocumentQuery<TResult>(Collection.DocumentsLink, sql).AsDocumentQuery();
            }

            var result = await query.ExecuteNextAsync<TResult>();

            var page = new PageDTO<TResult>
            {
                Items = result.ToList()
                ,
                Continuation = result.ResponseContinuation
                ,
                RequestCharge = (decimal)result.RequestCharge
                ,
                CollectionQuota = result.CollectionQuota
                ,
                CollectionSizeQuota = result.CollectionSizeQuota
                ,
                CollectionSizeUsage = result.CollectionSizeUsage
                ,
                CollectionUsage = result.CollectionUsage
            };
            return page;
        }

        public async Task<PageDTO<T>> GetPageAsync(Expression<Func<T, bool>> predicate, int maxItemCount, string continuation = "")
        {

            return page;
        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }


        private Document GetDocument(Expression<Func<Document, bool>> predicate)
        {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                .Where(predicate)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public async Task<Document> UpdateItemAsync(Expression<Func<Document, bool>> predicate, T item)
        {
            Document doc = GetDocument(predicate);
            var result = await Client.ReplaceDocumentAsync(doc.SelfLink, item);
            return result;
        }

        public async Task<Document> InsertItemAsync(T item)
        {
            Client.ReadDocumentCollectionAsync()
            var result = await Client.CreateDocumentAsync(doc.SelfLink, item);

            return result;
        }
    }
}
