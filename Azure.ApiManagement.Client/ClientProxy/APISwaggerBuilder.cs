using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Model;
using System.Collections.Generic;
using System.Text;

namespace Fitabase.Azure.ApiManagement
{

    class APISwaggerBuilder
    {
        private PathItem PathData;

        public APISwaggerBuilder(PathItem pathdata)
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

            IList<IParameter> parameters = GetParameters();
            if (parameters == null || parameters.Count == 0)
                return null;

            ParameterContract[] parameterContracts = new ParameterContract[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                string json = JsonConvert.SerializeObject(parameters[i]);
                ParameterContract template = JsonConvert.DeserializeObject<ParameterContract>(json);
                template.Description = parameters[i].Description;
                parameterContracts[i] = template;
            }
            return parameterContracts;
        }

        public string BuildRestParametersURL()
        {
            if (PathData == null)
                throw new SwaggerResourceException("PathData is required");

            StringBuilder builder = new StringBuilder();
            Operation operationMethod = GetOperationMethod();
            if (operationMethod.Parameters == null)
                return "";

            IList<IParameter> parameters = operationMethod.Parameters;
           
            for (int i = 0; i < parameters.Count; i++)
            {
                string pathVariable = parameters[i].Name;
                builder.Append("/").Append(pathVariable)
                        .Append("/{").Append(pathVariable).Append("}");
            }
            return builder.ToString();
        }


        public IList<IParameter> GetParameters()
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
        



        //public List<Operation> GetOperationMethod()
        //{
        //    if (PathData == null)
        //        throw new SwaggerResourceException("PathData is required");
        //    List<Operation> operations = new List<Operation>();


        //    if (PathData.Post != null) operations.Add(PathData.Post);
        //    if (PathData.Get != null) operations.Add(PathData.Get);
        //    if (PathData.Patch != null) operations.Add(PathData.Patch);
        //    if (PathData.Put != null) operations.Add(PathData.Put);
        //    if (PathData.Delete != null) operations.Add(PathData.Delete);
        //    if (PathData.Options != null) operations.Add(PathData.Options);
        //    if (PathData.Head != null) operations.Add(PathData.Head);

        //    return operations;
        //}

        public Operation GetOperationMethod()
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
