using SmallStepsLabs.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement
{
    public class ManagementClient : ClientBase
    {
        public ManagementClient(string host, string serviceId, string accessKey) :
            base(host, serviceId, accessKey)
        { }

        #region Products CRUD

        /// <summary>
        /// Get a list of all products
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
        /// </summary>
        /// <returns></returns>
        public Task<EntityCollection<Product>> GetProductsAsync(string filter = null, bool expandGroups = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new NameValueCollection();

            // conditional filter
            if (!String.IsNullOrEmpty(filter))
                query.Add(Constants.ApiManagement.Url.FilterQuery, filter);

            // conditional operation
            if (expandGroups)
                query.Add("expandGroups", "true");

            var request = base.GetRequest("/products", "GET", query);
            return base.ExecuteRequestAsync<EntityCollection<Product>>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Get a specific product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetProduct
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<Product> GetProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<Product>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Create a new product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#CreateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> CreateProductAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            this.ValidateProduct(product);

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PUT", data: product);

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Update a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#UpdateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            this.ValidateProduct(product);

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PATCH", data: product);

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.NoContent, cancellationToken);
        }

        /// <summary>
        /// Delete a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#DeleteProduct
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="deleteSubscriptions">Specify true to indicate that any subscriptions associated with this product should be deleted; otherwise false.If this query parameter is missing, the default is false</param>
        /// <returns></returns>
        public Task<bool> DeleteProductAsync(string productId, bool deleteSubscriptions = false,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}", productId);

            var query = new NameValueCollection();

            // conditional filter
            if (deleteSubscriptions)
                query.Add("deleteSubscriptions", "true");

            var request = base.GetRequest(uri, "DELETE", query);

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region Product APIs

        /// <summary>
        /// List APIs associated with a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListAPIs
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<EntityCollection<API>> GetProductAPIsAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/apis", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<EntityCollection<API>>(request, HttpStatusCode.OK, cancellationToken);
        }


        /// <summary>
        /// Adds an API to the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#AddAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<bool> AddProductAPIAsync(string productId, string apiId,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Removes the specified API from the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#RemoveAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<bool> RemoveProductAPIAsync(string productId, string apiId,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region Product Policy Configuration

        /// <summary>
        /// Gets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> GetProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "GET");

            //TODO - xml response handling
            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Determines if policy configuration is attached to the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> CheckProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "HEAD");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Sets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#SetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> SetProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "PUT");

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Removes the policy configuration for the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> DeleteProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "DELETE");

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync<bool>(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region APIs CRUD

        /// <summary>
        /// Get a list of all APIs
        /// https://msdn.microsoft.com/en-us/library/azure/dn781423.aspx#ListAPIs
        /// </summary>
        /// <returns></returns>
        public Task<EntityCollection<API>> GetAPIsAsync(string filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new NameValueCollection();

            // conditional filter
            if (!String.IsNullOrEmpty(filter))
                query.Add(Constants.ApiManagement.Url.FilterQuery, filter);

            var request = base.GetRequest("/apis", "GET", query);
            return base.ExecuteRequestAsync<EntityCollection<API>>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Get a specific API
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetAPI
        /// </summary>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<Product> GetAPIAsync(string apiId, CancellationToken cancellationToken = default(CancellationToken)) //TODO: Export support ?
        {
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("/apis/{0}", apiId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<Product>(request, HttpStatusCode.OK, cancellationToken);
        }

        #endregion

        #region Helpers

        private void ValidateProduct(Product product)
        {
            if (String.IsNullOrEmpty(product.Name) && product.Name.Length > 100)
                throw new InvalidProductException("");

            if (String.IsNullOrEmpty(product.Description) && product.Description.Length > 1000)
                throw new InvalidProductException("");

            // Cannot provide values for approvalRequired and subscriptionsLimit when subscriptionRequired is set to false in the request payload
            if (!product.SubscriptionRequired)
            {
                if (product.ApprovalRequired.HasValue || product.SubscriptionsLimit.HasValue)
                    throw new InvalidProductException("Product configuration is not valid. Subscription requirement cannot be false while either Subscription limit or Approval required is set");
            }

        }

        #endregion
    }
}
