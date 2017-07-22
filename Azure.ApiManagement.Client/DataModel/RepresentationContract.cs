using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// Operation response representation
    /// https://docs.microsoft.com/en-us/azure/templates/microsoft.apimanagement/service/apis/operations#template-format
    /// </summary>
    public class RepresentationContract
    {

        public static RepresentationContract Create(string contentType, string schemaId, 
                                                    string typeName, string sample, 
                                                    ParameterContract[] formParameters) {

            if (string.IsNullOrWhiteSpace(contentType))
                throw new InvalidEntityException("Representation's contentType is required");
            

            RepresentationContract representation = new RepresentationContract();
            representation.ContenType = contentType;
            representation.SchemaId = schemaId;
            representation.TypeName = typeName;
            representation.Sample = sample;

            // Condition to set form_parameter
            if (contentType.Equals("application/x-www-form-urlencoded") 
                || contentType.EndsWith("multipart/form-data"))
                representation.FormParameters = formParameters;

            return representation;
        }


        [JsonProperty("contentType")]
        public string ContenType { get; set; }

        [JsonProperty("sample")]
        public string Sample { get; set; }

        [JsonProperty("schemaId", NullValueHandling =NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        [JsonProperty("typeName", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeName { get; set; }

        [JsonProperty("formParameters", NullValueHandling = NullValueHandling.Ignore)]
        public ParameterContract[] FormParameters { get; set; }
    }
}
