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
        public EntityBase(string prefixId)
        {
            PrintMessage.Debug(this.GetType().Name, _id);
            if (String.IsNullOrWhiteSpace(_id))
                this.Id = GenerateIdSignature(prefixId);
        }

        private string GenerateIdSignature(string prefixId)
        {
            return new StringBuilder()
                        .Append(prefixId).Append("_")
                        .Append(Guid.NewGuid().ToString("N"))
                        .ToString();
        }

        protected abstract string UriIdFormat { get; }

        public string Id { get; set; }
        private string _id;
        //[JsonIgnore]
        //public string Id
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(_id))
        //        {

        //            if (!String.IsNullOrEmpty(this.Uri))
        //            {

        //                if (this.Uri.StartsWith(this.UriIdFormat, StringComparison.InvariantCultureIgnoreCase))
        //                    _id = this.Uri.Substring(UriIdFormat.Length);
        //            }
        //        }

        //        return _id;
        //    }
        //    set
        //    {
        //        _id = value;
        //    }
        //}
    
        /// <summary>
        /// Resource identifier. Uniquely identifies the entity within the current API Management service instance. 
        /// </summary>
        //[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        //public string Uri { get; set; }
        
        
    }
}
