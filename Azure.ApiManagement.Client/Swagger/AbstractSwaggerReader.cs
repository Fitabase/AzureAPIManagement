using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Model;
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
        /// Cast json string to SwaggerDocument object
        /// </summary>
        /// <returns>SwaggerDocument</returns>
        public SwaggerDocument GetSwaggerObject()
        {
            string json = this.GetSwaggerJson();

            if (String.IsNullOrWhiteSpace(json))
            {
                throw new SwaggerResourceException("Missing json content");
            }

            JsonConverter[] converters = { new ParameterConverter(), new SchemeSecurityConverter() };
            SwaggerDocument doc = JsonConvert.DeserializeObject<SwaggerDocument>(json, converters);
            return doc;
        }


        /// <summary>
        /// Retrieve swagger json from resource
        /// </summary>
        /// <returns>Json string</returns>
        protected abstract string GetSwaggerJson();
    }
}
