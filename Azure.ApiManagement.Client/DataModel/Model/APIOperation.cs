using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{   
    public class APIOperation : EntityBase
    {
        protected override string UriIdFormat => "/operations";

        
        public static APIOperation Create(string name, 
                            RequestMethod method, string urlTemplate, 
                            List<TemplateParameter> parameters,
                            RequestContract request)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("APIOperation name is required");
            if (String.IsNullOrWhiteSpace(urlTemplate))
                throw new InvalidEntityException("APIOperation urlTemplate is required");

            APIOperation api = new APIOperation();
            api.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.APIOPERATION);
            api.Name = name;
            api.Method = method.ToString();
            api.UrlTemplate = urlTemplate;
            api.TemplateParameter = parameters;
            api.Request = request;

            return api;
        }

        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("method")]
        public string Method { get; set; }          // A Valid HTTP Operation Method.

        [JsonProperty("urlTemplate")]
        public string UrlTemplate { get; set; }     // Relative URL template identifying the target resource for this operation
        
        [JsonProperty("templateParameters")]
        public List<TemplateParameter> TemplateParameter { get; set; }  // Collection of URL template parameters. E.g calc.com/sum?a=5&b=10
        
        [JsonProperty("description")]
        public string Description { get; set; }         // Description of the operation. May include HTML formatting tags.
        
        [JsonProperty("request")]
        public RequestContract Request { get; set; }    // An entity containing request details

        [JsonProperty("responses")]
        public OperationResponse[] Responses { get; set; }

        
    }











}
