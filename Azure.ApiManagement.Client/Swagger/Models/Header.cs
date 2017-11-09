using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{ 

    public class Header
    {
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }         // A short description of the header

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }                // The type of the parameter.

        [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
        public string Format { get; set; }              // 	The extending format for the previously mentioned type.

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public Item Item { get; set; }                  // Describes the type of items in the array.

        [JsonProperty("collectionFormat", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionFormat { get; set; }    // 	Determines the format of the array if type array is used. 

        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public string Default { get; set; }             // 	Declares the value of the parameter that the server will use if none is provided

        [JsonProperty("maximum", NullValueHandling = NullValueHandling.Ignore)]
        public int? Maximum { get; set; }

        [JsonProperty("exclusiveMaximum", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExclusiveMaximum { get; set; }

        [JsonProperty("minimum", NullValueHandling = NullValueHandling.Ignore)]
        public int? Minimum { get; set; }

        [JsonProperty("exclusiveMinimum", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExclusiveMinimum { get; set; }

        [JsonProperty("maxLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength { get; set; }

        [JsonProperty("minLength", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength { get; set; }

        [JsonProperty("pattern", NullValueHandling = NullValueHandling.Ignore)]
        public string Pattern { get; set; }

        [JsonProperty("maxItems", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxItems { get; set; }

        [JsonProperty("minItems", NullValueHandling = NullValueHandling.Ignore)]
        public int? MinItems { get; set; }

        [JsonProperty("uniqueItems", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UniqueItems { get; set; }

        [JsonProperty("multipleOf", NullValueHandling = NullValueHandling.Ignore)]
        public string MultipleOf { get; set; }
    }
}
