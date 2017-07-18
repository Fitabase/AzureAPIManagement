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
            var inputFile  = @"C:\Users\inter\Desktop\FitabaseAPI\bodyTrace.json";
            var outputFile = @"C:\Users\inter\Desktop\FitabaseAPI\result";
            APIPubliser publiser = new APIPubliser(inputFile, outputFile);
            publiser.Publish();
            var errorStack = ManagementClient.ErrorStack;
            if(errorStack != null)
            {
                foreach(var error in errorStack)
                {
                    PrintMessage.Debug(this.GetType().Name, error);
                }
            }
        }

    }
}
