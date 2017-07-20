using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{


    /// <summary>
    /// A metadata object that allows for more fine-tuned XML model definitions.
    /// 
    /// When using arrays, XML element names are not inferred(for singular/plural forms) 
    /// and the name property should be used to add that information.See examples for 
    /// expected behavior.
    /// </summary>
    public class XML
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }    // 	Replaces the name of the element/attribute used for the described schema property.

        [JsonProperty("namespace", NullValueHandling = NullValueHandling.Ignore)]
        public string Namespace { get; set; }   // The URL of the namespace definition. Value SHOULD be in the form of a URL.

        [JsonProperty("prefix", NullValueHandling = NullValueHandling.Ignore)]
        public string Prefix { get; set; }      // The prefix to be used for the name.

        [JsonProperty("attribute", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Attribute { get; set; }     // Declares whether the property definition translates to an attribute instead of an element.

        [JsonProperty("wrapped", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Wrapped { get; set; }       // MAY be used only for an array definition. 
    }
}
