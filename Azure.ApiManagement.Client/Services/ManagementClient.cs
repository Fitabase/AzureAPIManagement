using SmallStepsLabs.Azure.ApiManagement.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement
{
    public class ManagementClient : ClientBase
    {
        public ManagementClient(string host, string serviceId, string accessKey):
            base(host, serviceId, accessKey)
        { }

        #region Products CRUD

        /// <summary>
        /// Get a list of all products
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
        /// </summary>
        /// <returns></returns>
        public Task<EntityCollection<Product>> GetProductsAsync(int start = 0, int limit = 10, bool expandGroups = false, 
            CancellationToken token = default(CancellationToken))
        {
            var request = base.GetRequest("/products", "GET");
            return base.ExecuteRequestAsync<EntityCollection<Product>>(request, token);
        }

        /// <summary>
        /// Get a specific product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetProduct
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<Product> GetProductAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<Product>(request, token);
        }

        /// <summary>
        /// Create a new product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#CreateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<EntityOperationResult> CreateProductAsync(Product product, CancellationToken token = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Update a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#UpdateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<EntityOperationResult> UpdateProductAsync(Product product, CancellationToken token = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            // Request header //If - Match

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PATCH");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Delete a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#DeleteProduct
        /// </summary>
        /// <param name="product"></param>
        /// <param name="deleteSubscriptions">Specify true to indicate that any subscriptions associated with this product should be deleted; otherwise false.If this query parameter is missing, the default is false</param>
        /// <returns></returns>
        public Task<EntityOperationResult> DeleteProductAsync(Product product, bool deleteSubscriptions = false,
             CancellationToken token = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            // Request header //If - Match

            var uri = String.Format("/products/{0}", product.Id);

            // conditional operation
            var uriQuery = deleteSubscriptions ? "deleteSubscriptions=true" : String.Empty;

            var request = base.GetRequest(uri, "DELETE", uriQuery);

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        #endregion

        #region Product APIs

        /// <summary>
        /// List APIs associated with a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListAPIs
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityCollection<API>> GetProductAPIsAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/apis", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<EntityCollection<API>>(request, token);
        }


        /// <summary>
        /// Adds an API to the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#AddAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> AddProductAPIAsync(string productId, string apiId,
             CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Removes the specified API from the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#RemoveAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> RemoveProductAPIAsync(string productId, string apiId,
             CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        #endregion

        #region Product Policy Configuration

        /// <summary>
        /// Gets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> GetProductPolicyAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "GET");

            //TODO - xml response handling
            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Determines if policy configuration is attached to the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> CheckProductPolicyAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "HEAD");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Sets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#SetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> SetProductPolicyAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        /// <summary>
        /// Removes the policy configuration for the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityOperationResult> DeleteProductPolicyAsync(string productId, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<EntityOperationResult>(request, token);
        }

        #endregion

        #region APIs CRUD

        /// <summary>
        /// Get a list of all APIs
        /// https://msdn.microsoft.com/en-us/library/azure/dn781423.aspx#ListAPIs
        /// </summary>
        /// <returns></returns>
        public Task<EntityCollection<API>> GetAPIsAsync(int start = 0, int limit = 10,
             CancellationToken token = default(CancellationToken))
        {
            var request = base.GetRequest("/apis", "GET");
            return base.ExecuteRequestAsync<EntityCollection<API>>(request, token);
        }

        /// <summary>
        /// Get a specific API
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetAPI
        /// </summary>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<Product> GetAPIAsync(string apiId, CancellationToken token = default(CancellationToken)) //TODO: Export support ?
        {
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/apis/{0}", apiId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<Product>(request, token);
        }

        #endregion
    }
}
