using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// This class contains metadata about the API. This metadata can be used
    /// by the clients if needed.
    /// </summary>
    public class Info
    {
        [JsonProperty("version")]
        public string Version { get; set; }         // Provides the version of the application API
        [JsonProperty("title")]
        public string Title { get; set; }           // The title of the application.
        [JsonProperty("description")]
        public string Description { get; set; }     // A short description of the application. 
        [JsonProperty("termsOfService")]
        public string TermsOfService { get; set; }  // The Terms of Service for the API.
        [JsonProperty("Contact")]
        public Contact Contact { get; set; }        // The contact information for the exposed API.
        [JsonProperty("License")]
        public License License { get; set; }        // The license information for the exposed API

    }
}
