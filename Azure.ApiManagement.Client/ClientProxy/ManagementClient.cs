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
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Azure.ResourceManager.ApiManagement.Models;
using Azure.ResourceManager.ApiManagement;
using System.Linq;
using Azure.ApiManagement.Client.DataModel;
using Azure.ApiManagement.Client.MappingProfiles;
using Microsoft.Extensions.Azure;

namespace Fitabase.Azure.ApiManagement
{
    public class ManagementClient
    {
        //static readonly string user_agent = "Fitabase/v1";
        public static readonly int RatesReqTimeout = 25;
        public static readonly int TransactionReqTimeOut = 25;
        static readonly Encoding encoding = Encoding.UTF8;

        static string _api_endpoint;
        static string _resourceGroup;
        static string _serviceId;
        static string _serviceName;
        static string _subscriptionId;
        static string _accessToken;
        static string _apiVersion;

        public string GetEndpoint()
        {
            return _api_endpoint;
        }



        public int TimeoutSeconds { get; set; }


        public ManagementClient(string host, string serviceId, string accessToken)
            : this(host, serviceId, accessToken, Constants.ApiManagement.Versions.Jan2018)
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

        public ManagementClient(string host, string resourceGroup, string serviceId, string serviceName, string accessToken, string subscriptionId, string apiVersion = null)
        {
            _api_endpoint = String.Format("{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}", host, subscriptionId, resourceGroup, serviceName);
            _resourceGroup = resourceGroup;
            _serviceId = serviceId;
            _serviceName = serviceName;
            _subscriptionId = subscriptionId;
            _accessToken = accessToken;
            _apiVersion = String.IsNullOrEmpty(apiVersion) ? Constants.ApiManagement.Versions.Aug2022 : apiVersion;
            TimeoutSeconds = 25;
        }

        public ManagementClient(string filePath)
        {
            Init(filePath);
            TimeoutSeconds = 25;

            if (string.IsNullOrEmpty(_apiVersion))
            {
                _apiVersion = Constants.ApiManagement.Versions.Jan2018;
            }
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
                    _api_endpoint = json["host"]?.ToString();
                    _serviceId = json["serviceId"]?.ToString();
                    _accessToken = json["accessKey"]?.ToString();
                    _apiVersion = json["apiVersion"]?.ToString();
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
            }
            finally
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
        /// Retrieves token used for single sign on authentication 
        /// into the developer portal for a given user.
        /// </summary>
        public async Task<string> GetSsoTokenAsync(string userId, string keyType, string expiry, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/users/{1}/token", _api_endpoint, userId);

            TokenGenerationUsedKeyType tokenValue = TokenGenerationUsedKeyType.Primary;
            if (String.Equals(keyType, "secondary", StringComparison.InvariantCultureIgnoreCase))
            {
                tokenValue = TokenGenerationUsedKeyType.Secondary;
            }

            //format expiry date to ISO 8601, default is UTCNow + 1 hr
            DateTime expiryValue;
            string expiryValueFormatted = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);

            if (DateTime.TryParse(expiry, out expiryValue))
            {

                expiryValueFormatted = expiryValue.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
            }

            dynamic tokenContent = new { properties = new { keyType = tokenValue, expiry = expiryValueFormatted } };

            var ssoToken = await DoRequestAsync<Dictionary<string, string>>(endpoint, RequestMethod.POST, Utility.SerializeToJson(tokenContent), cancellationToken);

            if (ssoToken == null)
            {
                throw new ArgumentException("failure to retrieve SSO token");
            }

            return HttpUtility.UrlEncode(ssoToken["value"]);
        }

