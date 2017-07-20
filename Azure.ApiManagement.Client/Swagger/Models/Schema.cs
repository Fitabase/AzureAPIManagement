using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger.Models
{
    public class SchemaDictionary
    {
        public string Name { get; set; }
        public Schema Schema { get; set; }

        public SchemaDictionary(string name, Schema schema)
        {
            this.Name = name;
            this.Schema = schema;
        }
    }

    /// <summary>
    /// The Schema Object allows the definition of input and output data types. 
    /// These types can be objects, but also primitives and arrays. This object 
    /// is based on the JSON Schema Specification Draft 4 and uses a predefined 
    /// subset of it. On top of this subset, there are extensions provided by 
    /// this specification to allow for more complete documentation.
    /// </summary>
    public class Schema
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("properties", NullValueHandling=NullValueHandling.Ignore)]
        public Dictionary<string, Property> Properties { get; set; }

        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Required { get; set; }

        [JsonProperty("discriminator", NullValueHandling = NullValueHandling.Ignore)]
        public string Discriminator { get; set; }       // 	Adds support for polymorphism

        [JsonProperty("readOnly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReadOnly { get; set; }              // Relevant only for Schema "properties" definitions.

        [JsonProperty("externalDocs", NullValueHandling = NullValueHandling.Ignore)]
        public ExternalDoc ExternalDocs { get; set; }  // Additional external documentation for this schema.

        [JsonProperty("xml", NullValueHandling = NullValueHandling.Ignore)]
        public XML Xml { get; set; }                    // This MAY be used only on properties schemas.
        

        public HashSet<PropertyDictionary> GetProperyDictionary()
        {
            HashSet<PropertyDictionary> set = new HashSet<PropertyDictionary>();
            foreach (KeyValuePair<string, Property> entry in Properties)
            {
                set.Add(new PropertyDictionary(entry.Key, entry.Value));
            }
            return set;
        }

    }
    


}
