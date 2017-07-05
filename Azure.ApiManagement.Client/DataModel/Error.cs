using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.DataModel
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Error
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "param")]
        public string Parameter { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string ErrorType { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
