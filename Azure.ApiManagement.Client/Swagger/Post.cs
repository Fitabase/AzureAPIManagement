using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{

    public class Post
    {
        [JsonProperty("tags")]
        public string[] tags { get; set; }

        [JsonProperty("operationId")]
        public string operationId { get; set; }

        [JsonProperty("consumes")]
        public string[] consumes { get; set; }

        [JsonProperty("produces")]
        public string[] produces { get; set; }
        
        public Dictionary<string, SwaggerResponse> responses { get; set; }
    }

}
