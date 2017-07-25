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
        public AbstractSwaggerReader SwaggerReader { get; set; }

      
        public APIConfiguration(AbstractSwaggerReader swaggerReader)
        {
            this.SwaggerReader = swaggerReader;
        }
    }
}
