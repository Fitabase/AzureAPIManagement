using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public abstract class EntityBase
    {
        protected abstract string UriIdFormat { get; }
        private string _id;
        [JsonIgnore]
        public string Id
        {
            get
            {
                if (String.IsNullOrEmpty(_id))
                {

                    if (!String.IsNullOrEmpty(this.Uri))
                    {

                        if (this.Uri.StartsWith(this.UriIdFormat, StringComparison.InvariantCultureIgnoreCase))
                            _id = this.Uri.Substring(UriIdFormat.Length);
                    }
                }

                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Resource identifier. Uniquely identifies the entity within the current API Management service instance. 
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }

        public string GetPlainId()
        {
            string[] splits = Uri.Split('/');
            return splits[splits.Length - 1];
        }

    }
}
