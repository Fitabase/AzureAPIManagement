using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// A list of tags used by the specification with additional metadata. 
    /// The order of the tags can be used to reflect on their order by the 
    /// parsing tools. Not all tags that are used by the Operation Object 
    /// must be declared. The tags that are not declared may be organized 
    /// randomly or based on the tools' logic. Each tag name in the list 
    /// MUST be unique.
    /// </summary>
    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }                // The name of the tag.

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }         // 	A short description for the tag.

        [JsonProperty("externalDocs", NullValueHandling = NullValueHandling.Ignore)]
        public ExternalDoc ExternalDocs { get; set; }  // Additional external documentation for this tag.
    }
}
