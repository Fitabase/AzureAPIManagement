using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Fitabase.Azure.ApiManagement.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// This class is used to compose a Swagger object to a API object
    /// </summary>
    class APIComposer
    {
        public SwaggerObject Swagger;


        public APIComposer(SwaggerObject swagger)
        {
            this.Swagger = swagger;
        }
        
        

        /// <summary>
        /// Compose a SwaggerObject to a API entity 
        /// </summary>
        /// <returns></returns>
        public API Compose()
        {
            if (Swagger == null)
                throw new SwaggerResourceException("SwaggerObject is required");
            
            string name         = GetAPIName();                     // Get API name from swagger
            string path         = GetAPIPath();                     // Get API path from swagger
            string description  = Swagger.Info.Description;         // API description
            string serviceUrl   = Swagger.Host;                     // API service URL form swagger
            string[] protocols  = Swagger.Schemes;

            AuthenticationSettingsConstract authentication   = null;
            SubscriptionKeyParameterNames customNames        = null;
            
            API api = API.Create(name, serviceUrl, path, protocols, description, authentication, customNames);
            api.Operations = GetOperations(Swagger.Paths);      // Inject operations to the api
            return api;
        }




        /// <summary>
        /// Provide a API path from swagger. The API path will likely append to host
        /// Ex. /v1/bodytrace with title = bodytrace, basepath = /v1
        /// </summary>
        /// <returns></returns>
        private string GetAPIPath()
        {
            if (Swagger == null)
                throw new SwaggerResourceException("SwaggerObject is required");
            return Swagger.BasePath + "/" + Swagger.Info.Title;
        }


        /// <summary>
        /// Provide a (unique) API name from swagger
        /// Ex. bodytrace/v1 with title = bodytrace, basepath = /v1
        /// </summary>
        /// <returns></returns>
        private string GetAPIName()
        {
            if (Swagger == null)
                throw new SwaggerResourceException("SwaggerObject is required");
            return Swagger.Info.Title + Swagger.BasePath;
        }

        



        /// <summary>
        /// Compose a list of API Operations
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private List<APIOperation> GetOperations(Dictionary<string, PathData> paths)
        {
            List<APIOperation> operations = new List<APIOperation>();
            foreach (KeyValuePair<string, PathData> path in paths)
            {
                APIOperation apiOperation = GetOperation(path);
                operations.Add(apiOperation);
            }
            return operations;
        }
        

        /// <summary>
        /// Compose a swagger path to a API operation
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private APIOperation GetOperation(KeyValuePair<string, PathData> path)
        {
            PathData pathdata = path.Value;
            APIBuilder builder = new APIBuilder(pathdata);

            OperationMethod pathOperation = builder.GetOperationMethod();
            string operationName = pathOperation.OperationId;
            string urlTemplate = GetOperationnUrl(path.Key) + builder.BuildServiceURL();
            string description = null;
            RequestMethod method = builder.GetRequestMethod();
            ParameterContract[] parameters = builder.BuildeTemplateParameters();
            RequestContract request = GetRequest();
            ResponseContract[] responses =  GetMethodResponses(pathOperation, method).ToArray();
            
            APIOperation apiOperation = APIOperation.Create(operationName, method, urlTemplate, parameters, request, responses, description);
            return apiOperation;
        }
        

        private RequestContract GetRequest()
        { 
            // TODO create operation request
            return RequestContract.Create();
        }


        /// <summary>
        /// Remove the api title from the operation name. Prevent duplication on API operation URL
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        private string GetOperationnUrl(string operationName)
        {
            return operationName.ToLower().Replace("/" + Swagger.Info.Title.ToLower(), "");
        }



        
        private string GetSampleData()
        {
            return JsonConvert.SerializeObject(Swagger.Definitions, Formatting.Indented);
        }


        private List<ResponseContract> GetMethodResponses(OperationMethod operation, RequestMethod method)
        {
            if (operation == null)
                return null;

            List<ResponseContract> list = new List<ResponseContract>();
            foreach(KeyValuePair<string, Response> response in operation.Responses)
            {
                string code = response.Key;
                int statusCode = int.Parse(code);
                string description = response.Value.Description;

                RepresentationContract[] representations = null;
                if(method == RequestMethod.GET)
                    representations = new RepresentationContract[] { GetJsonRepresenation() };

                ResponseContract r = ResponseContract.Create(statusCode, description, representations);
                list.Add(r);
            }
            return list;
        }
        
        private RepresentationContract GetJsonRepresenation()
        {
            return RepresentationContract.Create("application/json", null, Swagger.Info.Title, GetSampleData(), null);
        }

    }
}
