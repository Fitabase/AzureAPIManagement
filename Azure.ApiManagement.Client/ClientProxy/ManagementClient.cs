using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
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

        static string api_endpoint;
        static string serviceId;
        static string accessToken;
        static string apiVersion;



        public int TimeoutSeconds { get; set; }


        public ManagementClient()
        {
            Init();
            TimeoutSeconds = 25;
        }


        /// <summary>
        /// Read and initialize keys
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private void Init(string filePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\APIMKeys.json")
        {
            string apiKeysContent;
            try
            {
                using (StreamReader sr = new StreamReader(filePath)) //make sure this file has "Copy to output directory" Set to "Copy Always"
                {
                    apiKeysContent = sr.ReadToEnd();
                    var json = JObject.Parse(apiKeysContent);
                    api_endpoint = json["apiEndpoint"].ToString();
                    serviceId = json["serviceId"].ToString();
                    accessToken = json["accessKey"].ToString();
                    apiVersion = json["apiVersion"].ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private string AppendUrlApiVersion(string url)
        {
            return (url.Contains("?"))
                            ? ("&api-version=" + apiVersion)
                            : ("?api-version=" + apiVersion);
        }

        /// <summary>
        /// Set up request header metadata for each api call
        /// </summary>
        /// <param name="method">request method</param>
        /// <param name="url">endpoint request url</param>
        /// <returns>WebRequest</returns>
        protected virtual WebRequest SetupRequestHeader(String method, string url, string body = null)
        {
            url += AppendUrlApiVersion(url);
            string token = Utility.CreateSharedAccessToken(serviceId, accessToken, DateTime.UtcNow.AddDays(1));
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = TimeoutSeconds * 1000;
            request.Headers.Add("Authorization", Constants.ApiManagement.AccessToken + " " + token);
            request.Headers.Add("api-version", apiVersion);

            // Add header metadata depending on request method
            if (method == "POST" || method == "PUT")
            {
                request.ContentType = "application/json";
                if (body == null)
                {
                    request.ContentLength = 0;
                }
            }
            else if (method == "PATCH")
            {
                request.Accept = "application/json";
                request.ContentType = "application/json";
                request.Headers.Add("If-Match", "*");
            }
            else if (method == "DELETE")
            {
                request.Headers.Add("If-Match", "*");
            }

            return request;
        }

        static string GetResponseAsString(WebResponse response)
        {
            using (StreamReader responseStreamReader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return responseStreamReader.ReadToEnd();
            }
        }



        #region Generic Requests

        public virtual T DoRequest<T>(string endpoint, string method = "GET", string body = null)
        {
            var json = DoRequest(endpoint, method, body);
            var jsonDeserialized = Utility.DeserializeToJson<T>(json);
            return jsonDeserialized;
        }


        public virtual string DoRequest(string endpoint, string method, string body)
        {
            string result = null;

            WebRequest request = SetupRequestHeader(method, endpoint);

            if (body != null)
            {
                byte[] requestBodyBytes = encoding.GetBytes(body.ToString());
                request.ContentLength = requestBodyBytes.Length;
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                }
            }

            try
            {
                using (WebResponse resp = (WebResponse)request.GetResponse())
                {
                    result = GetResponseAsString(resp);
                }
            }
            catch (WebException wexc)
            {
                if (wexc.Response != null)
                {
                    string json_error = GetResponseAsString(wexc.Response);
                    HttpStatusCode status_code = HttpStatusCode.BadRequest;
                    HttpWebResponse resp = wexc.Response as HttpWebResponse;
                    
                    if (resp != null)
                        status_code = resp.StatusCode;

                    if ((int)status_code <= 500)
                    {
                        throw new Exception(json_error, wexc);
                    }
                }
                throw;
            }
            return result;
        }

        #endregion




        #region USER

        /// <summary>
        /// Retrieves a redirection URL containing an authentication 
        /// token for signing a given user into the developer portal.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SsoUrl GenerateSsoURL(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}/generateSsoUrl", api_endpoint, userId);
            return DoRequest<SsoUrl>(endpoint, "POST");
        }
        public User CreateUser(string userId, User user)
        {
            Validator.ValidateUser(user);
            string endpoint = String.Format("{0}/users/{1}", api_endpoint, userId);
            return DoRequest<User>(endpoint, "PUT", Utility.SerializeToJson(user));
        }
        public User GetUser(string userId)
        {
            string endpoint = String.Format("{0}/users/{1}", api_endpoint, userId);
            return DoRequest<User>(endpoint, "GET");
        }

        public EntityCollection<User> AllUsers()
        {
            string endpoint = String.Format("{0}/users", api_endpoint);
            return DoRequest<EntityCollection<User>>(endpoint);
        }
        public User UpdateUser(string userId, Hashtable parameters)
        {
            string endpoint = String.Format("{0}/users/{1}", api_endpoint, userId);
            return DoRequest<User>(endpoint, "PATCH", Utility.SerializeToJson(parameters));
        }
    

        #endregion


        #region API
        public API CreateAPI(string apiId, API api)
        {
            string endpoint = String.Format("{0}/apis/{1}", api_endpoint, apiId);
            return DoRequest<API>(endpoint, "PUT", Utility.SerializeToJson(api));
        }
        public API GetAPI(string id)
        {
            string endpoint = String.Format("{0}/apis/{1}", api_endpoint, id);
            return DoRequest<API>(endpoint, "GET");
        }
        public EntityCollection<API> AllAPIs()
        {
            string endpoint = String.Format("{0}/apis", api_endpoint);
            return DoRequest<EntityCollection<API>>(endpoint, "GET");
        }


        #endregion


        #region API Operations

        public APIOperation CreateAPIOperation(string apiId, string operationId, APIOperation operation)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                api_endpoint, apiId, operationId);
            return DoRequest<APIOperation>(endpoint, "PUT", Utility.SerializeToJson(operation));
        }
        public APIOperation GetAPIOperation(string apiId, string operationId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                api_endpoint, apiId, operationId);
            return DoRequest<APIOperation>(endpoint, "GET");
        }
        public EntityCollection<APIOperation> GetByAPI(string apiId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations", api_endpoint, apiId);
            return DoRequest<EntityCollection<APIOperation>>(endpoint, "GET");
        }
        public APIOperation DeleteOperation(string apiId, string operationId)
        {
            string endpoint = String.Format("{0}/apis/{1}/operations/{2}",
                                                api_endpoint, apiId, operationId);
            return DoRequest<APIOperation>(endpoint, "DELETE");
        }
        #endregion


        #region Product
        public Product CreateProduct(string productId, Product product)
        {
            Validator.ValidateProduct(product);
            string endpoint = String.Format("{0}/products/{1}", api_endpoint, product);
            return DoRequest<Product>(endpoint, "PUT", Utility.SerializeToJson(product));
        }
        public Product GetProduct(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}", api_endpoint, productId);
            return DoRequest<Product>(endpoint, "GET");
        }
        public void UpdateProduct(Product product)
        {
            string endpoint = String.Format("{0}/products/{1}", api_endpoint, product.Id);
            DoRequest<Product>(endpoint, "PATCH", Utility.SerializeToJson(product));
        }
        public Product DeleteProduct(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}?deleteSubscriptions=true", api_endpoint, productId);
            return DoRequest<Product>(endpoint, "DELETE");
        }
        public EntityCollection<Product> AllProducts()
        {
            string endpoint = String.Format("{0}/products", api_endpoint);
            return DoRequest<EntityCollection<Product>>(endpoint, "GET");
        }

        /// <summary>
        /// Adds an API to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="api"></param>
        public void AddProductAPI(string productId, string apiId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    api_endpoint, productId, apiId);
            DoRequest<API>(endpoint, "PUT");
        }

        /// <summary>
        /// Deletes the specified API from the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="apiId"></param>
        public void DeleteProductAPI(string productId, string apiId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis/{2}",
                                    api_endpoint, productId, apiId);
            DoRequest<API>(endpoint, "DELETE");
        }

        /// <summary>
        /// Lists the collection of apis to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<API> GetProductAPIs(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/apis",
                                    api_endpoint, productId);
            return DoRequest<EntityCollection<API>>(endpoint, "GET");
        }

        /// <summary>
        /// Lists the collection of subscriptions to the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<Subscription> GetProductSubscriptions(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/subscriptions",
                                    api_endpoint, productId);
            return DoRequest<EntityCollection<Subscription>>(endpoint, "GET");
        }

        /// <summary>
        /// Adds the association between the specified developer group with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public void AddProductGroup(string productId, string groupId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    api_endpoint, productId, groupId);
            DoRequest<API>(endpoint, "PUT");

        }

        /// <summary>
        /// Deletes the association between the specified group and product.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        public void DeleteProductGroup(string productId, string groupId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups/{2}",
                                    api_endpoint, productId, groupId);
            DoRequest<Group>(endpoint, "DELETE");
        }

        /// <summary>
        /// Lists the collection of developer groups associated with the specified product.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public EntityCollection<Group> GetProductGroups(string productId)
        {
            string endpoint = String.Format("{0}/products/{1}/groups",
                                    api_endpoint, productId);
            return DoRequest<EntityCollection<Group>>(endpoint, "GET");
        }

        #endregion


        #region Group
        public Group CreateGroup(string groupId, Group group)
        {
            string endpoint = String.Format("{0}/groups/{1}", api_endpoint, group);
            return DoRequest<Group>(endpoint, "PUT", Utility.SerializeToJson(group));
        }
        public Group GetGroup(string groupId)
        {
            string endpoint = String.Format("{0}/groups/{1}", api_endpoint, groupId);
            return DoRequest<Group>(endpoint, "GET");
        }
        public EntityCollection<Group> AllGroups()
        {
            string endpoint = String.Format("{0}/groups", api_endpoint);
            return DoRequest<EntityCollection<Group>>(endpoint, "GET");
        }
        public Group DeleteGroup(string groupId)
        {
            string endpoint = String.Format("{0}/groups/{1}", api_endpoint, groupId);
            return DoRequest<Group>(endpoint, "DELETE");
        }
        #endregion


        #region Subscription
        public Subscription CreateSubscription(string subscriptionId, Product subscription)
        {
            string endpoint = String.Format("{0}/subscriptions/{1}", api_endpoint, subscription);
            return DoRequest<Subscription>(endpoint, "PUT", Utility.SerializeToJson(subscription));
        }
        public Subscription GetSubscription(string subscriptionId)
        {
            string endpoint = String.Format("{0}/subscriptions/{1}", api_endpoint, subscriptionId);
            return DoRequest<Subscription>(endpoint, "GET");
        }
        public EntityCollection<Subscription> AllSubscriptions()
        {
            string endpoint = String.Format("{0}/subscriptions", api_endpoint);
            return DoRequest<EntityCollection<Subscription>>(endpoint, "GET");
        }

        /// <summary>
        /// Gernerate subscription primary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public void GeneratePrimaryKey(string subscriptionId)
        {
            string endPoint = String.Format("{0}/subscriptions/{1}/regeneratePrimaryKey", api_endpoint, subscriptionId);
            DoRequest<string>(endPoint, "POST");
        }

        /// <summary>
        /// Generate subscription secondary key
        /// </summary>
        /// <param name="subscriptionId">Subscription credentials which uniquely identify Microsoft Azure subscription</param>
        public void GenerateSecondaryKey(string subscriptionId)
        {
            string endPoint = String.Format("{0}/subscriptions/{1}/regenerateSecondaryKey", api_endpoint, subscriptionId);
            DoRequest<string>(endPoint, "POST");
        }
        #endregion


    }
}
