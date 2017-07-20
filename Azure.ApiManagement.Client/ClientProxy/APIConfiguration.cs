using Fitabase.Azure.ApiManagement.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    public class APIConfiguration
    {
        public bool WriteToFile { get; set; }
        public string OutputFolder { get; set; }
        public AbstractSwaggerReader SwaggerReader { get; set; }

      
        public APIConfiguration(AbstractSwaggerReader swaggerReader, string outputFolder = null)
        {
            this.SwaggerReader = swaggerReader;
            this.OutputFolder = outputFolder;
            this.WriteToFile = (outputFolder != null) ? true : false;
        }
    }
}
