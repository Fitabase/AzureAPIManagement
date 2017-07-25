using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Newtonsoft.Json;
using System;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// This class is used to r
    /// </summary>
    public abstract class AbstractSwaggerReader
    {

        protected string ResourcePath;

        public AbstractSwaggerReader(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        /// <summary>
        /// Get Swagger Components from input resource
        /// </summary>
        /// <returns>SwaggerAPIComponent</returns>
        public SwaggerObject GetSwaggerObject()
        {
            string json = this.GetSwaggerJson();
            if(String.IsNullOrWhiteSpace(json))
            {
                throw new SwaggerResourceException("Missing json content");
            }
            return JsonConvert.DeserializeObject<SwaggerObject>(json);
        }
        


        /// <summary>
        /// Retrieve swagger json from resource
        /// </summary>
        /// <returns>Json string</returns>
        protected abstract string GetSwaggerJson();
    }
}
