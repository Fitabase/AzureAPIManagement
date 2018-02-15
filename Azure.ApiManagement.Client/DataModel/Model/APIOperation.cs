using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fitabase.Azure.ApiManagement.Model
{   

    public class APIOperationHelper
    {
        public APIOperation Operation;



        public APIOperationHelper(APIOperation operation)
        {
            this.Operation = operation;
        }

        public string GetOriginalURL()
        {
            string paramURL = GetParametersURL();
            if(String.IsNullOrEmpty(paramURL))
            {
                return Operation.UrlTemplate;
            }
            return Operation.UrlTemplate.Replace(GetParametersURL(), "");
        }
        
        public string GetParametersURL()
        {
            return BuildParametersURL(Operation.TemplateParameters);
        }

        public static string BuildParametersURL(ICollection<ParameterContract> parameters)
        {
            if (parameters == null) return "";
            StringBuilder builder = new StringBuilder();
            foreach (var param in parameters)
            {
                builder.Append("/").Append(param.Name)
                        .Append("/{").Append(param.Name).Append("}");
            }
            return builder.ToString();
        }
        public static string BuildURL(string url, ICollection<ParameterContract> parameters)
        {
            if (parameters == null) return url;

            string parameterURL = BuildParametersURL(parameters);
            StringBuilder builder = new StringBuilder(url);
            if(!url.Contains(parameterURL))
            {
                builder.Append(parameterURL);
            }
            return builder.ToString();
        }
    }

    public class APIOperation : EntityBase
    {
        protected override string UriIdFormat => "/operations";

        
        public static APIOperation Create(string name, 
                            RequestMethod method, string urlTemplate, 
                            ParameterContract[] parameters,
                            RequestContract request, ResponseContract[] responses, string description = null)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("Operation's is required");
            if (name.Length > 100)
                throw new InvalidEntityException("Length of operation's name must be < 100");
            if (String.IsNullOrWhiteSpace(urlTemplate))
                throw new InvalidEntityException("Operation's urlTemplate is required");
            
            
            APIOperation operation = new APIOperation();
            operation.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.APIOPERATION);
            operation.Name = name;
            operation.Method = method.ToString();
            operation.TemplateParameters = parameters;
            operation.Request = request;
            operation.Responses = responses;
            operation.Description = description;

            operation.UrlTemplate = urlTemplate;
            //operation.UrlTemplate = APIOperationHelper.BuildURL(urlTemplate, parameters);
            
            return operation;
        }


        public static APIOperation Create(string name,
                            string method, string urlTemplate,
                            ParameterContract[] parameters,
                            RequestContract request, ResponseContract[] responses, string description = null)
        {
            RequestMethod requestMethod;
            if (method == "GET") requestMethod = RequestMethod.GET;
            else if (method == "POST") requestMethod = RequestMethod.POST;
            else if (method == "PUT") requestMethod = RequestMethod.PUT;
            else if (method == "DELETE") requestMethod = RequestMethod.DELETE;
            else if (method == "PATCH") requestMethod = RequestMethod.PATCH;
            else throw new InvalidEntityException("Invalid operation's method");

            return Create(name, requestMethod, urlTemplate, parameters, request, responses, description);
        }


        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        
        [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }          // A Valid HTTP Operation Method.

        [JsonProperty("policies", NullValueHandling = NullValueHandling.Ignore)]
        public string Policies { get; set; }          // A Valid HTTP Operation Method.

        [JsonProperty("urlTemplate", NullValueHandling = NullValueHandling.Ignore)]
        public string UrlTemplate { get; set; }     // Relative URL template identifying the target resource for this operation
        
        [JsonProperty("templateParameters", NullValueHandling = NullValueHandling.Ignore)]
        public ParameterContract[] TemplateParameters { get; set; }  // Collection of URL template parameters. E.g calc.com/sum?a=5&b=10
        
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }         // Description of the operation. May include HTML formatting tags.
        
        [JsonProperty("request", NullValueHandling = NullValueHandling.Ignore)]
        public RequestContract Request { get; set; }    // An entity containing request details

        [JsonProperty("responses", NullValueHandling = NullValueHandling.Ignore)]
        public ResponseContract[] Responses { get; set; }

        public string GetOriginalPath()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < TemplateParameters.Length; i++)
            {
                string param = TemplateParameters[i].Name;
                builder.Append("/").Append(param)
                        .Append("/{").Append(param).Append("}");
            }
            string str = UrlTemplate.Replace(builder.ToString(), "");
            return str;
        }
    }
}
