using Newtonsoft.Json;
using System;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// This class represent a URL template parameter.
    /// E.x calc.com/sum?a=5&b=10
    /// </summary>
    public class TemplateParameter
    {
        public TemplateParameter(string name, ParameterType type)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("TemplateParameter name is required");
            this.Name = name;
            this.Type = TemplateParameterType.GetType(type);
        }

        [JsonProperty("name")]
        public string Name { get; set; }                // Parameter name.

        [JsonProperty("description")]
        public string Description { get; set; }         // Parameter description.

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }        // Default parameter value.


        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("values")]
        public string[] Values { get; set; }

    }
}
