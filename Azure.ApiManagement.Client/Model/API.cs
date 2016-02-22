using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class API
    {
        /// <summary>
        /// Resource identifier. Uniquely identifies the API within the current API Management service instance. 
        /// The value is a valid relative URL in the format of apis/{id} where {id} is an API identifier. This property is read-only.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the API. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the API. Must not be empty. May include HTML formatting tags. Maximum length is 1000 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Absolute URL of the backend service implementing this API.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Relative URL uniquely identifying this API and all of its resource paths within the API Management service instance.
        /// It is appended to the API endpoint base URL specified during the service instance creation to form a public URL for this API.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Describes on which protocols the operations in this API can be invoked.
        /// Allowed values are http, https, or both http and https.
        /// </summary>
        public IEnumerable<string> Protocols { get; set; }


        // authenticationSettings
        // subscriptionKeyParameterNames
    }
}