        /// <summary>
        /// Create a new user model
        /// </summary>
        public async Task<User> CreateOrUpdateUserAsync(string userId, string firstName, string lastName, string email, string password, string userState, string note, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (firstName == null)
                throw new InvalidEntityException("user first name is required");
            if (lastName == null)
                throw new InvalidEntityException("user last name is required");
            if (email == null)
                throw new InvalidEntityException("user email is required");
            if (String.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            User userContent = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                State = GetUserState(userState),
                Note = note
            };

            RequestBody<User> requestBody = new RequestBody<User>();
            requestBody.Properties = userContent;

            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, userId);
            var userResponse = await DoRequestAsync<ResponseBody<User>>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(requestBody), cancellationToken);

            userResponse.Properties.Id = userResponse.Name;
            userResponse.Properties.Uri = userResponse.Id;

            return userResponse.Properties;
        }

        /// <summary>
        ///  Retrieve a specific user model of a given id
        /// </summary>
        public async Task<User> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
                throw new InvalidEntityException("userId is required");

            string endpoint = String.Format("{0}/users/{1}", _api_endpoint, userId);
            var userResponse = await DoRequestAsync<ResponseBody<User>>(endpoint, RequestMethod.GET, cancellationToken);

            userResponse.Properties.Id = userResponse.Name;
            userResponse.Properties.Uri = userResponse.Id;

            return userResponse.Properties;
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
        public async Task<List<User>> GetUsersAsync(QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/users", _api_endpoint);
            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

            var usersResponse = await DoRequestAsync<EntityCollection<ResponseBody<User>>>(endpoint, RequestMethod.GET, cancellationToken);

            List<User> users = new List<User>();
            foreach (var user in usersResponse.Values)
            {
                User mapUser = new User();
                mapUser = user.Properties;

                mapUser.Id = user.Name;
                mapUser.Uri = user.Id;

                users.Add(mapUser);
            }

            return users;
        }

        /// <summary>
        /// Retrieve a list of subscriptions by the user
        /// </summary>
        public async Task<EntityCollection<Subscription>> GetUserSubscriptionAsync(string userId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public async Task<EntityCollection<Group>> GetUserGroupsAsync(string userId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
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

        public UserState GetUserState(string userState)
        {
            switch (userState)
            {
                case var uState when userState.Equals("active", StringComparison.InvariantCultureIgnoreCase):
                    return UserState.active;
                case var uState when userState.Equals("blocked", StringComparison.InvariantCultureIgnoreCase):
                    return UserState.blocked;
                case var uState when userState.Equals("pending", StringComparison.InvariantCultureIgnoreCase):
                    return UserState.pending;
                case var uState when userState.Equals("deleted", StringComparison.InvariantCultureIgnoreCase):
                    return UserState.deleted;
                default:
                    return UserState.pending;
            }
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

        public async Task<API> GetAPIAsync(string apiId, string revision = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis", _api_endpoint);
            if (!string.IsNullOrWhiteSpace(revision))
            {
                if (!apiId.Contains(";rev="))
                {                                   // Ensure that that the api revision has not included in the API.
                    apiId = string.Format("{0};rev={1}", apiId, revision);
                }
            }
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
        public async Task<EntityCollection<API>> GetAPIsAsync(QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/apis", _api_endpoint);

            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
            return await DoRequestAsync<EntityCollection<API>>(endpoint, RequestMethod.GET, cancellationToken);
        }


        public async Task<EntityCollection<APIRevision>> GetApiRevisions(string apiId)
        {
            string endpoint = String.Format("{0}/apis/{1}/revisions", _api_endpoint, apiId);
            return await DoRequestAsync<EntityCollection<APIRevision>>(endpoint, RequestMethod.GET, null);
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
        public async Task<EntityCollection<APIOperation>> GetOperationsByAPIAsync(string apiId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
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


        /// <summary>
        /// List of policy configuration at the API Operation level.
        /// </summary>
        public async Task<EntityCollection<Policy>> GetOperationPolicyByOperationAsync(string apiId, string operationId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations/{2}/policies", _api_endpoint, apiId, operationId);

            return await DoRequestAsync<EntityCollection<Policy>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// List of policy configuration at the API Operation level.
        /// </summary>
        public async Task<EntityCollection<Policy>> SetOperationPolicyByOperationAsync(string apiId, string operationId, string policyContent, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/apis/{1}/operations/{2}/policies/policy", _api_endpoint, apiId, operationId);
            Policy policy = new Policy()
            {
                PolicyContent = policyContent
            };

            return await DoRequestAsync<EntityCollection<Policy>>(endpoint, RequestMethod.PUT, JsonConvert.SerializeObject(policy), cancellationToken);
        }




        #endregion


        /*********************************************************/
        /**********************  PRODUCT  ************************/
        /*********************************************************/

        #region Product

        /// <summary>
        /// Create a product
        /// </summary>
        public async Task<Product> CreateOrUpdateProductAsync(string productId, string displayName, string productState, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(displayName))
                throw new InvalidEntityException("product display name is required");
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            Product productContent = new Product()
            {
                DisplayName = displayName,
                State = GetProductState(productState)
            };

            RequestBody<Product> requestBody = new RequestBody<Product>();
            requestBody.Properties = productContent;

            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, productId);
            var productResponse = await DoRequestAsync<ResponseBody<Product>>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(requestBody), cancellationToken);

            productResponse.Properties.Id = productResponse.Name;
            productResponse.Properties.Uri = productResponse.Id;

            return productResponse.Properties;
        }

        /// <summary>
        /// Gets the details of the product specified by its identifier.
        /// </summary>
        public async Task<Product> GetProductAsync(string productId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}", _api_endpoint, productId);
            var productResponse = await DoRequestAsync<ResponseBody<Product>>(endpoint, RequestMethod.GET, cancellationToken);
            productResponse.Properties.Id = productResponse.Name;
            productResponse.Properties.Uri = productResponse.Id;

            return productResponse.Properties;
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
        public async Task<List<Product>> GetProductsAsync(QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/products", _api_endpoint);
            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

            var productsResponse = await DoRequestAsync<EntityCollection<ResponseBody<Product>>>(endpoint, RequestMethod.GET, cancellationToken);

            List<Product> products = new List<Product>();
            foreach (var product in productsResponse.Values)
            {
                Product mapProduct = new Product();
                mapProduct = product.Properties;

                mapProduct.Id = product.Name;
                mapProduct.Uri = product.Id;

                products.Add(mapProduct);
            }

            return products;
        }

        /// <summary>
        /// Adds an API to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="api"></param>
        public async Task<API> CreateOrUpdateProductAPIAsync(string productId, string apiId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");
            if (string.IsNullOrEmpty(apiId))
                throw new InvalidEntityException("apiId is required");

            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    _api_endpoint, productId, apiId);
            var productApiResponse = await DoRequestAsync<ResponseBody<API>>(endpoint, RequestMethod.PUT, cancellationToken);

            productApiResponse.Properties.Id = productApiResponse.Name;
            productApiResponse.Properties.Uri = productApiResponse.Id;

            return productApiResponse.Properties;
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
        public async Task<List<API>> GetProductAPIsAsync(string productId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/apis",
                                    _api_endpoint, productId);
            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

            var productApisResponse = await DoRequestAsync<EntityCollection<ResponseBody<API>>>(endpoint, RequestMethod.GET, cancellationToken);

            List<API> apis = new List<API>();
            foreach (var api in productApisResponse.Values)
            {
                API mapAPI = new API();
                mapAPI = api.Properties;

                mapAPI.Id = api.Name;
                mapAPI.Uri = api.Id;

                apis.Add(mapAPI);
            }

            return apis;
        }

        /// <summary>
        /// Lists the collection of subscriptions to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<EntityCollection<Subscription>> GetProductSubscriptionsAsync(string productId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/subscriptions",
                                    _api_endpoint, productId);

            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
            return await DoRequestAsync<EntityCollection<Subscription>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        public ProductState GetProductState(string productState)
        {
            switch (productState)
            {
                case var pState when productState.Equals("published", StringComparison.InvariantCultureIgnoreCase):
                    return ProductState.published;
                case var pState when productState.Equals("notPublished", StringComparison.InvariantCultureIgnoreCase):
                    return ProductState.notPublished;
                default:
                    return ProductState.published;
            }
        }


        #endregion

        #region Product Group

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
            await DoRequestAsync<ResponseBody<API>>(endpoint, RequestMethod.PUT, cancellationToken);

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
        public async Task<List<Group>> GetProductGroupsAsync(string productId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(productId))
                throw new InvalidEntityException("productId is required");

            string endpoint = String.Format("{0}/products/{1}/groups",
                                    _api_endpoint, productId);

            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());

            var productGroupsResponse = await DoRequestAsync<EntityCollection<ResponseBody<Group>>>(endpoint, RequestMethod.GET, cancellationToken);

            List<Group> productGroups = new List<Group>();
            foreach (var productGroup in productGroupsResponse.Values)
            {
                Group mapProduct = new Group();
                mapProduct = productGroup.Properties;

                mapProduct.Id = productGroup.Name;
                mapProduct.Uri = productGroup.Id;

                productGroups.Add(mapProduct);
            }

            return productGroups;
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
        public async Task<EntityCollection<Group>> GetGroupsAsync(QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            string endpoint = String.Format("{0}/groups", _api_endpoint);
            if (filter != null)
                endpoint = string.Format("{0}?{1}", endpoint, filter.GetFilterQuery());
            return await DoRequestAsync<EntityCollection<Group>>(endpoint, RequestMethod.GET, cancellationToken);
        }

        /// <summary>
        /// Lists a collection of the members of the group, specified by its identifier.
        /// </summary>
        public async Task<EntityCollection<User>> GetUsersInGroupAsync(string groupId, QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
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
        public async Task<Subscription> CreateOrUpdateSubscriptionAsync(string displayName, string scope, string ownerId, string subscriptionId, string productOrApiId, string subscriptionState, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(displayName))
                throw new InvalidEntityException("subscription is required");
            if (String.IsNullOrEmpty(scope))
                throw new InvalidEntityException("scope is required");
            if (String.IsNullOrEmpty(subscriptionId))
            {
                subscriptionId = Guid.NewGuid().ToString();
            }

            Subscription subscriptionContent = new Subscription()
            {
                DisplayName = displayName,
                Scope = GetSubscriptionScope(scope, productOrApiId),
                OwnerId = ownerId,
                State = GetSubscriptionState(subscriptionState)
            };

            RequestBody<Subscription> requestBody = new RequestBody<Subscription>();
            requestBody.Properties = subscriptionContent;

            string endpoint = String.Format("{0}/subscriptions/{1}", _api_endpoint, subscriptionId);
            var subscriptionResponse = await DoRequestAsync<ResponseBody<Subscription>>(endpoint, RequestMethod.PUT, Utility.SerializeToJson(requestBody), cancellationToken);

            subscriptionResponse.Properties.Id = subscriptionResponse.Name;
            subscriptionResponse.Properties.Uri = subscriptionResponse.Id;

            return subscriptionResponse.Properties;
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
        public async Task<EntityCollection<Subscription>> GetSubscriptionsAsync(QueryFilterExpression filter = null, CancellationToken cancellationToken = default(CancellationToken))
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

        public Model.SubscriptionState GetSubscriptionState(string subscriptionState)
        {
            switch (subscriptionState)
            {
                case var sState when subscriptionState.Equals("active", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.active;
                case var sState when subscriptionState.Equals("cancelled", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.cancelled;
                case var sState when subscriptionState.Equals("expired", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.expired;
                case var sState when subscriptionState.Equals("rejected", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.rejected;
                case var sState when subscriptionState.Equals("submitted", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.submitted;
                case var sState when subscriptionState.Equals("suspended", StringComparison.InvariantCultureIgnoreCase):
                    return Model.SubscriptionState.suspended;
                default:
                    return Model.SubscriptionState.submitted;
            }
        }

        public string GetSubscriptionScope(string scope, string scopeId)
        {
            switch (scope)
            {
                case var sScope when scope.Equals("products", StringComparison.InvariantCultureIgnoreCase):
                    return $"/products/{scopeId}";
                case var sScope when sScope.Equals("apis", StringComparison.InvariantCultureIgnoreCase):
                    return $"/apis/{scopeId}";
                default:
                    return $"/products/{scopeId}";
            }
        }

        #endregion

    }
}