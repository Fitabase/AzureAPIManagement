using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {
        private string UrlPath;
        private AbstractSwaggerReader _SwaggerReader;
        private SwaggerObject _SwaggerObject;
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
            
            _SwaggerReader = new SwaggerUrlReader(UrlPath);
            _SwaggerObject = _SwaggerReader.GetSwaggerObject();
            _Client = new ManagementClient(@"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\APIMKeys.json");
        }



        #region Publish an API

        [TestMethod]
        public void PublishSwaggerAPI()
        {
            UrlPath = "localhost:2598/swagger/docs/BodyTrace";

            try
            {
                APIBuilder builder = APIBuilder.GetBuilder(UrlPath);
                API api = builder.BuildAPIAndOperations();
                //_Client.CreateAPI(api);
                //foreach(APIOperation operation in api.Operations)
                //{
                //    _Client.CreateAPIOperation(api, operation);
                //}
                PrintMessage.Debug(this, api);
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
            catch (SwaggerResourceException ex)
            {
                PrintMessage.Debug(this, ex.Message);
            }
        }

        #endregion Publish an API

    }
}
