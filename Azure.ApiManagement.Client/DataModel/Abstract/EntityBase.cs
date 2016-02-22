using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public abstract class EntityBase
    {
        protected abstract string UriIdFormat { get; }

        [JsonIgnore]
        public string Id
        {
            get
            {
                if (this.Uri.StartsWith(this.UriIdFormat, StringComparison.InvariantCultureIgnoreCase))
                    return this.Uri.Substring(UriIdFormat.Length);

                return null;
            }
            set
            {
                //TODO
            }
        }

        #region Common properties

        /// <summary>
        /// Resource identifier. Uniquely identifies the product within the current API Management service instance. 
        /// </summary>
        [JsonProperty("id")]
        public string Uri { get; set; }

        /// <summary>
        /// Name of the product. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the entity. Must not be empty. May include HTML formatting tags. Maximum length is 1000 characters.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        #endregion
    }
}
