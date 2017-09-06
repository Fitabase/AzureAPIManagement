using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {
        private string UrlPath;
        private ManagementClient _Client;

        [TestInitialize]
        public void Init()
        {
            string[] urls = {
                @"http://localhost:2598/swagger/docs/BatchExport",          // 0
                @"http://localhost:2598/swagger/docs/BodyTrace",            // 1
                @"http://localhost:2598/swagger/docs/DailyActivity",        // 2
                @"http://localhost:2598/swagger/docs/Echo",                 // 3
                @"http://localhost:2598/swagger/docs/Error",                // 4
                @"http://localhost:2598/swagger/docs/Profiles",             // 5
                @"http://localhost:2598/swagger/docs/Projects",             // 6
                @"http://localhost:2598/swagger/docs/Sync",                 // 7
                @"http://localhost:2598/swagger/docs/TimeSeries",           // 8
                @"http://localhost:2598/swagger/docs/Values"                // 9
            };
            UrlPath = urls[0];

            _Client = new ManagementClient(@"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\APIMKeys.json");

        }

        #region Test APIBuilder
        [TestMethod]
        public void BuildSwaggerWithSwaggerReader()
        {
            string urlPath = @"http://localhost:2598/swagger/docs/BodyTrace";
            AbstractSwaggerReader swaggerReader = new SwaggerUrlReader(urlPath);
            APIBuilder builder = APIBuilder.GetBuilder(swaggerReader);
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void BuildSwaggerWithURL()
        {
            string urlPath = @"http://localhost:2598/swagger/docs/BodyTrace";
            APIBuilder builder = APIBuilder.GetBuilder(urlPath);
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void BuildAPIWithSwaggerReader()
        {
            string urlPath = @"http://localhost:2598/swagger/docs/BatchExport";
            AbstractSwaggerReader swaggerReader = new SwaggerUrlReader(urlPath);
            APIBuilder builder = APIBuilder.GetBuilder(swaggerReader);
            API api = builder.BuildAPIAndOperations();
            Assert.IsNotNull(api);
            Assert.IsNotNull(api.Id);
            Assert.IsNotNull(api.Operations);
            Assert.IsTrue(api.Operations.Count > 0);
        }

        #endregion Test APIBuilder


        #region Publish an API

        [TestMethod]
        public void PublishSwaggerAPI()
        {
            string url = @"http://localhost:2598/swagger/docs/Values";
            APIBuilder builder = APIBuilder.GetBuilder(url);
            API api = builder.BuildAPIAndOperations();
            var entity = _Client.CreateAPIAsync(api).Result;
            Assert.IsNotNull(entity);
            foreach(var operation in api.Operations)
            {
                var entityOperation = _Client.CreateAPIOperationAsync(entity, operation).Result;
                Assert.IsNotNull(entityOperation);
            }
        }
        

        #endregion Publish an API



        [TestMethod]
        public void ReadLocalSwaggerFile()
        {
            var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            string filePath = @"C:\Users\inter\Downloads\swagger.json";
            AbstractSwaggerReader swaggerReader = new SwaggerFileReader(filePath);
            APIBuilder builder = APIBuilder.GetBuilder(swaggerReader);

            API api = builder.BuildAPIAndOperations();
            API entity = _Client.CreateAPIAsync(api).Result;
            foreach (APIOperation o in api.Operations)
            {
                try
                {
                    APIOperation e = _Client.CreateAPIOperationAsync(entity, o).Result;
                }
                catch (Exception)
                {

                }
            }
        }



        



        [TestMethod]
        public void ReadLocalOperation()
        {

            string filePath = @"C:\Users\inter\Downloads\swaggerOperation.json";
            AbstractSwaggerReader swaggerReader = new SwaggerFileReader(filePath);
            APIBuilder builder = APIBuilder.GetBuilder(swaggerReader);

            ICollection<APIOperation> operations = builder.BuildAPIAndOperations().Operations;


            string apiId = "api_577edd5ee62543d297bd5d568af78a82";
            API entity = _Client.GetAPIAsync(apiId).Result;
            string json = JsonConvert.SerializeObject(entity.Operations);
            foreach (APIOperation o in operations)
            {
                try
                {
                    APIOperation e = _Client.CreateAPIOperationAsync(entity, o).Result;
                }
                catch (Exception)
                {

                }
            }

        }
    }
}
