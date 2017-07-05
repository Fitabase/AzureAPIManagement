using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{

    [JsonObject(MemberSerialization.OptIn)]
    public class FitabaseCollection<T>
    {
        
        [JsonProperty("count")]
        public int Count { get; set; }              // Total item in then collection
        
        [JsonProperty("value")]
        public List<T> Items { get; set; }
        
    }
}
