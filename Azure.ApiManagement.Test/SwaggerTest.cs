using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.ClientProxy;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {

        private string FilePath;
        private string UrlPath;


        [TestInitialize]
        public void Init()
        {
            UrlPath = @"http://localhost:2598/swagger/docs/BodyTrace";
            FilePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
        }



        [TestMethod]
        public void PublishSwaggerAPI()
        {
            try
            {
                AbstractSwaggerReader swaggerReader = new SwaggerUrlReader(UrlPath);
                APIConfiguration configuration = new APIConfiguration(swaggerReader);
                APIPubliser publiser = new APIPubliser(configuration);
                publiser.Publish();
            }
            catch (HttpResponseException ex)
            {
                PrintMessage.Debug(this, ex.StatusCode);
                PrintMessage.Debug(this, ex.ErrorResponse.ErrorData.ValidationDetails.ElementAt(0).Message);
                PrintMessage.Debug(this, ex.ErrorResponse);
            }
            catch (InvalidEntityException ex) 
            {
                PrintMessage.Debug(this, ex.Message);
            }
            catch(SwaggerResourceException ex)
            {
                PrintMessage.Debug(this, ex.Message);
            }
        }




        [TestMethod]
        public void GetSwaggerFromUrl()
        {
            SwaggerObject swagger = new SwaggerUrlReader(UrlPath).GetSwaggerObject();
            PrintMessage.Debug(this, swagger);
        }



        [TestMethod]
        public void GetSwaggerFromFile()
        {
            SwaggerObject swagger = new SwaggerFileReader(FilePath).GetSwaggerObject();
            //PrintMessage.Debug(this, swagger);

            // Return specific api path
            string query = "/user/login";
            var path = swagger.Paths.Where(x => x.Key == query);
            PrintMessage.Debug(this, path);
        }


        [TestMethod]
        public void SwaggerReader()
        {
            AbstractSwaggerReader reader = new SwaggerUrlReader(FilePath);
            reader.GetSwaggerObject();
        }

    }
}
