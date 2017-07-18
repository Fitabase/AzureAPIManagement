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

        
        public static API Create(string name, string description, 
                   string serviceUrl, string path, 
                   string[] protocols, 
                   AuthenticationSettingsConstract authentication = null, 
                   SubscriptionKeyParameterNames customNames = null) 
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("API name is required");
            if (String.IsNullOrWhiteSpace(serviceUrl))
                throw new InvalidEntityException("API service url is required");
            if (String.IsNullOrWhiteSpace(path))
                throw new InvalidEntityException("API path is required");

            API api = new API();
            api.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.API);
            api.Name = name;
            api.Description = description;
            api.ServiceUrl = serviceUrl;
            api.Path = path.ToLower();
            api.Protocols = protocols;
            api.Authentication = authentication;
            api.CustomNames = customNames;

            return api;
        }

        
        [JsonProperty("name")]
        public string Name { get; set; }                

        [JsonProperty("apiRevision")]
        public string ApiRevision { get; set; }         // Describes the Revision of the Api. If no value is provided, default revision 1 is created

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
        public string[] Protocols { get; set; }             // Describes on which protocols the operations in this API can be invoked.
        

        [JsonProperty("authenticationSettings")]
        public AuthenticationSettingsConstract Authentication { get; set; }  // Collection of authentication settings included in this API.

        // Optional property that can be used to specify custom names for 
        // query and/or header parameters containing the subscription key.
        // When this property is present it must contain at least one of 
        // the two following properties.
        [JsonProperty("subscriptionKeyParameterNames")]
        public SubscriptionKeyParameterNames CustomNames { get; set; }

        [JsonIgnore]
        public ICollection<APIOperation> Operations { get; set; }


        [JsonProperty("isCurrent")]
        public bool IsCurrent { get; set; }         // Indicates if API revision is current api revision.


        [JsonProperty("isOnline")]
        public bool IsOnline { get; set; }          // Indicates if API revision is accessible via the gateway.

    }

    
    

    public class AuthenticationSettingsConstract
    {
        [JsonProperty("oAuth2")]
        public OAuth2AuthenticationSettingsContract OAuth2 { get; set; }

        [JsonProperty("openid")]
        public string OpenId { get; set; }
    }
    
    public class OAuth2AuthenticationSettingsContract
    {
        [JsonProperty("authorizationServerId")]
        public string AuthorizationServerId { get; set; }

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
