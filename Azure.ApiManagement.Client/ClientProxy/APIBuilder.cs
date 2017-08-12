using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fitabase.Azure.ApiManagement
{
    /// <summary>
    /// This class is used to compose a Swagger object to a API object
    /// </summary>
    public class APIBuilder
    {
        public SwaggerDocument _Swagger;


        public APIBuilder(SwaggerDocument swagger)
        {
            this._Swagger = swagger;
        }
        
        public static APIBuilder GetBuilder(string swaggerURL)
        {
            AbstractSwaggerReader reader = new SwaggerUrlReader(swaggerURL);
            return new APIBuilder(reader.GetSwaggerObject());
        }

        public static APIBuilder GetBuilder(AbstractSwaggerReader reader)
        {
            if(reader == null)
            {
                throw new SwaggerResourceException("Swagger reader cannot be null");
            }
            return new APIBuilder(reader.GetSwaggerObject());
        }
        
        
        
        public API BuildAPIAndOperations()
        {
            API api = new APIMetatDataBuilder(this).BuildAPI();
            api.Operations = new OperationBuilder(this).BuildOperations();
            return api;
        }

        #region UNKNOWN

        private List<APIOperation> BuildOperations()
        {
            if (_Swagger == null)
                throw new SwaggerResourceException("Swagger cannot be null");

            List<APIOperation> operations = new List<APIOperation>();
            foreach (KeyValuePair<string, PathItem> path in _Swagger.Paths)
            {
                APIOperation apiOperation = GetOperation(path);
                operations.Add(apiOperation);
            }
            return operations;
        }






        /// <summary>
        /// Compose a list of API Operations
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private List<APIOperation> GetOperations(Dictionary<string, PathItem> paths)
        {
            List<APIOperation> operations = new List<APIOperation>();
            foreach (KeyValuePair<string, PathItem> path in paths)
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
        private APIOperation GetOperation(KeyValuePair<string, PathItem> path)
        {
            PathItem pathdata = path.Value;
            APISwaggerBuilder builder = new APISwaggerBuilder(pathdata);

            Operation pathOperation = builder.GetOperationMethod();
            string operationName = pathOperation.OperationId;
            string urlTemplate = GetOperationnUrl(path.Key) + builder.BuildRestParametersURL();        // Append parameters to the URL
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
            return RequestContract.Create();
        }
        
        private RepresentationContract[] GetRequestRepresent()
        {
            return null;
        }

        
        /// <summary>
        /// Remove the api title from the operation name. Prevent duplication on API operation URL
        /// </summary>
        /// <param name="operationName"></param>
        /// <returns></returns>
        private string GetOperationnUrl(string operationName)
        {
            return operationName.ToLower().Replace("/" + _Swagger.Info.Title.ToLower(), "");
        }

        
        private string GetSampleData()
        {
            return JsonConvert.SerializeObject(_Swagger.Definitions, Formatting.Indented);
        }


        private List<ResponseContract> GetMethodResponses(Operation operation, RequestMethod method)
        {
            if (operation == null)
                return null;

            List<ResponseContract> list = new List<ResponseContract>();
            foreach(KeyValuePair<string, Response> response in operation.Responses)
            {
                string code = response.Key;
                int statusCode;
                try
                {
                    statusCode = int.Parse(code);
                } catch(FormatException)
                {
                    continue;   // invalid response code, continue with next response.
                }
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
            return RepresentationContract.Create("application/json", null, _Swagger.Info.Title, GetSampleData(), null);
        }

        #endregion


        #region API Metadata Builder
        class APIMetatDataBuilder
        {
            private APIBuilder _builder;

            public APIMetatDataBuilder(APIBuilder builder)
            {
                this._builder = builder;
            }

            public API BuildAPI()
            {
                if (_builder._Swagger == null)
                    throw new SwaggerResourceException("Swagger cannot be null");

                string name = GetAPIName();                     // Get API name from swagger
                string path = GetAPIPath();                     // Get API path from swagger
                string description = _builder._Swagger.Info.Description;  // API description
                string serviceUrl = _builder._Swagger.Host;               // API service URL form swagger
                string[] protocols = _builder._Swagger.Schemes.Cast<string>().ToArray();

                AuthenticationSettingsConstract authentication = null;
                SubscriptionKeyParameterNames customNames = null;

                API api = API.Create(name, serviceUrl, path, protocols, description, authentication, customNames);
                return api;
            }


            /// <summary>
            /// Provide a API path from swagger. The API path will likely append to host
            /// Ex. /v1/bodytrace with title = bodytrace, basepath = /v1
            /// </summary>
            /// <returns></returns>
            private string GetAPIPath()
            {
                if (_builder._Swagger == null)
                    throw new SwaggerResourceException("SwaggerObject is required");
                return _builder._Swagger.BasePath + "/" + _builder._Swagger.Info.Title.Replace(" ", "");
            }


            /// <summary>
            /// Provide a (unique) API name from swagger
            /// Ex. bodytrace/v1 with title = bodytrace, basepath = /v1
            /// </summary>
            /// <returns></returns>
            private string GetAPIName()
            {
                if (_builder._Swagger == null)
                    throw new SwaggerResourceException("SwaggerObject is required");
                return _builder._Swagger.Info.Title + _builder._Swagger.BasePath;
            }



        }
        #endregion



        #region Operation Builder
        class OperationBuilder
        {
            private APIBuilder _builder;
            
            public OperationBuilder(APIBuilder builder)
            {
                this._builder = builder;
            }


            public HashSet<APIOperation> BuildOperations()
            {
                HashSet<APIOperation> operations = new HashSet<APIOperation>();
                foreach(KeyValuePair<string, PathItem> path in _builder._Swagger.Paths)
                {
                    string pathKey = path.Key;
                    PathItem pathItem = path.Value;
                    AddNonNullOperation(operations, BuildOperation(pathKey, pathItem.Get, RequestMethod.GET));
                    AddNonNullOperation(operations, BuildOperation(pathKey, pathItem.Post, RequestMethod.POST));
                    AddNonNullOperation(operations, BuildOperation(pathKey, pathItem.Put, RequestMethod.PUT));
                    AddNonNullOperation(operations, BuildOperation(pathKey, pathItem.Patch, RequestMethod.PATCH));
                    AddNonNullOperation(operations, BuildOperation(pathKey, pathItem.Delete, RequestMethod.DELETE));
                }
                return operations;
            }
            
            private void AddNonNullOperation(HashSet<APIOperation> operations, APIOperation operation)
            {
                if (operation != null)
                    operations.Add(operation);
            }


            public APIOperation BuildOperation(string urlTemplate, Operation operation, RequestMethod method)
            {
                if (operation == null)
                    return null;

                ParameterContract[] parameters = operation.;
                RequestContract request = null;
                ResponseContract[] responses = null;
                string description = operation.Description;
                string name = operation.OperationId;
                return APIOperation.Create(name, method, urlTemplate, parameters, request, responses, description);
            }



            private ParameterContract[] BuildTemplateParameters(Operation operation)
            {
                if (operation == null)
                    throw new SwaggerResourceException("PathData is required");

                IList<IParameter> parameters = operation.Parameters;
                IList<ParameterContract> parameterContracts = new List<ParameterContract>();
                foreach(IParameter parameter in parameters)
                {
                    if(parameter.In == "path")
                    {
                        NonBodyParameter nonBodyParameter = parameter as NonBodyParameter;
                        ParameterContract parameterContract = ParameterContract.Create(nonBodyParameter.Name, nonBodyParameter.Type, nonBodyParameter.Description, nonBodyParameter.Default.ToString(), nonBodyParameter.Required);
                        parameterContracts.Add(parameterContract);
                    }
                }

                //if (parameters == null || parameters.Count == 0)
                //    return null;

                //ParameterContract[] parameterContracts = new ParameterContract[parameters.Count];

                //for (int i = 0; i < parameters.Count; i++)
                //{
                //    string json = JsonConvert.SerializeObject(parameters[i]);
                //    ParameterContract template = JsonConvert.DeserializeObject<ParameterContract>(json);
                //    template.Description = parameters[i].Description;
                //    parameterContracts[i] = template;
                //}
                return parameterContracts.ToArray();
            }
        }
        

        #endregion Operation Builder

    }

}
