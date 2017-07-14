using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// Allows referencing an external resource for extended documentation.
    /// </summary>
    public class ExternalDocs
    {
        [JsonProperty("description")]
        public string Description { get; set; }     // A short description of the target documentation. 
        [JsonProperty("url")]
        public string Url { get; set; }             // The URL for the target documentation.
    }
}
