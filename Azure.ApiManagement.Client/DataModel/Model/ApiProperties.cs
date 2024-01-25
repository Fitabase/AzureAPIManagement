using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class ApiProperties
    {
        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("apiRevision", NullValueHandling = NullValueHandling.Ignore)]
        public string ApiRevision { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("serviceUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceUrl { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("protocols", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Protocols { get; set; }

        [JsonProperty("isCurrent", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsCurrent { get; set; }
    }
}
