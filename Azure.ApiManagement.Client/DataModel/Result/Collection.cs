using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class EntityCollection<T> where T : EntityBase
    {
        [JsonProperty("count")]
        internal int Count { get; set; }

        [JsonProperty("value")]
        internal List<T> Values { get; set; }

        [JsonProperty("nextLink")]
        internal string NextLink { get; set; }
    }
}
