using Newtonsoft.Json;
using System;


namespace Fitabase.Azure.ApiManagement.Model
{
    public class RequestHeader
    {
        public RequestHeader(string name, ParameterType type, string defaultValue = null, bool required = false)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("RequestHeader name is required");
            this.Name = name;
            this.Type = TemplateParameterType.GetType(type);
            this.DefaultValue = defaultValue;
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
