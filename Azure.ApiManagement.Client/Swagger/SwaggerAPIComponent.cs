using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// Swagger component object
    /// 
    /// https://swagger.io/specification/
    /// </summary>
    public class SwaggerAPIComponent
    {
        [JsonProperty("swagger")]
        public string Swagger { get; set; }         // Specifies the Swagger specification version being userd

        [JsonProperty("info")]
        public Info Info { get; set; }       // Provides metada about the api

        [JsonProperty("host")]
        public string Host { get; set; }            // The host (name or ip) serving the API

        [JsonProperty("basePath")]
        public string BasePath { get; set; }        // The base path on which the API is served, which is realtive to the host.

        [JsonProperty("schemes")]
        public string[] Schemes { get; set; }       // The transfer protocol of the API. 

        //[JsonProperty("consumes")]
        //public string[] Consumes { get; set; }      // A list of MIME types the APIS can consume

        //[JsonProperty("produces")]
        //public string[] Produces { get; set; }      // A list of MIME types the APIS can produce

        [JsonProperty("paths")]
        public Dictionary<string, PathData> Paths { get; set; } // The available paths and perations for the API

        [JsonProperty("definitions")]
        public DefinitionCollection Definitions { get; set; }   // An object to hold data types produced and consumed by operations
        

        
    }




    /// <summary>
    /// This class contains definitions
    /// </summary>
    public class DefinitionCollection
    {
        private List<string> Definitions { get; set; }

        public string GetDefinition()
        {
            if (Definitions == null) return "";

            StringBuilder builder = new StringBuilder();
            foreach (string str in this.Definitions)
                builder.Append(str).Append(" ");

            return builder.ToString();
        }
    }



















}
