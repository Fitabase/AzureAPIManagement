using Newtonsoft.Json;
using System;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// TODO not so sure what query parameter does...
    /// </summary>
    public class QueryParameter
    {
        public QueryParameter(string name, ParameterType type, bool required = false)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("QueryParameter name is required");
            this.Name = name;
            this.Type = TemplateParameterType.GetType(type);
            this.Required = required;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }
    }

}
