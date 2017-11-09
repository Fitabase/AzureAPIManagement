using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Newtonsoft.Json;
using System.Text;

namespace Fitabase.Azure.ApiManagement
{

    class APISwaggerBuilder
    {
        private PathData PathData;

        public APISwaggerBuilder(PathData pathdata)
        {
            this.PathData = pathdata;
        }


        /// <summary>
        /// Create an API template parameters from Swagger Path parameters.
        /// </summary>
        /// <returns>Template parameter array</returns>
        public ParameterContract[] BuildeTemplateParameters()
        {
            if (PathData == null)
                throw new SwaggerResourceException("PathData is required");

            Parameter[] parameters = GetParameters();
            if (parameters == null || parameters.Length == 0)
                return null;

            ParameterContract[] parameterContracts = new ParameterContract[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                string json = JsonConvert.SerializeObject(parameters[i]);
                ParameterContract template = JsonConvert.DeserializeObject<ParameterContract>(json);
                template.Description = parameters[i].Format;
                parameterContracts[i] = template;
            }
            return parameterContracts;
        }

        public string BuildRestParametersURL()
        {
            if (PathData == null)
                throw new SwaggerResourceException("PathData is required");

            StringBuilder builder = new StringBuilder();
            OperationMethod operationMethod = GetOperationMethod();
            if (operationMethod.Parameters == null)
                return "";

            Parameter[] parameters = operationMethod.Parameters;
           
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

}
