using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace AzureAPIManagement
{
    /// <summary>
    /// Wrapper class for Azure Api Management service. Mostly taken from here, but abstraced out of controller code:
    /// https://github.com/Azure/api-management-samples/blob/master/delegation/ContosoWebApplication/ContosoWebApplication/Controllers/AccountController.cs
    /// </summary>
    public class ApiManagementClient
    {
        string _ApimRestHost; // = "https://contosoinc.management.azure-api.net/";
        string _ApimRestId; // = "547f835d8b70eb0628030003";
        string _ApimRestPK; // = "0NfbTL7h+L8C80LzBmlNrF61x8CvxKPCnqZzYMTuljhIsWfApIQlWTD4M5KioZsZx/1F8aI0XGHyX2ALNpV8Ow==";
        System.DateTime ApimRestExpiry = DateTime.UtcNow.AddDays(10);
        string _ApimRestApiVersion = "2014-02-14-preview";
        string _delegationValidationKey;
        public ApiManagementClient(string apimRestHost, string apimRestId, string apimRestKey, string delegationValidationKey)
        {
            _ApimRestHost = apimRestHost;
            _ApimRestId = apimRestId;
            _ApimRestPK = apimRestKey;
            _delegationValidationKey = delegationValidationKey;
        }



        #region Helper Methods

        private static string SerializeToJson<TAnything>(TAnything value)
        {
            var settings = new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc };
            var formatting = Formatting.None;
            var writer = new StringWriter();

            var serializer = JsonSerializer.Create(settings);
            var jsonWriter = new JsonTextWriter(writer) { Formatting = formatting };

            serializer.Serialize(jsonWriter, value);
            return writer.GetStringBuilder().ToString();
        }


        private static TAnything DeserializeToJson<TAnything>(String value)
        {
            var settings = new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc };
            var reader = new StringReader(value);

            var serializer = JsonSerializer.Create(settings);
            var jsonReader = new JsonTextReader(reader);

            return (TAnything)serializer.Deserialize(jsonReader, typeof(TAnything));
        }


        private string ApimRestAuthHeader()
        {
            using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(_ApimRestPK)))
            {
                var dataToSign = _ApimRestId + "\n" + ApimRestExpiry.ToString("O", CultureInfo.InvariantCulture);
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                var signature = Convert.ToBase64String(hash);
                var encodedToken = string.Format("SharedAccessSignature uid={0}&ex={1:o}&sn={2}", _ApimRestId, ApimRestExpiry, signature);
                return encodedToken;
            }
        }

        #endregion

        public string GetRequestOperationSignature(string operation, string salt, string returnUrl = null, string productId = null, string userId = null, string subscriptionId = null)
        {
            string key = _delegationValidationKey;
            var encoder = new HMACSHA512(Convert.FromBase64String(key));
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

        public async Task<SsoUrl> LoginUserWithSsoUrlAsync(string userId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

                HttpResponseMessage response = await client.PostAsync("/users/" + userId + "/generateSsoUrl?api-version=" + _ApimRestApiVersion, new StringContent("", Encoding.UTF8, "text/json"));
                string httpContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    //We got an SSO token - redirect
                    //HttpContent receiveStream = response.Content;
                    //var SsoUrlJson = await receiveStream.ReadAsStringAsync();
                    SsoUrl su = DeserializeToJson<SsoUrl>(httpContent);
                    return su;
                }
                else
                {
                    throw new HttpException((int)response.StatusCode, httpContent);
                }

            }
        }

        public async Task<bool> CreateUserAsync(string userId, string userFirstName, string userLastName, string userEmail, string userPassword)
        {
            //create user in APIM as well
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

                var ApimUser = new
                {
                    firstName = userFirstName,
                    lastName = userLastName,
                    email = userEmail,
                    password = userPassword,
                    state = "active"
                };

                var ApimUserJson = SerializeToJson(ApimUser);

                HttpResponseMessage response = await client.PutAsync("/users/" + userId + "?api-version=" + _ApimRestApiVersion, new StringContent(ApimUserJson, Encoding.UTF8, "text/json"));
                string httpContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    //User created successfully

                    return true;
                }
                else
                {
                    throw new HttpException((int)response.StatusCode, httpContent);
                }
            }
        }

        public async Task<bool> AddProductSubscriptionAsync(string userId, string productId, string createSubscriptionId)
        {
            //Register user for product
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

                HttpResponseMessage response;

                var ApimSubscription = new
                {
                    userId = "/users/" + userId,
                    productId = "/products/" + productId,
                    state = "active"
                };

                var ApimSubscriptionJson = SerializeToJson(ApimSubscription);

                //Guid subscriptionId = Guid.NewGuid();

                response = await client.PutAsync("/subscriptions/" + createSubscriptionId + "?api-version=" + _ApimRestApiVersion, new StringContent(ApimSubscriptionJson, Encoding.UTF8, "text/json"));
                string httpContent = await response.Content.ReadAsStringAsync();
                
                
                if (response.IsSuccessStatusCode)
                {
                    //Subscription created

                    //return Redirect(Request.QueryString["returnUrl"]);
                    return true; //Redirect("https://contosoinc.portal.azure-api.net/developer");
                }
                else
                {
                    throw new HttpException((int)response.StatusCode, httpContent);
                }
            }

        }

        public async Task<bool> RemoveProductSubscriptionAsync(string userId, string subscriptionId)
        {
            //Register user for product
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

                HttpResponseMessage response;
                
                client.DefaultRequestHeaders.Add("If-Match", "*");
                response = await client.DeleteAsync("/subscriptions/" + subscriptionId + "?api-version=" + _ApimRestApiVersion);
                

                if (response.IsSuccessStatusCode)
                {
                    //Subscription created

                    //return Redirect(Request.QueryString["returnUrl"]);
                    return true; //return Redirect("https://contosoinc.portal.azure-api.net/developer");
                }
                else
                {
                    throw new HttpException((int)response.StatusCode, await response.RequestMessage.Content.ReadAsStringAsync());
                }
                
            }
        }





    }
}
