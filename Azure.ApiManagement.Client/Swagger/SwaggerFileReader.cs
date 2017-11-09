using Fitabase.Azure.ApiManagement.Model.Exceptions;
using System.IO;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class SwaggerFileReader : AbstractSwaggerReader
    {
        public SwaggerFileReader(string resourcePath) : base(resourcePath)
        {
        }

      
        protected override string GetSwaggerJson()
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
