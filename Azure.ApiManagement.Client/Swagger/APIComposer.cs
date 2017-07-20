﻿using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger.Models;
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
        public SwaggerObject Swagger;

        public APIComposer(SwaggerObject swagger)
        {
            if (swagger == null)
            {
                throw new SwaggerResourceException("SwaggerObject is required");
            }

            this.Swagger = swagger;
        }
        

        /// <summary>
        /// Compose a SwaggerObject to a API entity 
        /// </summary>
        /// <returns></returns>
        public API Compose()
        {
            string name             = Swagger.Info.Title + Swagger.BasePath;
            string path             = Swagger.BasePath + "/" + Swagger.Info.Title;
            string description      = Swagger.Info.Description;
            string serviceUrl       = Swagger.Host;
            
            string[] protocols  = Swagger.Schemes;
            AuthenticationSettingsConstract authentication   = null;
            SubscriptionKeyParameterNames customNames        = null;
            
            API api = API.Create(name, serviceUrl, path, protocols, description, authentication, customNames);
            api.Operations = GetOperations(Swagger.Paths);      // Inject api operations
            return api;
        }


        /// <summary>
        /// Compose Swagger response to API operation response
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        private OperationResponse[] GetResponse(
            Dictionary<string, Response> responses)
        {
            List<OperationResponse> list = new List<OperationResponse>();
            foreach (KeyValuePair<string, Response> response in responses)
            {
                var code = response.Key;
                var description = response.Value.Description;
                list.Add(new OperationResponse(Int32.Parse(code), description));
                    
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


            return APIOperation.Create(operationName, method, urlTemplate, parameters, request);
        }
    }
}