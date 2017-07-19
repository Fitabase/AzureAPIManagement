using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class JsonFileReader : AbstractSwaggerReader
    {
        public JsonFileReader(string resourcePath) : base(resourcePath)
        {
        }

      
        public override string GetSwaggerJson()
        {
            // quick check if the file is json
            if (!this.ResourcePath.EndsWith(".json"))
            {
                throw new SwaggerResourceException("Json file is required");
            }
            if (!File.Exists(this.ResourcePath))
            {
                throw new SwaggerResourceException("File doesn't exist");
            }
            return File.ReadAllText(this.ResourcePath);  // Open, read, then close the file
        }
        
    }
}
