using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class JsonFileReader : ISwaggerFileReader
    {

        /// <summary>
        /// Convert a file content to Swagger object
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public SwaggerAPIComponent GetSwaggerFromFile(string filePath)
        {
            var jsonStr = GetJson(filePath);
            return JsonConvert.DeserializeObject<SwaggerAPIComponent>(jsonStr);
        }
        

        /// <summary>
        /// Read the file and append to a string
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <returns>String in json format</returns>
        private string GetJson(string filePath)
        {
            // quick check if the file is json
            if(!filePath.EndsWith(".json"))
            {
                throw new ArgumentException("Json file is required");
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File doesn't exist");
            }
            return File.ReadAllText(filePath);  // Open, read, then close the file
        }
        
    }
}
