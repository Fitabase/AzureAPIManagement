using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    public class ManagementClient : ClientBase
    {
        public ManagementClient(string host, string serviceId, string accessKey) :
            base(host, serviceId, accessKey)
        { }


        #region User Operations

        public string GetRequestOperationSignature(string operation, string salt, string delegationValidationKey,
            string returnUrl = null, string productId = null, string userId = null, string subscriptionId = null)
        {
            var encoder = new System.Security.Cryptography.HMACSHA512(Convert.FromBase64String(delegationValidationKey));
            string signature;

            switch (operation)
            {
                case "SignIn":
                    signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + returnUrl)));
                    break;
                case "Subscribe":
                    signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + productId + "\n" + userId)));
                    break;
                case "Unsubscribe":
                    signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + subscriptionId)));
                    break;
                case "ChangeProfile":
                case "ChangePassword":
                case "SignOut":
                    signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + userId)));
                    break;
                default:
                    signature = "";
                    break;
            }

            return signature;
        }

        public Task<List<User>> GetUsersAsync(string filter = null, bool expandGroups = false,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new NameValueCollection();

            // conditional filter
            if (!String.IsNullOrEmpty(filter))
                query.Add(Constants.ApiManagement.Url.FilterQuery, filter);

            // conditional operation
            if (expandGroups)
                query.Add("expandGroups", "true");

            var request = base.BuildRequest("/users", "GET", query);
            return base.ExecuteRequestAsync<EntityCollection<User>>(request, HttpStatusCode.OK, cancellationToken)
                       .ContinueWith(t =>
                       {
                           return t.Result.Values;
                       });
        }

        public Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (String.IsNullOrEmpty(user.Id))
                throw new ArgumentException("Valid User Id is required");

            Utility.ValidateUser(user);

            var uri = String.Format("/users/{0}", user.Id);
            var request = base.BuildRequest(uri, "PUT");

            // build content from supported fields
            base.BuildRequestContent(request, new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                State = user.State,
                Note = user.Note,
            });

            return base.ExecuteRequestAsync(request, HttpStatusCode.Created, cancellationToken);
        }

        public Task<SsoUrl> GetUserSsoLoginAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentException("Valid User Id is required");

            var uri = String.Format("/users/{0}/generateSsoUrl", userId);
            var request = base.BuildRequest(uri, "POST");

            return base.ExecuteRequestAsync<SsoUrl>(request, HttpStatusCode.OK, cancellationToken);
        }

        #endregion

        #region Products CRUD

        /// <summary>
        /// Get a list of all products
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <returns></returns>
        public Task<List<Product>> GetProductsAsync(string filter = null, bool expandGroups = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new NameValueCollection();

            // conditional filter
            if (!String.IsNullOrEmpty(filter))
                query.Add(Constants.ApiManagement.Url.FilterQuery, filter);

            // conditional operation
            if (expandGroups)
                query.Add("expandGroups", "true");

            var request = base.BuildRequest("/products", "GET", query);
            return base.ExecuteRequestAsync<EntityCollection<Product>>(request, HttpStatusCode.OK, cancellationToken)
                       .ContinueWith(t =>
                       {
                           return t.Result.Values;
                       });
        }

        /// <summary>
        /// Get a specific product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetProduct
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <returns></returns>
        public Task<Product> GetProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}", productId);
            var request = base.BuildRequest(uri, "GET");

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

            Utility.ValidateProduct(product);

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.BuildRequest(uri, "PUT");
            base.BuildRequestContent(request, product);

            return base.ExecuteRequestAsync(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Update a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#UpdateProduct
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <returns></returns>
        public Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (String.IsNullOrEmpty(product.Id))
                throw new ArgumentException("Valid Product Id is required");

            Utility.ValidateProduct(product);

            var uri = String.Format("/products/{0}", product.Id);
            var request = base.BuildRequest(uri, "PATCH");
            base.BuildRequestContent(request, product);

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync(request, HttpStatusCode.NoContent, cancellationToken);
        }

        /// <summary>
        /// Delete a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#DeleteProduct
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
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

            var request = base.BuildRequest(uri, "DELETE", query);

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region Product APIs

        /// <summary>
        /// List APIs associated with a product
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListAPIs
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<List<API>> GetProductAPIsAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/apis", productId);
            var request = base.BuildRequest(uri, "GET");

            return base.ExecuteRequestAsync<EntityCollection<API>>(request, HttpStatusCode.OK, cancellationToken)
                       .ContinueWith<List<API>>(t =>
                       {
                           return t.Result.Values;
                       });
        }

        /// <summary>
        /// Adds an API to the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#AddAPI
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
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

            var uri = String.Format("/products/{0}/apis/{1}", productId, apiId);
            var request = base.BuildRequest(uri, "PUT");

            return base.ExecuteRequestAsync(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Removes the specified API from the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#RemoveAPI
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
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

            var uri = String.Format("/products/{0}/apis/{1}", productId, apiId);
            var request = base.BuildRequest(uri, "DELETE");

            return base.ExecuteRequestAsync(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region Product Subscription

        public Task<bool> AddProductSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription");
            if (String.IsNullOrEmpty(subscription.Id))
                throw new ArgumentException("Valid Subscription Id is required");
            if (String.IsNullOrEmpty(subscription.UserId))
                throw new ArgumentException("Valid Subscription User Id is required");
            if (String.IsNullOrEmpty(subscription.ProductId))
                throw new ArgumentException("Valid Subscription Product Id is required");

            //these 2 have a special "relative url" format they need to be passed with
            //https://msdn.microsoft.com/en-us/library/azure/dn776325.aspx#SubscribeProduct

            if (!subscription.UserId.Contains("/users/"))
                subscription.UserId = String.Format("/users/{0}", subscription.UserId);

            if (!subscription.ProductId.Contains("/products/"))
                subscription.ProductId = String.Format("/products/{0}", subscription.ProductId);

            var uri = String.Format("/subscriptions/{0}", subscription.Id);
            var request = base.BuildRequest(uri, "PUT");

            // build content from supported fields
            base.BuildRequestContent(request, new Subscription
            {
                Id = subscription.Id,
                UserId = subscription.UserId,
                ProductId = subscription.ProductId,
                State = subscription.State,
                PrimaryKey = subscription.PrimaryKey,
                SecondaryKey = subscription.SecondaryKey
            });

            return base.ExecuteRequestAsync(request, HttpStatusCode.Created, cancellationToken);
        }

        public Task<bool> RemoveProductSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(subscriptionId))
                throw new ArgumentException("subscriptionId is required");
           
            var uri = String.Format("/subscriptions/{0}", subscriptionId);
            var request = base.BuildRequest(uri, "DELETE");

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        #region Product Policy Configuration

        /// <summary>
        /// Gets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetPolicy
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<string> GetProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.BuildRequest(uri, "GET");

            return base.ExecuteRequestAsync<string>(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Determines if policy configuration is attached to the specified product.
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> CheckProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.BuildRequest(uri, "HEAD");

            return base.ExecuteRequestAsync(request, HttpStatusCode.OK, cancellationToken);
        }

        /// <summary>
        /// Sets the policy configuration for the specified product.
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#SetPolicy
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <param name="policy">Policy content (xml).</param>
        /// <returns></returns>
        public Task<bool> SetProductPolicyAsync(string productId, string policy, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.BuildRequest(uri, "PUT");
            base.BuildRequestContent(request, policy, Constants.MimeTypes.ApplicationXmlPolicy);

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync(request, HttpStatusCode.Created, cancellationToken);
        }

        /// <summary>
        /// Removes the policy configuration for the specified product.
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public Task<bool> DeleteProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var uri = String.Format("/products/{0}/policy", productId);
            var request = base.BuildRequest(uri, "DELETE");

            // Apply changes regardless of entity state
            base.EntityStateUpdate(request, "*");

            return base.ExecuteRequestAsync(request, HttpStatusCode.NoContent, cancellationToken);
        }

        #endregion

        
        
        #region APIs CRUD

        /// <summary>
        /// Get a list of all APIs
        /// https://msdn.microsoft.com/en-us/library/azure/dn781423.aspx#ListAPIs
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <returns></returns>
        public Task<List<API>> GetAPIsAsync(string filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = new NameValueCollection();

            // conditional filter
            if (!String.IsNullOrEmpty(filter))
                query.Add(Constants.ApiManagement.Url.FilterQuery, filter);

            var request = base.BuildRequest(Constants.Request.API_REQUEST, "GET", query);
            return base.ExecuteRequestAsync<EntityCollection<API>>(request, HttpStatusCode.OK, cancellationToken)
                       .ContinueWith<List<API>>(t =>
                       {
                           return t.Result.Values;
                       });
        }

        /// <summary>
        /// Get a specific API
        /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#GetAPI
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="apiId">API identifier.</param>
        /// <returns></returns>
        public Task<API> GetAPIAsync(string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new ArgumentException("apiId is required");

            var uri = String.Format("{0}/{1}", Constants.Request.API_REQUEST, apiId);
            var request = base.BuildRequest(uri, "GET");
            return base.ExecuteRequestAsync<API>(request, HttpStatusCode.OK, cancellationToken);
        }

        #endregion
    }
}
