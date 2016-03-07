using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// This class represents a collection of service entities
    /// https://msdn.microsoft.com/en-us/library/azure/dn776332.aspx#collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EntityCollection<T> where T : EntityBase
    {
        /// <summary>
        /// The total number of elements in the collection.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Contains a collection of items included in this response.
        /// </summary>
        [JsonProperty("value")]
        public List<T> Values { get; set; }

        /// <summary>
        /// The absolute url to the remaining items in the collection.
        /// </summary>
        [JsonProperty("nextLink")]
        public string NextLink { get; set; }
    }
}
