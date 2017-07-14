using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// This class is used to compose a Swagger object to a API object
    /// </summary>
    class APIComposer
    {
        public SwaggerAPIComponent Swagger;

        public APIComposer(SwaggerAPIComponent swagger)
        {
            if (swagger == null)
            {
                throw new ArgumentException("SwaggerObject is required");
            }

            this.Swagger = swagger;
        }
        

        /// <summary>
        /// Compose a SwaggerObject to a API 
        /// </summary>
        /// <returns></returns>
        public API Compose()
        {
            string name             = Swagger.Info.Title;               
            string description      = Swagger.Definitions.GetDefinition();
            string serviceUrl       = Swagger.Host;
            string path = null;
            List<string> protocols  = Swagger.Schemes.ToList();
            AuthenticationSettings authentication       = null;
            SubscriptionKeyParameterNames customNames   = null;
            
            API api = new API(name, description, serviceUrl, path,
                               protocols, authentication, customNames);
            api.Operations = GetOperations(Swagger.Paths);      // Inject api operations


            return api;
        }


        /// <summary>
        /// Compose Swagger response to API operation response
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        private APIOperation.OperationResponse[] GetResponse(
            Dictionary<string, Response> responses)
        {
            List<APIOperation.OperationResponse> list = new List<APIOperation.OperationResponse>();
            foreach (KeyValuePair<string, Response> response in responses)
            {
                var code = response.Key;
                var description = response.Value.Description;
                list.Add(new APIOperation.OperationResponse(Int32.Parse(code), description));
                    
            }

            return list.ToArray();
        }
        

        /// <summary>
        /// Compose a list of API Operations
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private List<APIOperation> GetOperations(Dictionary<string, PathData> paths)
        {
            List<APIOperation> operations = new List<APIOperation>();
            foreach (KeyValuePair<string, PathData> p in paths)
            {
                APIOperation apiOperation = GetOperation(p);
                if (p.Value.Post != null) {
                    apiOperation.Responses = GetResponse(p.Value.Post.Responses);
                } else if(p.Value.Get != null) {
                    apiOperation.Responses = GetResponse(p.Value.Get.Responses);
                }
                operations.Add(apiOperation);
            }
            return operations;
        }

        /// <summary>
        /// Compose swagger path to API operation
        /// </summary>
        /// <param name="pathDictionary"></param>
        /// <returns></returns>
        private APIOperation GetOperation(KeyValuePair<string, PathData> pathDictionary)
        {
            string urlTemplate = pathDictionary.Key;
            PathData path = pathDictionary.Value;

            string operationName = null; 
            var method = RequestMethod.UNKNOWN;
            List<TemplateParameter> parameters = null;
            RequestContract request = null;
            
            if (path.Post != null)
            {
                method = RequestMethod.POST;
                operationName = path.Post.OperationId;
            } else if(path.Get != null)
            {
                method = RequestMethod.GET;
                operationName = path.Get.OperationId;
            } else if(path.Put != null)
            {
                method = RequestMethod.PUT;
                operationName = path.Put.OperationId;
            } else if(path.Patch != null)
            {
                method = RequestMethod.PATCH;
                operationName = path.Patch.OperationId;
            } else if(path.Delete != null)
            {
                method = RequestMethod.DELETE;
                operationName = path.Delete.OperationId;
            }


            return new APIOperation(operationName, method, urlTemplate, parameters, request);
        }
    }
}
