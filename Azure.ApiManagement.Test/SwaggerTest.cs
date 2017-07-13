using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.ClientProxy;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {
        [TestMethod]
        public void GetSwaggerObject()
        {
            var inputFile = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
            var outputFile = @"C:\Users\inter\Desktop\output.json";
            APIPubliser publiser = new APIPubliser(inputFile, outputFile);
            publiser.Publish();
            
        }

    }
}
