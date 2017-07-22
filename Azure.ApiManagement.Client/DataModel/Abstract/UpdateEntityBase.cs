using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections;

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
