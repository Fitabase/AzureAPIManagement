using SmallStepsLabs.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement
{
    public class ProductsClient : ClientBase
    {
        #region Products CRUD

        /// <summary>
        /// Get a list of all products
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProductsAsync(int top, int skip, bool expandGroups = false)
        {
            var request = base.GetRequest("/products", "GET");
            return base.ExecuteRequestAsync<IEnumerable<Product>>(request);
        }

        /// <summary>
        /// Get a specific product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetProduct
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<Product> GetProductAsync(string productId)
        {
            var uri = String.Format("/products/{0}", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<Product>(request);
        }

        /// <summary>
        /// Create a new product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#CreateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> CreateProductAsync(Product product)
        {
            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Update a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#UpdateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> UpdateProductAsync(Product product)
        {
            // Request header //If - Match

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "PATCH");

            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Delete a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#DeleteProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> DeleteProductAsync(Product product)
        {
            // Request header //If - Match

            // Query Parameter
            //Type
            //Description
            //deleteSubscriptions
            //boolean
            //Specify true to indicate that any subscriptions associated with this product should be deleted; otherwise false.If this query parameter is missing, the default is false.

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<bool>(request);
        }

        #endregion

        #region Product APIs

        /// <summary>
        /// List APIs associated with a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListAPIs
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<IEnumerable<API>> GetProductAPIsAsync(string productId)
        {
            var uri = String.Format("/products/{0}/apis", productId);
            var request = base.GetRequest(uri, "GET");

            return base.ExecuteRequestAsync<IEnumerable<API>>(request);
        }


        /// <summary>
        /// Adds an API to the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#AddAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<bool> AddProductAPIAsync(string productId, string apiId)
        {
            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Removes the specified API from the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#RemoveAPI
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<bool> RemoveProductAPIAsync(string productId, string apiId)
        {
            var uri = String.Format("/products/{0}/apis/{0}", productId, apiId);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<bool>(request);
        }

        #endregion

        #region Product Policy Configuration

        /// <summary>
        /// Gets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> GetProductPolicyAsync(string productId)
        {
            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "GET");

            //TODO - xml response handling
            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Determines if policy configuration is attached to the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> CheckProductPolicyAsync(string productId)
        {
            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "HEAD");

            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Sets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#SetPolicy
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> SetProductPolicyAsync(string productId)
        {
            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "PUT");

            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Removes the policy configuration for the specified product.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> DeleteProductPolicyAsync(string productId)
        {
            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.GetRequest(uri, "DELETE");

            return base.ExecuteRequestAsync<bool>(request);
        }

        #endregion
    }
}
