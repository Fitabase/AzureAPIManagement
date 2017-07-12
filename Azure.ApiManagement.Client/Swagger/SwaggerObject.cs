using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerObject
    {
        [JsonProperty("swagger")]
        public string swagger { get; set; }
        [JsonProperty("info")]
        public SwaggerInfo info { get; set; }
        [JsonProperty("host")]
        public string host { get; set; }
        [JsonProperty("schemes")]
        public string[] schemes { get; set; }
        [JsonProperty("paths")]
        public Dictionary<string, PathData> paths { get; set; }
    }
}
