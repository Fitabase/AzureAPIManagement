using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Utilities
{
    public class APIBuilder
    {

        public APIServiceUrlBuilder GetBuilder(PathData swaggerPathdata)
        {
            return new APIServiceUrlBuilder(swaggerPathdata);
        }

        public APIResponseBuilder GetBuilder(Dictionary<string, Response> swaggerResponses)
        {
            return new APIResponseBuilder(swaggerResponses);
        }

        public APISampleDataBuilder GetBuilder(Dictionary<string, Schema> schemas)
        {
            return new APISampleDataBuilder(schemas);
        }


        #region

        public class APISampleDataBuilder
        {
            private Dictionary<string, Schema> Definitions;

            public APISampleDataBuilder(Dictionary<string, Schema> definitions)
            {
                this.Definitions = definitions;
            }

            
            public void Build()
            {
                if (this.Definitions == null)
                    throw new SwaggerResourceException("Missing swagger definitions");
                foreach (KeyValuePair<string, Schema> def in Definitions)
                {
                    string key = def.Key;
                    Schema schema = def.Value;
                    List<RepresentationContract> representations = new List<RepresentationContract>();
                    foreach (KeyValuePair<string, Swagger.Models.Property> property in schema.Properties)
                    {
                        // TODO create represenation
                        string contentType = null;
                        string sample = null;
                        RepresentationContract representation = null; // Representation.Create(contentType, sample, formParameters);
                        representations.Add(representation);
                    }

                }
            }
        }

        internal APISampleDataBuilder GetBuilder(object schemas)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region APIReponseBuilder


        public class APIResponseBuilder
        {
            private Dictionary<string, Response> Definitions;

            public APIResponseBuilder(Dictionary<string, Response> definitions)
            {
                this.Definitions = definitions;
            }

            public ResponseContract[] Build()
            {
                List<ResponseContract> list = new List<Model.ResponseContract>();
                foreach (KeyValuePair<string, Response> response in Definitions)
                {
                    // TODO create represetantation contract
                    string code = response.Key;
                    string description = response.Value.Description;
                    RepresentationContract[] representation = null;
                    list.Add(ResponseContract.Create(int.Parse(code), description, representation));
                }

                return list.ToArray();
            }
        }
        #endregion




        #region APIServiceUrlBuilder

        public class APIServiceUrlBuilder
        {
            private PathData PathData { get; set; }

            public APIServiceUrlBuilder(PathData PathData)
            {
                this.PathData = PathData;
            }


            /// <summary>
            /// Create an API template parameters from Swagger Path parameters.
            /// </summary>
            /// <returns>Template parameter array</returns>
            public ParameterContract[] GetTemplateParameters()
            {
                if (PathData == null)
                    throw new SwaggerResourceException("PathData is required");
                
                Parameter[] parameters = GetParameters();
                if (parameters == null || parameters.Length == 0)
                    return null;

                ParameterContract[] array = new ParameterContract[parameters.Length - 1];

                for(int i = 0; i < parameters.Length; i++)
                {
                    string json = JsonConvert.SerializeObject(parameters[i]);
                    ParameterContract template = JsonConvert.DeserializeObject<ParameterContract>(json);
                    template.Description = parameters[i].Format;
                    array[i] = template;
                }
                return array;
            }
            
            public string BuildURN()
            {
                if (PathData == null)
                    throw new SwaggerResourceException("PathData is required");

                StringBuilder builder = new StringBuilder();
                Parameter[] parameters = GetOperationMethod().Parameters;
                for (int i = 0; i < parameters.Length; i++)
                {
                    string pathVariable = parameters[i].Name;
                    builder.Append("/").Append(pathVariable)
                            .Append("/{").Append(pathVariable).Append("}");
                }
                return builder.ToString();
            }


            public Parameter[] GetParameters()
            {
                if (PathData.Post != null) return PathData.Post.Parameters;
                if (PathData.Get != null) return PathData.Get.Parameters;
                if (PathData.Put != null) return PathData.Put.Parameters;
                if (PathData.Delete != null) return PathData.Delete.Parameters;
                if (PathData.Patch != null) return PathData.Patch.Parameters;
                if (PathData.Options != null) return PathData.Options.Parameters;
                if (PathData.Head != null) return PathData.Head.Parameters;
                return null;

            }
            
            /// <returns></returns>
            public RequestMethod GetRequestMethod()
            {
                if (PathData == null)
                    throw new SwaggerResourceException("PathData is required");

                if (PathData.Post != null) return RequestMethod.POST;
                if (PathData.Get != null) return RequestMethod.GET;
                if (PathData.Patch != null) return RequestMethod.PATCH; ;
                if (PathData.Put != null) return RequestMethod.PUT;
                if (PathData.Delete != null) return RequestMethod.DELETE;

                throw new SwaggerResourceException("PathData's method is neither [POST, GET, PUT, PATCH, DELETE]");
            }

            
            public OperationMethod GetOperationMethod()
            {
                if (PathData == null)
                    throw new SwaggerResourceException("PathData is required");

                if (PathData.Post != null) return PathData.Post;
                if (PathData.Get != null) return PathData.Get;
                if (PathData.Patch != null) return PathData.Patch;
                if (PathData.Put != null) return PathData.Put;
                if (PathData.Delete != null) return PathData.Delete;
                if (PathData.Options != null) return PathData.Options;
                if (PathData.Head != null) return PathData.Head;

                throw new SwaggerResourceException("Invalid path method");
            }
        }

        #endregion
    }

}
