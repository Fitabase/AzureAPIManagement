using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
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
        #region APIBuilder Initializer
        private SwaggerDocument _swagger;
        private APIBuilderSetting _setting;
        
        private APIBuilder() { }
        
        public static APIBuilder GetBuilder(string swaggerURL, APIBuilderSetting setting = null)
        {
            AbstractSwaggerReader reader = new SwaggerUrlReader(swaggerURL);
            return GetBuilder(reader, setting);
        }

        public static APIBuilder GetBuilder(AbstractSwaggerReader reader, APIBuilderSetting setting = null)
        {
            if (reader == null)
                throw new SwaggerResourceException("SwaggerReader cannot be null");
            if (reader.GetSwaggerObject() == null)
                throw new SwaggerResourceException("SwaggerDocument cannot be null");

            APIBuilder builder = new APIBuilder()
            {
                _swagger = reader.GetSwaggerObject(),
                _setting = (setting == null) ? new APIBuilderSetting() : setting,
            };
            return builder;
        }

        #endregion
        

        public API BuildAPIAndOperations()
        {
            API api = new APIMetatDataBuilder(this).BuildAPI();
            api.Operations = new OperationBuilder(this).BuildOperations();
            return api;
        }
        
        #region API Metadata Builder
        class APIMetatDataBuilder
        {
            private APIBuilder _builder;

            public APIMetatDataBuilder(APIBuilder builder)
            {
                this._builder = builder;
            }

            /// <summary>
            /// Build a new api from swagger document
            /// </summary>
            /// <returns>An API</returns>
            public API BuildAPI()
            {
                if (_builder._swagger == null)
                    throw new SwaggerResourceException("Swagger cannot be null");

                string   name = GetAPIName();                     // Get API name from swagger
                string   path = GetAPIPath();                     // Get API path from swagger
                string   description = _builder._swagger.Info.Description;  // API description
                string   serviceUrl  = _builder._swagger.Host;              // API service URL form swagger
                string[] protocols   = _builder._swagger.Schemes.Cast<string>().ToArray();

                AuthenticationSettingsConstract authentication = null;
                SubscriptionKeyParameterNames customNames = null;

                API api = API.Create(name, serviceUrl, path, protocols, description, authentication, customNames);
                return api;
            }


            /// <summary>
            /// Return the API path from swagger. The API path will likely append to 
            /// host; hence the API
            /// Ex. apiPath = /v1/bodytrace where title = bodytrace, basepath = /v1
            /// </summary>
            /// <returns>The API path </returns>
            private string GetAPIPath()
            {
                if (_builder._swagger == null)
                    throw new SwaggerResourceException("Swagger is required");
                string version = _builder._swagger.BasePath.Replace("/", "");
                string path    = _builder._swagger.Info.Title.Replace(" ", "");

                if (_builder._setting.API_PathOnly)
                    return path;
                if (_builder._setting.API_PathThenVersion)
                    return String.Format("/{0}/{1}", path, version);
                else
                    return String.Format("/{0}/{1}", version, path);
            }

            
            /// <summary>
            /// Return a (unique) API name from swagger. The API's name is appended
            /// with the basepath (or api's version) to provide an unique API name 
            /// betweeen different API versions
            /// 
            /// Ex. bodytrace/v1 where title = bodytrace, basepath = /v1
            /// </summary>
            /// <returns>an unique API name</returns>
            private string GetAPIName()
            {
                if (_builder._swagger == null)
                    throw new SwaggerResourceException("Swagger is required");

                string name    = _builder._swagger.Info.Title;
                string version = _builder._swagger.BasePath.Replace("/", "");

                if (_builder._setting.API_NameOnly)
                    return _builder._swagger.Info.Title;
                if (_builder._setting.API_NameThenVersion)
                    return String.Format("{0}/{1}", name, version);
                else
                 return String.Format("{0}/{1}", version, name);
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

            /// <summary>
            /// Build api operations from swagger document. 
            /// </summary>
            /// <returns>A set of api operations</returns>
            public HashSet<APIOperation> BuildOperations()
            {
                HashSet<APIOperation> operations = new HashSet<APIOperation>();
                foreach (KeyValuePair<string, PathItem> path in _builder._swagger.Paths)
                {
                    string pathKey    = path.Key;
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
                _pathParameters     = new List<ParameterContract>();
                _queryParameters    = new List<ParameterContract>();
                _headerParameters   = new List<ParameterContract>();
                _formDataParameters = new List<ParameterContract>();
            }

            /// <summary>
            /// Build an APIOperation from a swagger operation
            /// </summary>
            /// <param name="method"></param>
            /// <returns>An APIOperation</returns>
            public APIOperation BuildOperation(RequestMethod method = RequestMethod.GET)
            {
                if (_operation == null)
                    return null;

                PrepareParameterLists();

                string name        = _operation.OperationId;              
                string description = _operation.Description;
                string urlTemplate = _baseUrl + BuildQueryUrl();

                ParameterContract[] parameters = _pathParameters.Concat(_queryParameters).ToArray();
                ResponseContract[]  responses  = BuildResponses();
                RequestContract     request    = BuildRequest();
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

                if (_operation.Parameters == null || _operation.Parameters.Count == 0)
                    return;                                     

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
                            _formDataParameters.Add(BuildParameter(parameter as BodyParameter));
                            break;
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
                    string type = "string";
                    if(p.Schema != null && p.Schema.Type != null)
                    {
                        type = p.Schema.Type;
                    }
                    return ParameterContract.Create(p.Name, type, p.Description, null, p.Required);
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
                    
                    if(p.Type == "array")
                    {
                        if (p.Items != null && p.Items.Type != null)
                        {
                            description = "array " + p.Items.Type + ". " + description;
                        }
                    }

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
                else if(_formDataParameters.Count > 0)
                {
                    representations = new RepresentationContract[]
                    {
                        RepresentationContract.Create("application/json", null, null, null, _formDataParameters.ToArray())
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
