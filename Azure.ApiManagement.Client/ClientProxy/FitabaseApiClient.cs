using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    public class FitabaseApiClient
    {
        //static readonly string user_agent = "Fitabase/v1";
        public static readonly int RatesReqTimeout = 25;
        public static readonly int TransactionReqTimeOut = 25;
        static readonly Encoding encoding = Encoding.UTF8;

        static string api_endpoint = "";
        static string serviceId;  
        static string accessToken;
        static string apiVersion;
        static string user_agent;
        

       
        public int TimeoutSeconds { get; set; }


        public FitabaseApiClient()
        {
            init();
            TimeoutSeconds = 25;
        }
        

        /// <summary>
        /// Read and initialize keys
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private void init(string filePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\APIMKeys.json")
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



        /// <summary>
        /// Set up request header for each api call
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual WebRequest SetupRequestHeader(String method, string url)
        {
            if(url.Contains("?"))
            {
                url += "&api-version=" + apiVersion;
            } else
            {
                url += "?api-version=" + apiVersion;
            }
            
            WebRequest req = WebRequest.Create(url) as HttpWebRequest;
            if (req is HttpWebRequest && user_agent != null)
            {
                ((HttpWebRequest)req).UserAgent = user_agent;
            }

            string token = Utility.CreateSharedAccessToken(serviceId, accessToken, DateTime.UtcNow.AddDays(1));

            req.Method = method;            
            req.Headers.Add("Authorization", Constants.ApiManagement.AccessToken + " " + token);
            req.Headers.Add("api-version", apiVersion);
            req.Timeout = TimeoutSeconds * 1000;
            
            // Set request constent type
            if (method == "POST" || method == "PUT" || method == "PATCH")
            {
                req.ContentType = "application/json";
            }

            return req;
        }



        static string GetResponseAsString(WebResponse response)
        {
            using (StreamReader responseStreamReader = new StreamReader(response.GetResponseStream(), encoding))
            {
                return responseStreamReader.ReadToEnd();
            }
        }
        



        #region GenericRequests
            
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public virtual T DoRequest<T> (string endpoint, string method = "GET", string body = null)
        {

            var json = DoRequest(endpoint, method, body);
            PrintMessage.Debug(this.GetType().Name, "json: " + json);
            //var jsonDeserialized = JsonConvert.DeserializeObject<T>(json);
            var jsonDeserialized = Utility.DeserializeToJson<T>(json);
            return jsonDeserialized;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        /// <param name="body"></param>
        /// <returns></returns>
     
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

                    PrintMessage.Debug(this.GetType().Name, resp.ContentType);
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
        public User CreateUser(string userId, User user)
        {
            Validator.ValidateUser(user);
            //string endpoint = String.Format("{0}/users/{1}/generateSsoUrl", api_endpoint, userId);
            string endpoint = String.Format("{0}/users/{1}", api_endpoint, userId);
            return DoRequest<User>(endpoint, "PUT", Utility.SerializeToJson(user));
        }
        public User GetUser(string id)
        {
            string endpoint = String.Format("{0}/users/{1}", api_endpoint, id);
            return DoRequest<User>(endpoint, "GET");
        }
        public FitabaseCollection<User> AllUsers()
        {
            string endpoint = String.Format("{0}/users", api_endpoint);
            //PrintMessage.Debug(this.GetType().Name, "endpoint: " + endpoint);
            return DoRequest<FitabaseCollection<User>>(endpoint);
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
        public FitabaseCollection<API> AllAPIs()
        {
            string endpoint = String.Format("{0}/apis", api_endpoint);
            return DoRequest<FitabaseCollection<API>>(endpoint, "GET");
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

        public Product UpdateProduct(string productId, Hashtable parameters)
        {
            string endpoint = String.Format("{0}/products/{1}", api_endpoint, productId);
            return DoRequest<Product>(endpoint, "PUT", Utility.SerializeToJson(parameters));
        }

        //public Product AddProductToGroup(string productId, string groupId)
        //{
        //    string endpoint = "";
        //    return DoRequest<Product>
        //}

        public FitabaseCollection<Product> AllProducts()
        {
            string endpoint = String.Format("{0}/products", api_endpoint);
            return DoRequest<FitabaseCollection<Product>>(endpoint, "GET");
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

        public FitabaseCollection<Group> AllGroups()
        {
            string endpoint = String.Format("{0}/groups", api_endpoint);
            return DoRequest<FitabaseCollection<Group>>(endpoint, "GET");
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

        public FitabaseCollection<Subscription> AllSubscriptions()
        {
            string endpoint = String.Format("{0}/subscriptions", api_endpoint);
            return DoRequest<FitabaseCollection<Subscription>>(endpoint, "GET");
        }

        #endregion
    }
}
