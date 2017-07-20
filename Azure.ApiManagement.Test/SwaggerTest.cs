using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.ClientProxy;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Linq;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {

        private string ResourcePath;



        [TestInitialize]
        public void Init()
        {
            //Resource_Path = @"http://localhost:2598/swagger/docs/BodyTrace";
            ResourcePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
        }



        [TestMethod]
        public void PublishSwaggerAPI()
        {
            //var inputFile  = @"C:\Users\inter\Desktop\FitabaseAPI\bodyTrace.json";
            //var outputFile = @"C:\Users\inter\Desktop\FitabaseAPI\result";

            AbstractSwaggerReader swaggerReader = new SwaggerUrlReader(ResourcePath);
            APIConfiguration configuration = new APIConfiguration(swaggerReader);
            try
            {
                APIPubliser publiser = new APIPubliser(configuration);
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

        [TestMethod]
        public void SwaggerObject()
        {
            //AbstractSwaggerReader reader = new SwaggerUrlReader(Resource_Path);
            AbstractSwaggerReader reader = new SwaggerFileReader(ResourcePath);
            SwaggerObject swagger = reader.GetSwaggerComponents();
            PrintMessage.Debug(this, swagger);
        }

        [TestMethod]
        public void SwaggerReader()
        {
            AbstractSwaggerReader reader = new SwaggerUrlReader(ResourcePath);
            reader.GetSwaggerComponents();
        }

    }
}
