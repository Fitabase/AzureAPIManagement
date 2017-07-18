using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ErrorResponse
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "param")]
        public string Parameter { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public DetailError[] Details { get; set; }
    }

    internal class DetailError
    {

        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
        
        [JsonProperty(PropertyName = "target")]
        public string Target { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}
