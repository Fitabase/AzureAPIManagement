using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
                    if (url.EndsWith("?"))
                    {
                        builder.Append("api-version=").Append(_apiVersion);
                    }
                    else
                    {
                        builder.Append("&api-version=").Append(_apiVersion);
                    }
                }
                else
                {
                    builder.Append("?api-version=").Append(_apiVersion);
                }
            }
            return builder.ToString();
        }


        protected virtual HttpRequestMessage GetRequest(String method, string uri, string body = null)
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
                    content = new StringContent(body, Encoding.UTF8, "application/json");
                }
            }
            else if (method == RequestMethod.PATCH.ToString())
            {
                content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Headers.Add("If-Match", "*");
            }
            else if (method == RequestMethod.DELETE.ToString())
            {
                request.Headers.Add("If-Match", "*");
            }

            request.Headers.Add("Authorization", Constants.ApiManagement.AccessToken + " " + token);
            request.Headers.Add("api-version", _apiVersion);
            request.Content = content;

            return request;
        }
        

        #region Generic Requests

        public virtual T DoRequest<T>(string endpoint, RequestMethod method = RequestMethod.GET, string body = null)
        {
            //var json = DoRequest(endpoint, method.ToString(), body);
            string json = null;
            Task.Run(async () =>
            {
                json = await MakeRequest(endpoint, method.ToString(), body);
            }).GetAwaiter().GetResult();

            if (String.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public virtual T GetById<T>(string endpoint, string ID)
        {
            string[] splits = ID.Split('_');
            string entitySignatureName = (splits.Length > 1) ? splits[0] : "entity";
            T entity;
            try
            {
                return entity = DoRequest<T>(String.Format("{0}/{1}", endpoint, ID));
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

        public virtual async Task<string> MakeRequest(string endpoint, string method, string body)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = GetRequest(method, endpoint, body);
            HttpResponseMessage response = await client.SendAsync(request);
            string result = await OnHandleResponseAsync(response);
            return result;
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

        
        #endregion




        /*********************************************************/
        /************************  USER  *************************/
        /*********************************************************/

        #region USER

        /// <summary>
        /// Retrieves a redirection URL containing an authentication 
        /// token for signing a given user into the developer portal.
        /// </summary>
        public SsoUrl GenerateSsoURL(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}/generateSsoUrl", _api_endpoint, userId);
            return DoRequest<SsoUrl>(endpoint, RequestMethod.POST);
        }

        /// <summary>
        /// Create a new user model
        /// </summary>
        public User CreateUser(User user)
        {
            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, user.Id);
            DoRequest<User>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(user));
            return user;
        }

        /// <summary>
        ///  Retrieve a specific user model of a given id
        /// </summary>
        public User GetUser(string userId)
        {
            string endpoint = String.Format("{0}/users", _api_endpoint);
            return GetById<User>(endpoint, userId);
        }


        /// <summary>
        /// Delete a specific user model of a given id
        /// </summary>
        public void DeleteUser(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, userId);
            DoRequest<User>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Delete user's subscriptions
        /// </summary>
        public void DeleteUserWithSubscriptions(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}?deleteSubscriptions=true", _api_endpoint, userId);
            DoRequest<User>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Retrieve all user models
        /// </summary>
        public EntityCollection<User> GetUsers()
        {
            string endpoint = String.Format("{0}/users", _api_endpoint);
            return DoRequest<EntityCollection<User>>(endpoint);
        }

        /// <summary>
        /// Retrieve a list of subscriptions by the user
        /// </summary>
        public EntityCollection<Subscription> GetUserSubscription(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}/subscriptions", _api_endpoint, userId);
            return DoRequest<EntityCollection<Subscription>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Retrieve a list of groups that the specific user belongs to
        /// </summary>
        public EntityCollection<Group> GetUserGroups(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}/groups", _api_endpoint, userId);
            return DoRequest<EntityCollection<Group>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Update a specific user model
        /// </summary>
        public void UpdateUser(User user)
        {
            if (String.IsNullOrWhiteSpace(user.Id))
                throw new InvalidEntityException("User's Id is required");
            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, user.Id);
            DoRequest<User>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(user));
        }

        #endregion






        /*********************************************************/
        /*************************  API  *************************/
        /*********************************************************/

        #region API

        /// <summary>
        /// Creates new API of the API Management service instance.
        /// </summary>
        public API CreateAPI(API api)
        {
            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, api.Id);
            DoRequest<API>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(api));
            return api;
        }

        /// <summary>
        /// Gets the details of the API specified by its identifier.
        /// </summary>
        public API GetAPI(string apiId)
        {
            string endpoint = String.Format("{0}/apis", _api_endpoint);
            return GetById<API>(endpoint, apiId);
        }

        /// <summary>
        /// Deletes the specified API of the API Management service instance.
        /// </summary>
        public void DeleteAPI(string apiId)
        {
            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, apiId);
            DoRequest<API>(endpoint, RequestMethod.DELETE);
        }

        public void UpdateAPI(API api)
        {
            if (String.IsNullOrWhiteSpace(api.Id))
                throw new InvalidEntityException("API's Id is required");
            string endpoint = String.Format("{0}/apis/{1}", _api_endpoint, api.Id);
            DoRequest<API>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(api));
        }

        /// <summary>
        /// Lists all APIs of the API Management service instance.
        /// </summary>
        public EntityCollection<API> GetAPIs()
        {
            string endpoint = String.Format("{0}/apis", _api_endpoint);
            return DoRequest<EntityCollection<API>>(endpoint, RequestMethod.GET);
        }


        #endregion





        /*********************************************************/
        /******************   API OPERATIONS  ********************/
        /*********************************************************/

        #region API Operations


        /// <summary>
        /// Creates a new operation in the API
        /// </summary>
        public APIOperation CreateAPIOperation(string apiId, APIOperation operation)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operation.Id);
            DoRequest<APIOperation>(endpoint, RequestMethod.PUT, JsonConvert.SerializeObject(operation));
            return operation;
        }
        public APIOperation CreateAPIOperation(API api, APIOperation operation)
        {
            return CreateAPIOperation(api.Id, operation);
        }

        public void UpdateAPIOperation(string apiId, string operationId, APIOperation operation)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operationId);
            DoRequest<APIOperation>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(operation));
        }

        /// <summary>
        /// Gets the details of the API Operation specified by its identifier.
        /// </summary>
        public APIOperation GetAPIOperation(string apiId, string operationId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations", _api_endpoint, apiId);
            return GetById<APIOperation>(endpoint, operationId);

        }

        /// <summary>
        /// Lists a collection of the operations for the specified API.
        /// </summary>
        public EntityCollection<APIOperation> GetOperationsByAPI(string apiId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations", _api_endpoint, apiId);
            return DoRequest<EntityCollection<APIOperation>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Deletes the specified operation in the API.
        /// </summary>
        public void DeleteOperation(string apiId, string operationId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                _api_endpoint, apiId, operationId);
            DoRequest<APIOperation>(endpoint, RequestMethod.DELETE);
        }


        #endregion






        /*********************************************************/
        /**********************  PRODUCT  ************************/
        /*********************************************************/

        #region Product

        /// <summary>
        /// Create a product
        /// </summary>
        public Product CreateProduct(Product product)
        {
            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, product.Id);
            DoRequest<Product>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(product));
            return product;
        }

        /// <summary>
        /// Gets the details of the product specified by its identifier.
        /// </summary>
        public Product GetProduct(string productId)
        {
            string endpoint = String.Format("{0}/products", _api_endpoint);
            return GetById<Product>(endpoint, productId);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        public void UpdateProduct(Product product)
        {
            if (String.IsNullOrWhiteSpace(product.Id))
                throw new InvalidEntityException("Product's Id is required");
            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, product.Id);
            DoRequest<Product>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(product));
        }


        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="productId"></param>
        public void DeleteProduct(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}?deleteSubscriptions=true", _api_endpoint, productId);
            DoRequest<Product>(endpoint, RequestMethod.DELETE);
        }


        /// <summary>
        /// Lists a collection of products in the specified service instance.
        /// </summary>
        public EntityCollection<Product> GetProducts()
        {
            string endpoint = String.Format("{0}/products", _api_endpoint);
            return DoRequest<EntityCollection<Product>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Adds an API to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="api"></param>
        public void AddProductAPI(string productId, string apiId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    _api_endpoint, productId, apiId);
            DoRequest<API>(endpoint, RequestMethod.PUT);
        }

        /// <summary>
        /// Deletes the specified API from the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="apiId"></param>
        public void DeleteProductAPI(string productId, string apiId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    _api_endpoint, productId, apiId);
            DoRequest<API>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Lists the collection of apis to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<API> GetProductAPIs(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis",
                                    _api_endpoint, productId);
            return DoRequest<EntityCollection<API>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Lists the collection of subscriptions to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<Subscription> GetProductSubscriptions(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/subscriptions",
                                    _api_endpoint, productId);
            return DoRequest<EntityCollection<Subscription>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Adds the association between the specified developer group with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public void AddProductGroup(string productId, string groupId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    _api_endpoint, productId, groupId);
            DoRequest<API>(endpoint, RequestMethod.PUT);

        }

        /// <summary>
        /// Deletes the association between the specified group and product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public void DeleteProductGroup(string productId, string groupId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    _api_endpoint, productId, groupId);
            DoRequest<Group>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Lists the collection of developer groups associated with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<Group> GetProductGroups(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups",
                                    _api_endpoint, productId);
            return DoRequest<EntityCollection<Group>>(endpoint, RequestMethod.GET);
        }

        #endregion





        /*********************************************************/
        /**********************  GROUP  **************************/
        /*********************************************************/

        #region Group

        /// <summary>
        /// Create a group
        /// </summary>
        public Group CreateGroup(Group group)
        {
            string endpoint = String.Format("{0}/groups/{1}", _api_endpoint, group.Id);
            DoRequest<Group>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(group));
            return group;
        }

        /// <summary>
        /// Gets the details of the group specified by its identifier.
        /// </summary>
        public Group GetGroup(string groupId)
        {
            string endpoint = String.Format("{0}/groups", _api_endpoint);
            return GetById<Group>(endpoint, groupId);
        }

        /// <summary>
        /// Add a user to the specified group
        /// </summary>
        public void AddUserToGroup(string groupId, string userId)
        {
            string endpoint = String.Format("{0}/groups/{1}/users/{2}", _api_endpoint, groupId, userId);
            DoRequest<EntityCollection<User>>(endpoint, RequestMethod.PUT);
        }

        /// <summary>
        /// Remove existing user from existing group.
        /// </summary>
        public void RemoveUserFromGroup(string groupId, string userId)
        {

            string endpoint = String.Format("{0}/groups/{1}/users/{2}", _api_endpoint, groupId, userId);
            DoRequest<EntityCollection<User>>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Lists a collection of groups
        /// </summary>
        public EntityCollection<Group> GetGroups()
        {
            string endpoint = String.Format("{0}/groups", _api_endpoint);
            return DoRequest<EntityCollection<Group>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Lists a collection of the members of the group, specified by its identifier.
        /// </summary>
        public EntityCollection<User> GetUsersInGroup(string groupId)
        {
            string endpoint = String.Format("{0}/groups/{1}/users", _api_endpoint, groupId);
            return DoRequest<EntityCollection<User>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Deletes specific group of the API Management
        /// </summary>
        public void DeleteGroup(string groupId)
        {
            string endpoint = String.Format("{0}/groups/{1}", _api_endpoint, groupId);
            DoRequest<Group>(endpoint, RequestMethod.DELETE);
        }
        #endregion






        /*********************************************************/
        /**********************  SUBSCRIPTION  *******************/
        /*********************************************************/


        #region Subscription

        /// <summary>
        /// Creates or updates the subscription of specified user to the specified product.
        /// </summary>
        public Subscription CreateSubscription(Subscription subscription)
        {
            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscription.Id);
            DoRequest<Subscription>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(subscription));
            return subscription;
        }

        /// <summary>
        /// Gets the specified Subscription entity.
        /// </summary>
        public Subscription GetSubscription(string subscriptionId)
        {
            string endpoint = String.Format("{0}/subscriptions", _api_endpoint);
            return GetById<Subscription>(endpoint, subscriptionId);
        }

        /// <summary>
        /// Deletes the specified subscription.
        /// </summary>
        public void DeleteSubscription(string subscriptionId)
        {
            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscriptionId);
            DoRequest<Subscription>(endpoint, RequestMethod.DELETE);
        }

        /// <summary>
        /// Updates the details of a subscription specificied by its identifier.
        /// </summary>
        public void UpdateSubscription(Subscription subscription)
        {
            if (String.IsNullOrWhiteSpace(subscription.Id))
                throw new InvalidEntityException("Subscription's Id is required");
            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscription.Id);
            DoRequest<Subscription>(endpoint, RequestMethod.PATCH, JsonConvert.SerializeObject(subscription));
        }

        /// <summary>
        /// Lists all subscriptions of the API Management service instance.
        /// </summary>
        public EntityCollection<Subscription> GetSubscriptions()
        {
            string endpoint = String.Format("{0}/subscriptions", _api_endpoint);
            return DoRequest<EntityCollection<Subscription>>(endpoint, RequestMethod.GET);
        }

        /// <summary>
        /// Gernerate subscription primary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public void GeneratePrimaryKey(string subscriptionId)
        {
            string endPoint = String.Format("{0}/subscriptions/{1}/regeneratePrimaryKey", _api_endpoint, subscriptionId);
            DoRequest<string>(endPoint, RequestMethod.POST);
        }

        /// <summary>
        /// Generate subscription secondary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public void GenerateSecondaryKey(string subscriptionId)
        {
            string endPoint = String.Format("{0}/subscriptions/{1}/regenerateSecondaryKey", _api_endpoint, subscriptionId);
            DoRequest<string>(endPoint, RequestMethod.POST);
        }
        #endregion





        /*********************************************************/
        /**********************  LOGGERs  ************************/
        /*********************************************************/
        #region Loggers

        public Logger CreateLogger(Logger logger)
        {
            string endpoint = String.Format("{0}/loggers/{1}", _api_endpoint, logger.Id);
            DoRequest<Logger>(endpoint, RequestMethod.PUT, JsonConvert.SerializeObject(logger));
            return logger;
        }

        public EntityCollection<Logger> GetLoggers()
        {
            string endpoint = String.Format("{0}/loggers", _api_endpoint);
            return DoRequest<EntityCollection<Logger>>(endpoint);
        }

        public Logger GetLogger(string loggerId)
        {
            string endpoint = String.Format("{0}/loggers", _api_endpoint, loggerId);
            return GetById<Logger>(endpoint, loggerId);
        }

        public void DeleteLogger(string loggerId)
        {
            string endpoint = String.Format("{0}/loggers/{1}", _api_endpoint, loggerId);
            DoRequest<Logger>(endpoint, RequestMethod.DELETE);
        }

        #endregion

    }
}