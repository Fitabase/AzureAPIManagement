using SmallStepsLabs.Azure.ApiManagement.Model;
using SmallStepsLabs.Azure.ApiManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement
{
    //TODO:
    //Add an API to a product
    //Remove an API from a product
    //Get policy configuration for a product
    //Set policy configuration for a product

    public class ProductsClient : ApiManagementClientBase
    {
        /// <summary>
        /// Get a list of all products
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
        /// </summary>
        /// <returns></returns>
        [RestCall("/products", "GET")]

        public Task<List<Product>> GetProducts()
        {
            var request = base.BuildRequest();
            return base.ExecuteRequestAsync<List<Product>>(request);
        }

        /// <summary>
        /// Get a specific product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetProduct
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [RestCall("/products/{pid}", "GET")]
        public Task<Product> GetProduct(string productId)
        {
            var request = base.BuildRequest(productId);
            return base.ExecuteRequestAsync<Product>(request);
        }

        /// <summary>
        /// Create a new product
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [RestCall("/products/{pid}", "PUT")]
        public Task<bool> CreateProduct(Product product)
        {
            var request = base.BuildRequest(product);
            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [RestCall("/products/{pid}", "PATCH")]
        public Task<bool> UpdateProduct(Product product)
        {
            // Request header //If - Match

            var request = base.BuildRequest(product);
            return base.ExecuteRequestAsync<bool>(request);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [RestCall("/products/{pid}", "DELETE")]
        public Task<bool> DeleteProduct(Product product)
        {
            // Request header //If - Match

            // Query Parameter
            //Type
            //Description
            //deleteSubscriptions
            //boolean
            //Specify true to indicate that any subscriptions associated with this product should be deleted; otherwise false.If this query parameter is missing, the default is false.

            var request = base.BuildRequest(product);
            return base.ExecuteRequestAsync<bool>(request);
        }




        /// <summary>
        /// List APIs associated with a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListAPIs
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [RestCall("/products/{pid}/apis", "GET")]
        public Task<List<API>> GetProductAPIs(string productId)
        {
            var request = base.BuildRequest(productId);
            return base.ExecuteRequestAsync<List<API>>(request);
        }


        protected override string ResolveRequestUrl(string parameterizedUrl, object data)
        {
            if (data != null)
                throw new Exception(""); //TODO: custom ex for now supplied data

            var productId = String.Empty;

            if(data is Product)
                productId = (data as Product).Id;

            if(data is String)
                productId = data as String;

            return parameterizedUrl.Replace("{pid}", productId);
        }
    }
}
