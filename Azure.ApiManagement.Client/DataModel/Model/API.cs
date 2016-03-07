using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class API : EntityBase
    {
        protected override string UriIdFormat {  get { return "/apis/"; } }

        /// <summary>
        /// Name of the entity. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the entity. Must not be empty. May include HTML formatting tags. Maximum length is 1000 characters.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Absolute URL of the backend service implementing this API.
        /// </summary>
        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Relative URL uniquely identifying this API and all of its resource paths within the API Management service instance.
        /// It is appended to the API endpoint base URL specified during the service instance creation to form a public URL for this API.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Describes on which protocols the operations in this API can be invoked.
        /// Allowed values are http, https, or both http and https.
        /// </summary>
        [JsonProperty("protocols")]
        public List<string> Protocols { get; set; }

        /// <summary>
        /// Collection of authentication settings included in this API.
        /// </summary>
        [JsonProperty("authenticationSettings")]
        public AuthenticationSettings Authentication { get; set; }

        /// <summary>
        /// Optional property that can be used to specify custom names for query and/or header parameters containing the subscription key.
        /// When this property is present it must contain at least one of the two following properties.
        /// </summary>
        [JsonProperty("subscriptionKeyParameterNames")]
        public SubscriptionKeyParameterNames CustomNames { get; set; }

    }

    public class AuthenticationSettings
    {
        /// <summary>
        /// OAuth authorization server identifier. Corresponds to the id of one of the Authorization Server entities in the current service instance.
        /// </summary>
        [JsonProperty("authorizationServerId")]
        public string ServerId { get; set; }

        /// <summary>
        /// Operations scope. This property is optional. If present, it overrides the default scope from the authorization server.
        /// </summary>
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
