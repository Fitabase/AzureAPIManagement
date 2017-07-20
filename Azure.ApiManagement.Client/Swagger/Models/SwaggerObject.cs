using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{
    /// <summary>
    /// This class is the root of Swagger Schema.
    /// 
    /// https://swagger.io/specification/
    /// </summary>
    public class SwaggerObject
    {

        [JsonProperty("swagger", NullValueHandling = NullValueHandling.Ignore)]
        public string Swagger { get; set; }         // Specifies the Swagger specification version being userd
        
        [JsonProperty("info", NullValueHandling = NullValueHandling.Ignore)]
        public Info Info { get; set; }              // Provides metada about the api

        [JsonProperty("host", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }            // The host (name or ip) serving the API

        [JsonProperty("basePath", NullValueHandling = NullValueHandling.Ignore)]
        public string BasePath { get; set; }        // The base path on which the API is served, which is realtive to the host.

        [JsonProperty("schemes", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Schemes { get; set; }       // The transfer protocol of the API. 

        [JsonProperty("paths", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, PathData> Paths { get; set; } // The available paths and perations for the API

        [JsonProperty("definitions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Schema> Definitions { get; set; }
        
        [JsonProperty("securityDefinitions", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, SecurityDefinitions> SecurityDefinitions { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public Tag[] Tags { get; set; }

        [JsonProperty("externalDocs", NullValueHandling = NullValueHandling.Ignore)]
        public ExternalDoc ExternalDocs { get; set; }
    }
    
}
