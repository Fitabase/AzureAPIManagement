using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// This class represents an api of Azure API Management.
    /// </summary>
    public class API : EntityBase
    {
        protected override string UriIdFormat {  get { return "/apis/"; } }
       
        public API(string name, string description, 
                   string serviceUrl, string path, 
                   List<String> protocols, 
                   AuthenticationSettings authentication = null, 
                   SubscriptionKeyParameterNames customNames = null) : base("API")
        {
            this.Name = name;
            this.Description = description;
            this.ServiceUrl = serviceUrl;
            this.Path = path;
            this.Protocols = protocols;
            this.Authentication = authentication;
            this.CustomNames = customNames;
        }


        
        [JsonProperty("name")]
        public string Name { get; set; }                

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }      // Absolute URL of the backend service implementing this API.


        // Relative URL uniquely identifying this API and all of its 
        // resource paths within the API Management service instance.
        // It is appended to the API endpoint base URL specified during 
        // the service instance creation to form a public URL for this API.
        [JsonProperty("path")]
        public string Path { get; set; }
        
        [JsonProperty("protocols")]
        public List<string> Protocols { get; set; }             // Describes on which protocols the operations in this API can be invoked.
        

        [JsonProperty("authenticationSettings")]
        public AuthenticationSettings Authentication { get; set; }  // Collection of authentication settings included in this API.

        // Optional property that can be used to specify custom names for 
        // query and/or header parameters containing the subscription key.
        // When this property is present it must contain at least one of 
        // the two following properties.
        [JsonProperty("subscriptionKeyParameterNames")]
        public SubscriptionKeyParameterNames CustomNames { get; set; }

        [JsonIgnore]
        public ICollection<APIOperation> Operations { get; set; }


        [JsonProperty("parameters")]
        public ApiContract[] Parameters { get; set; }

    }


    /// <summary>
    /// Parameter that is included in the body of the API
    /// </summary>
    public class ApiContract
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("protocols")]
        public string[] Protocols { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("authenticationSettings")]
        public AuthenticationSettings AuthenticationSettings { get; set; }

        [JsonProperty("subscriptionKeyParameterNames")]
        public SubscriptionKeyParameterNames SubscriptionKeyParameterNames { get; set; }
    }

    public class AuthenticationSettings
    {
        [JsonProperty("authorizationServerId")]
        public string ServerId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }

    public class SubscriptionKeyParameterNames
    {
        [JsonProperty("header")]
        public string Header { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }
    }
}
