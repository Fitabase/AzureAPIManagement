using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// This class is used to r
    /// </summary>
    public abstract class AbstractSwaggerReader
    {

        public string ResourcePath;

        public AbstractSwaggerReader(string resourcePath)
        {
            ResourcePath = resourcePath;
        }

        /// <summary>
        /// Get Swagger Components from input resource
        /// </summary>
        /// <returns>SwaggerAPIComponent</returns>
        public SwaggerAPIComponent GetSwaggerComponents()
        {
            string json = this.GetSwaggerJson();
            if(String.IsNullOrWhiteSpace(json))
            {
                throw new SwaggerResourceException("Swagger Json is required");
            }
            PrintMessage.Debug(this, json);
            return JsonConvert.DeserializeObject<SwaggerAPIComponent>(json);
        }
        
        /// <summary>
        /// Retrieve swagger json from resource
        /// </summary>
        /// <returns>Json string</returns>
        public abstract string GetSwaggerJson();
    }
}
