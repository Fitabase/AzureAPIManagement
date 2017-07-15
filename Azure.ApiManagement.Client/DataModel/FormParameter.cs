using Newtonsoft.Json;
using System;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class FormParameter
    {
        public FormParameter(string name, ParameterType type)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("FormParameter name is required");
            this.Name = name;
            this.Type = type;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public ParameterType Type { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }
    }

}
