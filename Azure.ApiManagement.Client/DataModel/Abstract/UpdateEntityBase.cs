using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public abstract class UpdateEntityBase
    {
        public UpdateEntityBase(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
                throw new InvalidEntityException("Entity ID is required");
            this.Id = id;
        }
        
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        public abstract Hashtable GetUpdateProperties();
    }
}
