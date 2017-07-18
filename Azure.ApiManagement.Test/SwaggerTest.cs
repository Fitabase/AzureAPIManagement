using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.ClientProxy;
using Fitabase.Azure.ApiManagement.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
            try
            {
                APIPubliser publiser = new APIPubliser(inputFile, outputFile);
                publiser.Publish();
            }
            catch (HttpResponseException ex)
            {
                PrintMessage.Debug(this, ex.StatusCode);
                PrintMessage.Debug(this, ex.ErrorResponse);
            } catch(InvalidEntityException ex) 
                {
                PrintMessage.Debug(this, ex.Message);
            }

        }

    }
}
