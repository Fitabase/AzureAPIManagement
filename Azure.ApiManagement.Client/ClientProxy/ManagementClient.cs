using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    public class ManagementClient
    {
        //static readonly string user_agent = "Fitabase/v1";
        public static readonly int RatesReqTimeout = 25;
        public static readonly int TransactionReqTimeOut = 25;
        static readonly Encoding encoding = Encoding.UTF8;

        static string _api_endpoint;
        static string _serviceId;
        static string _accessToken;
        static string _apiVersion;

        public string GetEndpoint()
        {
            return _api_endpoint;
        }



        public int TimeoutSeconds { get; set; }


        public ManagementClient(string host, string serviceId, string accessToken)
            : this(host, serviceId, accessToken, Constants.ApiManagement.Versions.Feb2014)
        {

        }

        public ManagementClient(string host, string serviceId, string accessToken, string apiversion)
        {
            _api_endpoint = host;
            _serviceId = serviceId;
            _accessToken = accessToken;
            _apiVersion = apiversion;
            TimeoutSeconds = 25;
        }

        public ManagementClient(string filePath)
        {
            Init(filePath);
            TimeoutSeconds = 25;
        }

        /// <summary>
        /// Read and initialize keys
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private void Init(string filePath)
        {
            string apiKeysContent;
            try
            {
                using (StreamReader sr = new StreamReader(filePath)) //make sure this file has "Copy to output directory" Set to "Copy Always"
                {
                    apiKeysContent = sr.ReadToEnd();
                    var json = JObject.Parse(apiKeysContent);
                    _api_endpoint = json["apiEndpoint"].ToString();
                    _serviceId = json["serviceId"].ToString();
                    _accessToken = json["accessKey"].ToString();
                    _apiVersion = json["apiVersion"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Ensure the endpoint is properly formatted
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetFormatedEndpoint(string url)
        {
            StringBuilder builder = new StringBuilder();

            // ensure the url contains the api endpoint with proper format
            // url = _api_endpoint/request_endpoint
            if (!url.Contains(_api_endpoint))
            {
                if (url.StartsWith("/"))
                {
                    builder.Append(_api_endpoint);
                }
                else
                {
                    builder.Append(_api_endpoint).Append("/");
                }
                builder.Append(url);
            }
            else
            {
                builder.Append(url);
            }

            // ensure the url contains the api version 
            // url = _api_endpoint/request_endpoint?api-version=_apiVersion
            // or url = _api_endpoint/request_endpoint?params&api-version=_apiVersion
            if (!url.Contains(_apiVersion))
            {
                if (url.Contains("?"))
                {
                    if (!url.EndsWith("?"))
                    {
                        builder.Append("&");
                    }
                }
                else
                {
                    builder.Append("?");
                }
                builder.Append(Constants.ApiManagement.Url.VersionQuery).Append("=").Append(_apiVersion);
            }
            return builder.ToString();
        }


        protected virtual HttpRequestMessage GetRequest(String method, string uri, string body)
        {
            string endpointURI = GetFormatedEndpoint(uri);
            string token = Utility.CreateSharedAccessToken(_serviceId, _accessToken, DateTime.UtcNow.AddDays(1));
            
            HttpMethod httpMethod = new HttpMethod(method);
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, endpointURI);
            HttpContent content = null;
            

            if (method == RequestMethod.POST.ToString() || method == RequestMethod.PUT.ToString())
            {
                if (body != null)
                {
                    content = new StringContent(body, Encoding.UTF8, Constants.MimeTypes.ApplicationJson);
                }
            }
            else if (method == RequestMethod.PATCH.ToString())
            {
                content = new StringContent(body, Encoding.UTF8, Constants.MimeTypes.ApplicationJson);
                request.Headers.Add(Constants.ApiManagement.Headers.ETagMatch, "*");
            }
            else if (method == RequestMethod.DELETE.ToString())
            {
                request.Headers.Add(Constants.ApiManagement.Headers.ETagMatch, "*");
            }

            request.Headers.Add(Constants.ApiManagement.Headers.Authorization, Constants.ApiManagement.AccessToken + " " + token);
            request.Headers.Add(Constants.ApiManagement.Url.VersionQuery, _apiVersion);
            request.Content = content;

            return request;
        }


        #region Generic Requests
        
        public virtual async Task<T> DoRequestAsync<T>(string endpoint, RequestMethod request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await DoRequestAsync<T>(endpoint, request, null, cancellationToken);
        }

        public virtual async Task<T> DoRequestAsync<T>(string endpoint, RequestMethod method, string body, 
                                                        CancellationToken cancellationToken = default(CancellationToken))
        {
            string json = await DoRequestAsync(endpoint, method.ToString(), body, cancellationToken);
            if (String.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
        


        public virtual async Task<string> DoRequestAsync(string endpoint, string method, string body, CancellationToken cancellationToken)
        {
            HttpClient httpClient = null;
            HttpRequestMessage request = null;
            HttpResponseMessage response = null;
            try
            {
                httpClient = new HttpClient();
                request = GetRequest(method, endpoint, body);
                response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
                string result = await OnHandleResponseAsync(response);
                return result;
            } finally
            {
                // Cleanup
                if (request != null)
                {
                    request.Dispose();
                    request = null;
                }

                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }


        public virtual async Task<string> OnHandleResponseAsync(HttpResponseMessage response)
        {
            if (response == null)
                throw new HttpResponseException("Unable to get response message", HttpStatusCode.BadRequest);

            if (!response.IsSuccessStatusCode)
            {
                string message = response.Content.ReadAsStringAsync().Result;
                throw new HttpResponseException(message, response.StatusCode);
            }

            return await response.Content.ReadAsStringAsync();
        }

		public virtual async Task<T> GetByIdAsync<T>(string endpoint, string ID, CancellationToken cancellationToken = default(CancellationToken))
        {
            string[] splits = ID.Split('_');
            string entitySignatureName = (splits.Length > 1) ? splits[0] : "entity";
            try
            {
                T entity = await DoRequestAsync<T>(String.Format("{0}/{1}", endpoint, ID), RequestMethod.GET, cancellationToken);
                return entity;
            }
            catch (HttpResponseException)
            {
                string message = String.Format("Unable to find the {0} with ID = {1}", entitySignatureName, ID);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(message),
                    ReasonPhrase = message
                };

                throw new HttpResponseException(resp);
            }
        }



        #endregion



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


        /*********************************************************/
        /************************  USER  *************************/
        /*********************************************************/

        #region USER

        /// <summary>
        /// Retrieves a redirection URL containing an authentication 
        /// token for signing a given user into the developer portal.
        /// </summary>
        public async Task<SsoUrl> GenerateSsoURLAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new ArgumentException("userId is required");

            string endpoint = String.Format("{0}/users/{1}/generateSsoUrl", _api_endpoint, userId);
            return await DoRequestAsync<SsoUrl>(endpoint, RequestMethod.POST, cancellationToken);
        }

        /// <summary>
        /// Create a new user model
        /// </summary>
        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new InvalidEntityException("user is required");
            if (String.IsNullOrEmpty(user.Id))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, user.Id);
            await DoRequestAsync<User>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(user), cancellationToken);
            return user;
        }

        /// <summary>
        ///  Retrieve a specific user model of a given id
        /// </summary>
        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users", _api_endpoint);
            return await GetByIdAsync<User>(endpoint, userId, cancellationToken);
        }


        /// <summary>
        /// Delete a specific user model of a given id
        /// </summary>
        public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, userId);
            await DoRequestAsync<User>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Delete user's subscriptions
        /// </summary>
        public async Task DeleteUserWithSubscriptionsAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}?deleteSubscriptions=true", _api_endpoint, userId);
            await DoRequestAsync<User>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Retrieve all user models
        /// </summary>
        public async Task<EntityCollection<User>> GetUsersAsync(QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/users", _api_endpoint);
			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
            return await DoRequestAsync<EntityCollection<User>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Retrieve a list of subscriptions by the user
        /// </summary>
        public async Task<EntityCollection<Subscription>> GetUserSubscriptionAsync(string userId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}/subscriptions", _api_endpoint, userId);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

			return await DoRequestAsync<EntityCollection<Subscription>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Retrieve a list of groups that the specific user belongs to
        /// </summary>
        public async Task<EntityCollection<Group>> GetUserGroupsAsync(string userId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}/groups", _api_endpoint, userId);
			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<Group>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Update a specific user model
        /// </summary>
        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
                throw new InvalidEntityException("user is required");
            if (String.IsNullOrEmpty(user.Id))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, user.Id);
            await DoRequestAsync<User>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(user), cancellationToken);
        }

        #endregion






        /*********************************************************/
        /*************************  API  *************************/
        /*********************************************************/

        #region API

        /// <summary>
        /// Creates new API of the API Management service instance.
        /// </summary>
        public async Task<API> CreateAPIAsync(API api, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (api == null)
                throw new InvalidEntityException("API is required");
            if (String.IsNullOrEmpty(api.Id))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, api.Id);
            await DoRequestAsync<API>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(api), cancellationToken);
            return api;
        }

        /// <summary>
        /// Gets the details of the API specified by its identifier.
        /// </summary>
        public async Task<API> GetAPIAsync(string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis", _api_endpoint);
            return await GetByIdAsync<API>(endpoint, apiId, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified API of the API Management service instance.
        /// </summary>
        public async Task DeleteAPIAsync(string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, apiId);
            await DoRequestAsync<API>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        public async Task UpdateAPIAsync(API api, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (api == null)
                throw new InvalidEntityException("API is required");
            if (String.IsNullOrEmpty(api.Id))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, api.Id);
            await DoRequestAsync<API>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(api), cancellationToken);
        }

        /// <summary>
        /// Lists all APIs of the API Management service instance.
        /// </summary>
        public async Task<EntityCollection<API>> GetAPIsAsync(QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/apis", _api_endpoint);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<API>>(endpoint, RequestMethod.GET, cancellationToken);
        }


        #endregion





        /*********************************************************/
        /******************   API OPERATIONS  ********************/
        /*********************************************************/

        #region API Operations


        /// <summary>
        /// Creates a new operation in the API
        /// </summary>
        public async Task<APIOperation> CreateAPIOperationAsync(string apiId, APIOperation operation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");
            if (operation == null)
                throw new InvalidEntityException("operation is required");
            if (String.IsNullOrEmpty(operation.Id))
                throw new InvalidEntityException("operationId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operation.Id);
            await DoRequestAsync<APIOperation>(endpoint, RequestMethod.PUT, JsonConvert.SerializeObject(operation), cancellationToken);
            return operation;
        }
        public async Task<APIOperation> CreateAPIOperationAsync(API api, APIOperation operation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (api == null)
                throw new InvalidEntityException("API is required");
            if (String.IsNullOrEmpty(api.Id))
                throw new InvalidEntityException("apiId is required");
            if (operation == null)
                throw new InvalidEntityException("operation is required");
            if (String.IsNullOrEmpty(operation.Id))
                throw new InvalidEntityException("operationId is required");

            return await CreateAPIOperationAsync(api.Id, operation, cancellationToken);
        }

        public async Task UpdateAPIOperationAsync(string apiId, string operationId, APIOperation operation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");
            if (String.IsNullOrEmpty(operationId))
                throw new InvalidEntityException("operationId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operationId);
            await DoRequestAsync<APIOperation>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(operation), cancellationToken);
        }

        /// <summary>
        /// Gets the details of the API Operation specified by its identifier.
        /// </summary>
        public async Task<APIOperation> GetAPIOperationAsync(string apiId, string operationId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");
            if (String.IsNullOrEmpty(operationId))
                throw new InvalidEntityException("operationId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations", _api_endpoint, apiId);
            return await GetByIdAsync<APIOperation>(endpoint, operationId, cancellationToken);

        }

        /// <summary>
        /// Lists a collection of the operations for the specified API.
        /// </summary>
        public async Task<EntityCollection<APIOperation>> GetOperationsByAPIAsync(string apiId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations", _api_endpoint, apiId);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<APIOperation>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified operation in the API.
        /// </summary>
        public async Task DeleteOperationAsync(string apiId, string operationId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");
            if (String.IsNullOrEmpty(operationId))
                throw new InvalidEntityException("operationId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operationId);
            await DoRequestAsync<APIOperation>(endpoint, RequestMethod.DELETE, cancellationToken);
        }


        #endregion






        /*********************************************************/
        /**********************  PRODUCT  ************************/
        /*********************************************************/

        #region Product

        /// <summary>
        /// Create a product
        /// </summary>
        public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (product == null)
                throw new InvalidEntityException("product is required");
            if (String.IsNullOrEmpty(product.Id))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, product.Id);
            await DoRequestAsync<Product>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(product), cancellationToken);
            return product;
        }

        /// <summary>
        /// Gets the details of the product specified by its identifier.
        /// </summary>
        public async Task<Product> GetProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products", _api_endpoint);
            return await GetByIdAsync<Product>(endpoint, productId, cancellationToken);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (product == null)
                throw new InvalidEntityException("product is required");
            if (String.IsNullOrEmpty(product.Id))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, product.Id);
            await DoRequestAsync<Product>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(product), cancellationToken);
        }


        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="productId"></param>
        public async Task DeleteProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}?deleteSubscriptions=true", _api_endpoint, productId);
            await DoRequestAsync<Product>(endpoint, RequestMethod.DELETE, cancellationToken);
        }


        /// <summary>
        /// Lists a collection of products in the specified service instance.
        /// </summary>
        public async Task<EntityCollection<Product>> GetProductsAsync(QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/products", _api_endpoint);
			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<Product>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Adds an API to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="api"></param>
        public async Task AddProductAPIAsync(string productId, string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");
            if (string.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    _api_endpoint, productId, apiId);
            await DoRequestAsync<API>(endpoint, RequestMethod.PUT, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified API from the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="apiId"></param>
        public async Task DeleteProductAPIAsync(string productId, string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");
            if (string.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    _api_endpoint, productId, apiId);
            await DoRequestAsync<API>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Lists the collection of apis to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<EntityCollection<API>> GetProductAPIsAsync(string productId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/apis",
                                    _api_endpoint, productId);
			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

			return await DoRequestAsync<EntityCollection<API>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Lists the collection of subscriptions to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<EntityCollection<Subscription>> GetProductSubscriptionsAsync(string productId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/subscriptions",
                                    _api_endpoint, productId);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<Subscription>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Adds the association between the specified developer group with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public async Task AddProductGroupAsync(string productId, string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");
            if (string.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    _api_endpoint, productId, groupId);
            await DoRequestAsync<API>(endpoint, RequestMethod.PUT, cancellationToken);

        }

        /// <summary>
        /// Deletes the association between the specified group and product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public async Task DeleteProductGroupAsync(string productId, string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");
            if (string.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    _api_endpoint, productId, groupId);
            await DoRequestAsync<Group>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Lists the collection of developer groups associated with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<EntityCollection<Group>> GetProductGroupsAsync(string productId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/groups",
                                    _api_endpoint, productId);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

			return await DoRequestAsync<EntityCollection<Group>>(endpoint, RequestMethod.GET, cancellationToken);
        }


        #endregion

        #region Product Policy Configuration

        /// <summary>
        /// Gets the policy configuration for the specified product.
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public async Task<string> GetProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var endpoint = String.Format("{0}/products/{1}/policy", _api_endpoint, productId);
            return await DoRequestAsync<string>(endpoint, RequestMethod.GET, cancellationToken);
        }


        /// <summary>
        /// Determines if policy configuration is attached to the specified product.
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown on invalid operation.</exception>
        /// <param name="productId">Product identifier.</param>
        /// <returns></returns>
        public async Task<bool> CheckProductPolicyAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new ArgumentException("productId is required");

            var endpoint = String.Format("{0}/products/{1}/policy", _api_endpoint, productId);
            return await DoRequestAsync<bool>(endpoint, RequestMethod.HEAD, cancellationToken);
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

            var endpoint = String.Format("{0}/products/{1}/policy", _api_endpoint, productId);
            return DoRequestAsync<bool>(endpoint, RequestMethod.PUT, policy, cancellationToken);
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

            var endpoint = String.Format("{0}/products/{1}/policy", _api_endpoint, productId);
            return DoRequestAsync<bool>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        #endregion




        /*********************************************************/
        /**********************  GROUP  **************************/
        /*********************************************************/

        #region Group

        /// <summary>
        /// Create a group
        /// </summary>
        public async Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (group == null)
                throw new InvalidEntityException("group is required");
            if (String.IsNullOrEmpty(group.Id))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/groups/{1}", _api_endpoint, group.Id);
            await DoRequestAsync<Group>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(group), cancellationToken);
            return group;
        }

        /// <summary>
        /// Gets the details of the group specified by its identifier.
        /// </summary>
        public async Task<Group> GetGroupAsync(string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/groups", _api_endpoint);
            return await GetByIdAsync<Group>(endpoint, groupId, cancellationToken);
        }

        /// <summary>
        /// Add a user to the specified group
        /// </summary>
        public async Task AddUserToGroupAsync(string groupId, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/groups/{1}/users/{2}", _api_endpoint, groupId, userId);
            await DoRequestAsync<EntityCollection<User>>(endpoint, RequestMethod.PUT, cancellationToken);
        }

        /// <summary>
        /// Remove existing user from existing group.
        /// </summary>
        public async Task RemoveUserFromGroupAsync(string groupId, string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/groups/{1}/users/{2}", _api_endpoint, groupId, userId);
            await DoRequestAsync<EntityCollection<User>>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Lists a collection of groups
        /// </summary>
        public async Task<EntityCollection<Group>> GetGroupsAsync(QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/groups", _api_endpoint);
			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
			return await DoRequestAsync<EntityCollection<Group>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Lists a collection of the members of the group, specified by its identifier.
        /// </summary>
        public async Task<EntityCollection<User>> GetUsersInGroupAsync(string groupId, QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/groups/{1}/users", _api_endpoint, groupId);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

			return await DoRequestAsync<EntityCollection<User>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Deletes specific group of the API Management
        /// </summary>
        public async Task DeleteGroupAsync(string groupId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(groupId))
                throw new InvalidEntityException("groupId is required");

            string endpoint = String.Format("{0}/groups/{1}", _api_endpoint, groupId);
            await DoRequestAsync<Group>(endpoint, RequestMethod.DELETE, cancellationToken);
        }
        #endregion






        /*********************************************************/
        /**********************  SUBSCRIPTION  *******************/
        /*********************************************************/


        #region Subscription

        /// <summary>
        /// Creates or updates the subscription of specified user to the specified product.
        /// </summary>
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (subscription == null)
                throw new InvalidEntityException("subscription is required");
            if (String.IsNullOrEmpty(subscription.Id))
                throw new InvalidEntityException("subscriptionId is required");

            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscription.Id);
            await DoRequestAsync<Subscription>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(subscription), cancellationToken);
            return subscription;
        }

        /// <summary>
        /// Gets the specified Subscription entity.
        /// </summary>
        public async Task<Subscription> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(subscriptionId))
                throw new InvalidEntityException("subscriptionId is required");

            string endpoint = String.Format("{0}/subscriptions", _api_endpoint);
            return await GetByIdAsync<Subscription>(endpoint, subscriptionId, cancellationToken);
        }

        /// <summary>
        /// Deletes the specified subscription.
        /// </summary>
        public async Task DeleteSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(subscriptionId))
                throw new InvalidEntityException("subscriptionId is required");

            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscriptionId);
            await DoRequestAsync<Subscription>(endpoint, RequestMethod.DELETE, cancellationToken);
        }

        /// <summary>
        /// Updates the details of a subscription specificied by its identifier.
        /// </summary>
        public async Task UpdateSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (subscription == null)
                throw new InvalidEntityException("subscription is required");
            if (String.IsNullOrEmpty(subscription.Id))
                throw new InvalidEntityException("subscriptionId is required");

            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscription.Id);
            await DoRequestAsync<Subscription>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(subscription), cancellationToken);
        }

        /// <summary>
        /// Lists all subscriptions of the API Management service instance.
        /// </summary>
        public async Task<EntityCollection<Subscription>> GetSubscriptionsAsync(QueryFilter filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/subscriptions", _api_endpoint);

			if (filter != null)
				endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

			return await DoRequestAsync<EntityCollection<Subscription>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Gernerate subscription primary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public async Task GeneratePrimaryKeyAsync(string subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(subscriptionId))
                throw new InvalidEntityException("subscriptionId is required");

            string endPoint = String.Format("{0}/subscriptions/{1}/regeneratePrimaryKey", _api_endpoint, subscriptionId);
            await DoRequestAsync<string>(endPoint, RequestMethod.POST, cancellationToken);
        }

        /// <summary>
        /// Generate subscription secondary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public async Task GenerateSecondaryKeyAsync(string subscriptionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(subscriptionId))
                throw new InvalidEntityException("subscriptionId is required");

            string endPoint = String.Format("{0}/subscriptions/{1}/regenerateSecondaryKey", _api_endpoint, subscriptionId);
            await DoRequestAsync<string>(endPoint, RequestMethod.POST, cancellationToken);
        }
        #endregion
    }
}