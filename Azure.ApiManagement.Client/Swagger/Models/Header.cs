using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    public class Headers {
        Dictionary<string, Header> Header;
    }

    public class Header
    {
        [JsonProperty("description")]
        public string Description { get; set; } // A short description of the header

        [JsonProperty("type")]
        public string Type { get; set; }        // The type of the parameter.

        [JsonProperty("format")]
        public string Format { get; set; }      // 	The extending format for the previously mentioned type.

        [JsonProperty("items")]
        public Item Item { get; set; }              // Describes the type of items in the array.

        [JsonProperty("collectionFormat")]
        public string CollectionFormat { get; set; }    // 	Determines the format of the array if type array is used. 

        [JsonProperty("default")]
        public string Default { get; set; }         // 	Declares the value of the parameter that the server will use if none is provided

        [JsonProperty("maximum")]
        public int Maximum { get; set; }

        [JsonProperty("exclusiveMaximum")]
        public bool ExclusiveMaximum { get; set; }

        [JsonProperty("minimum")]
        public int Minimum { get; set; }

        [JsonProperty("exclusiveMinimum")]
        public bool ExclusiveMinimum { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }

        [JsonProperty("minLength")]
        public int MinLength { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("maxItems")]
        public int MaxItems { get; set; }

        [JsonProperty("minItems")]
        public int MinItems { get; set; }

        [JsonProperty("uniqueItems")]
        public bool UniqueItems { get; set; }

        [JsonProperty("multipleOf")]
        public string MultipleOf { get; set; }
    }
}
