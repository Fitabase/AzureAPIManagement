using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// An object to hold parameters that can be used across operations. 
    /// This property does not define global parameters for all operations.
    /// </summary>
    public class Parameter
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }        // The name of the parameter. Parameter names are case sensitive.

        [JsonProperty("in", NullValueHandling = NullValueHandling.Ignore)]
        public string In { get; set; }          // The location of the parameter. Possible values are "query", "header", "path", "formData" or "body".

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } // 	A brief description of the parameter. 

        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public bool Required { get; set; }      // 	Determines whether this parameter is mandatory.



        #region In VALUE IS 'body'

        [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
        public Schema Schema { get; set; }      // The schema defining the type used for the body parameter.

        #endregion



        #region In VALUE IS NOT 'body'

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }        // The type of the parameter.

        [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
        public string Format { get; set; }      // 	The extending format for the previously mentioned type.

        [JsonProperty("allowEmptyValue", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowEmptyValue { get; set; } // Sets the ability to pass empty-valued parameters.

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public Item Item { get; set; }              // Describes the type of items in the array.

        [JsonProperty("collectionFormat", NullValueHandling = NullValueHandling.Ignore)]
        public string CollectionFormat { get; set; }    // 	Determines the format of the array if type array is used. 

        [JsonProperty("default", NullValueHandling = NullValueHandling.Ignore)]
        public string Default { get; set; }         // 	Declares the value of the parameter that the server will use if none is provided

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
        #endregion
    }
}
