using Newtonsoft.Json;
using System;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// This class represent a URL template parameter.
    /// E.x calc.com/sum?a=5&b=10
    /// </summary>
    public class ParameterContract
    {
        
        
        public static ParameterContract Create(string name, string type, 
                                                string description = null, 
                                                string defaultValue = null, 
                                                bool? required = null, 
                                                string[] values = null)
        {

            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("ParameterContract's name is required");
            if (String.IsNullOrWhiteSpace(type))
                throw new ArgumentException("ParameterContract's type is required");

            ParameterContract entity = new ParameterContract();
            entity.Name = name;
            entity.Type = type;
            entity.Description = description;
            entity.DefaultValue = defaultValue;
            entity.Required = required;
            entity.Values = values;
            return entity;
        }


        [JsonProperty("name")]
        public string Name { get; set; }                // Parameter name.

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }         // Parameter description.


        [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultValue { get; set; }        // Default parameter value.


        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Required { get; set; }

        [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Values { get; set; }

    }
}
