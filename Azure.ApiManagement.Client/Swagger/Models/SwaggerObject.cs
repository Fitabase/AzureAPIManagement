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
        [JsonProperty("swagger_", NullValueHandling = NullValueHandling.Ignore)]
        public string Swagger { get; set; }         // Specifies the Swagger specification version being userd
        
        [JsonProperty("info_", NullValueHandling = NullValueHandling.Ignore)]
        public Info Info { get; set; }              // Provides metada about the api

        [JsonProperty("host_", NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }            // The host (name or ip) serving the API

        [JsonProperty("basePath_", NullValueHandling = NullValueHandling.Ignore)]
        public string BasePath { get; set; }        // The base path on which the API is served, which is realtive to the host.

        [JsonProperty("schemes_", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Schemes { get; set; }       // The transfer protocol of the API. 

        [JsonProperty("paths", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, PathData> Paths { get; set; } // The available paths and perations for the API

        [JsonProperty("definitions_", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Schema> Definitions { get; set; }
        //public DefinitionCollection Definitions { get; set; }   // An object to hold data types produced and consumed by operations
        
        [JsonProperty("securityDefinitions_", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, SecurityDefinitions> SecurityDefinitions { get; set; }

        
        [JsonProperty("tags_", NullValueHandling = NullValueHandling.Ignore)]
        public Tag[] Tags { get; set; }

        [JsonProperty("externalDocs_", NullValueHandling = NullValueHandling.Ignore)]
        public ExternalDoc ExternalDocs { get; set; }



        public HashSet<SchemaDictionary> GetDefinitions()
        {
            HashSet<SchemaDictionary> set = new HashSet<SchemaDictionary>();

            foreach (KeyValuePair<string, Schema> entry in Definitions)
            {
                set.Add(new SchemaDictionary(entry.Key, entry.Value));
            }
            return set;
        }
        
    }
}
