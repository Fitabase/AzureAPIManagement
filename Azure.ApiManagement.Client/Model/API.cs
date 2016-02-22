using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class API : EntityBase
    {
        protected override string UriIdFormat {  get { return "apis/"; } }

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
        public IEnumerable<string> Protocols { get; set; }

       

        // authenticationSettings
        // subscriptionKeyParameterNames
    }
}
