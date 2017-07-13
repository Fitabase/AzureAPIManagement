using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    class SwaggerApiComposer
    {
        public SwaggerObject Swagger;

        public SwaggerApiComposer(SwaggerObject swagger)
        {
            if (swagger == null)
            {
                throw new ArgumentException("Swagger object is required");
            }

            this.Swagger = swagger;
        }

        public void Publish()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<API> Compose()
        {
            HashSet<API> apiList = new HashSet<API>();
            HashSet<APIOperation> apiOperations = new HashSet<APIOperation>();
            string id = API.GenerateIdSignature();
            string name = Swagger.Info.Title;               
            string description = Swagger.GetDefinition();
            string serviceUrl = Swagger.Host;
            string path = "";
            List<string> protocols = Swagger.Schemes.ToList();
            AuthenticationSettings authentication = null;
            SubscriptionKeyParameterNames customNames = null;
            
            API api = new API(name, description,
                   serviceUrl, path,
                   protocols,
                   authentication,
                   customNames);

            // Append API operation
            api.Operations = GetOperations(Swagger.Paths);


            apiList.Add(api);
            return apiList;
        }


        /// <summary>
        /// Compose Swagger response to API operation response
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        public APIOperation.OperationResponse[] GetResponse(Dictionary<string, SwaggerObject.SwaggerResponse> responses)
        {
            List<APIOperation.OperationResponse> list = new List<APIOperation.OperationResponse>();
            foreach (KeyValuePair<string, SwaggerObject.SwaggerResponse> response in responses)
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
        public List<APIOperation> GetOperations(Dictionary<string, SwaggerObject.PathData> paths)
        {
            List<APIOperation> operations = new List<APIOperation>();
            foreach (KeyValuePair<string, SwaggerObject.PathData> p in paths)
            {
                APIOperation apiOperation = GetOperation(p);
                apiOperation.Responses = GetResponse(p.Value.Post.Responses);
                operations.Add(apiOperation);
            }
            return operations;
        }

        /// <summary>
        /// Compose swagger path to API operation
        /// </summary>
        /// <param name="pathDictionary"></param>
        /// <returns></returns>
        private APIOperation GetOperation(KeyValuePair<string, SwaggerObject.PathData> pathDictionary)
        {
            string id = APIOperation.GenerateIdSignature();
            string urlTemplate = pathDictionary.Key;
            SwaggerObject.PathData path = pathDictionary.Value;

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
                operationName = path.Post.OperationId;
            } else
            {
            }
            return new APIOperation(id, operationName, method, urlTemplate, parameters, request);
        }
    }
}
