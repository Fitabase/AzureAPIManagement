using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (reader == null)
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
            ResponseContract[] responses = GetMethodResponses(pathOperation, method).ToArray();

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
            foreach (KeyValuePair<string, Response> response in operation.Responses)
            {
                string code = response.Key;
                int statusCode;
                try
                {
                    statusCode = int.Parse(code);
                } catch (FormatException)
                {
                    continue;   // invalid response code, continue with next response.
                }
                string description = response.Value.Description;

                RepresentationContract[] representations = null;
                if (method == RequestMethod.GET)
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
                foreach (KeyValuePair<string, PathItem> path in _builder._Swagger.Paths)
                {
                    string pathKey = path.Key;
                    PathItem pathItem = path.Value;

                    if (pathItem.Get != null)
                    {
                        operations.Add(new SingleOperationBuilder(pathItem.Get, pathKey).BuildOperation(RequestMethod.GET));
                    }
                    if (pathItem.Post != null)
                    {
                        operations.Add(new SingleOperationBuilder(pathItem.Post, pathKey).BuildOperation(RequestMethod.POST));
                    }
                    if (pathItem.Put != null)
                    {
                        operations.Add(new SingleOperationBuilder(pathItem.Put, pathKey).BuildOperation(RequestMethod.PUT));
                    }
                    if (pathItem.Patch != null)
                    {
                        operations.Add(new SingleOperationBuilder(pathItem.Patch, pathKey).BuildOperation(RequestMethod.PATCH));
                    }
                    if (pathItem.Delete != null)
                    {
                        operations.Add(new SingleOperationBuilder(pathItem.Delete, pathKey).BuildOperation(RequestMethod.DELETE));
                    }
                }
                return operations;
            }
        }


        class SingleOperationBuilder
        {
            private Operation _operation;
            private string _baseUrl;
            private IList<ParameterContract> _pathParameters;
            private IList<ParameterContract> _queryParameters;
            private IList<ParameterContract> _headerParameters;
            private IList<ParameterContract> _formDataParameters;



            public SingleOperationBuilder(Operation operation, string baseUrl)
            {
                this._operation = operation;
                this._baseUrl = baseUrl;
                _pathParameters = new List<ParameterContract>();
                _queryParameters = new List<ParameterContract>();
                _headerParameters = new List<ParameterContract>();
                _formDataParameters = new List<ParameterContract>();
            }

            public APIOperation BuildOperation(RequestMethod method = RequestMethod.GET)
            {

                if (_operation == null)
                    return null;
                PrepareParameterLists();

                string description = _operation.Description;
                string name = _operation.OperationId;
                string urlTemplate = _baseUrl + BuildQueryUrl();
                ParameterContract[] parameters = _pathParameters.Concat(_queryParameters).ToArray();
                RequestContract request = BuildRequest();
                ResponseContract[] responses = BuildResponses();
                return APIOperation.Create(name, method, urlTemplate, parameters, request, responses, description);
            }


            // Ex. URL = /{accountId}/{subscriptionId}
            private string BuildPathUrl()
            {
                StringBuilder builder = new StringBuilder();
                foreach (ParameterContract p in _pathParameters)
                {
                    builder.Append("/{").Append(p.Name).Append("}");
                }
                return builder.ToString();
            }

            // Ex. URL = ?username={username}&pasword={password}
            private string BuildQueryUrl()
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _queryParameters.Count; i++)
                {
                    ParameterContract p = _queryParameters.ElementAt(i);

                    if (i == 0) builder.Append("?");
                    else builder.Append("&");

                    builder.Append(p.Name).Append("={").Append(p.Name).Append("}");
                }
                return builder.ToString();
            }


            /// <summary>
            /// Build Operation's template parameters. Depending on where the parameters, build the template parameters accordingly to it is.
            /// </summary>
            /// <returns></returns>
            private void PrepareParameterLists()
            {
                if (_operation == null)
                    throw new SwaggerResourceException("PathData is required");

                IList<IParameter> parameters = _operation.Parameters;
                foreach (IParameter parameter in parameters)
                {
                    switch (parameter.In)
                    {
                        case "path":
                            _pathParameters.Add(BuildParameter(parameter as NonBodyParameter));
                            break;
                        case "query":
                            _queryParameters.Add(BuildParameter(parameter as NonBodyParameter));
                            break;
                        case "header":
                            _headerParameters.Add(BuildParameter(parameter as NonBodyParameter));
                            break;
                        case "body":
                        case "formData":
                            _formDataParameters.Add(BuildParameter(parameter as NonBodyParameter));
                            break;
                        default:
                            break;
                    }
                }
            }

            private ParameterContract BuildParameter(BodyParameter p)
            {
                if (p != null)
                {
                    return ParameterContract.Create(p.Name, "string", p.Description, null, p.Required);
                }
                return null;
            }

            private ParameterContract BuildParameter(NonBodyParameter p)
            {
                if (p != null)
                {
                    string defaultValue = (p.Default != null) ? p.Default.ToString() : null;
                    string description = (p.Format != null) ? "format - " + p.Format + ". " : "";
                    description += p.Description;

                    return ParameterContract.Create(p.Name, p.Type, description, defaultValue, p.Required);
                }
                return null;
            }

            private RequestContract BuildRequest()
            {
                if (_operation == null)
                    throw new SwaggerResourceException("Operation is required");

                RepresentationContract[] representations = null;
                if(this._operation.Consumes != null && this._operation.Consumes.Count > 0)
                {
                    
                    representations = new RepresentationContract[]
                    {
                        RepresentationContract.Create(this._operation.Consumes[0], null, null, null, _formDataParameters.ToArray())
                    };
                }


                OperationRequestBuilder rBuilder = new OperationRequestBuilder()
                {
                    Description = _operation.Description,
                    Headers = _headerParameters.ToArray(),
                    //Queries = _queryParameters.ToArray(),
                    Representations = representations
                };
                return rBuilder.BuildRequest();
            }


            // Build operation responses, return null indicates no responses
            private ResponseContract[] BuildResponses()
            {
                if (_operation == null)
                    throw new SwaggerResourceException("Operation is required");
                if (_operation.Responses == null || _operation.Responses.Count == 0)
                {
                    return null;
                }

                List<ResponseContract> responseContracts = new List<ResponseContract>();
                foreach (KeyValuePair<string, Response> response in _operation.Responses)
                {
                    ResponseBuilder rBuilder = new ResponseBuilder()
                    {
                        _response = response.Value,
                        _statusCode = response.Key
                    };
                    responseContracts.Add(rBuilder.BuildResponse());
                }
                return responseContracts.ToArray();
            }

        }

        #endregion Operation Builder


        #region Operation Request Builder
        class OperationRequestBuilder {

            public string Description;
            public ParameterContract[] Headers;
            public ParameterContract[] Queries;
            public RepresentationContract[] Representations;

            public RequestContract BuildRequest()
            {
                return RequestContract.Create(Description, Headers, Queries, Representations);
            }
        }
    
        #endregion Operation Request Builder




        #region Operation Response Builder
        class ResponseBuilder
        {
            internal Response _response;
            internal string _statusCode;


            public ResponseContract BuildResponse()
            {
                RepresentationContract[] representations = {
                    RepresentationContract.Create("application/json", null, GetSchemaDefinition(), GetResponseSampleCode(), null)
                };
                return ResponseContract.Create(GetStatusCode(), _response.Description, representations);
            }


            private int GetStatusCode()
            {

                int sCode;
                try
                {
                    sCode = int.Parse(_statusCode);
                }
                catch (FormatException)
                {
                    string desc = _response.Description.ToLower();
                    if (desc.Contains("ok") || desc.Contains("successful"))
                        sCode = 200;
                    else
                        sCode = 500;

                }
                return sCode;
            }

            private string GetSchemaDefinition()
            {
                string def = null;
                if (_response.Schema != null && _response.Schema.Ref != null)
                {
                    string[] schemaRefs = _response.Schema.Ref.Split('/');
                    def = schemaRefs[schemaRefs.Length - 1];
                }
                return def;
            }

            private string GetResponseSampleCode()
            {
                return (_response.Examples != null) ? _response.Examples.ToString() : null;
            }

            //private RepresentationContract BuildRepresentation(string contentType, string typeName, string sample, ParameterContract[] formParameters, string schemaId = null)
            //{
            //    return RepresentationContract.Create(contentType, schemaId, typeName, sample, formParameters);
            //}

        }
        #endregion Operation Response Builder
    }

}
