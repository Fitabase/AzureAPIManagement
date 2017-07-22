using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Fitabase.Azure.ApiManagement.Utilities;
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
            {
                throw new SwaggerResourceException("SwaggerObject is required");
            }
            
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
            {
                throw new SwaggerResourceException("SwaggerObject is required");
            }
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
        {                  // Swagger path name to UrlTemplate
            PathData pathdata = path.Value;
            APIBuilder apiBuilder = new APIBuilder();
            APIBuilder.APIServiceUrlBuilder urlBuilder = apiBuilder.GetBuilder(pathdata);

            OperationMethod operationMethod = urlBuilder.GetOperationMethod();
            string operationName = operationMethod.OperationId;                     // Swagger path operationId
            RequestMethod method = urlBuilder.GetRequestMethod();                   // Get request method
            string urlTemplate = GetOperationnUrl(path.Key) + urlBuilder.BuildURN();

            ParameterContract[] parameters = urlBuilder.GetTemplateParameters();    // Get Template parameter;
            RequestContract request = null;
            
            ResponseContract[] responses = apiBuilder.GetBuilder(operationMethod.Responses).Build();

            APIOperation apiOperation = APIOperation.Create(operationName, method, urlTemplate, parameters, request, responses);
            return apiOperation;
        }



        /// <summary>
        /// Compose Swagger response to API operation response
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        //private APIResponse[] GetResponse(
        //    Dictionary<string, Response> responses)
        //{
        //    List<Model.APIResponse> list = new List<Model.APIResponse>();
        //    foreach (KeyValuePair<string, Response> response in responses)
        //    {
        //        var code = response.Key;
        //        var description = response.Value.Description;
        //        list.Add(Model.APIResponse.Create(int.Parse(code), description));

        //    }

        //    return list.ToArray();
        //}



        /// <summary>
        /// Remove the api title from the operation name. Prevent duplication on API operation URL
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        private string GetOperationnUrl(string operationName)
        {
            return operationName.ToLower().Replace("/" + Swagger.Info.Title.ToLower(), "");
        }
    }
}
