using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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



        #region Publish an API

        [TestMethod]
        public void PublishSwaggerAPI()
        {
            APIBuilder builder = APIBuilder.GetBuilder(UrlPath);
            API api = builder.BuildAPIAndOperations();

            string json = JsonConvert.SerializeObject(api.Operations, Formatting.None, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
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

            List<APIOperation> operations = api.Operations.ToList();
            List<string> operationIds = new List<string>();

            //string json = JsonConvert.SerializeObject(api.Operations);
            //json = JsonConvert.SerializeObject(operationIds);

            API entity = _Client.CreateAPIAsync(api).Result;
            foreach(APIOperation operation in operations)
            {
                var task = _Client.CreateAPIOperationAsync(entity, operation);
                task.Wait();
            }
        }
    }
}
