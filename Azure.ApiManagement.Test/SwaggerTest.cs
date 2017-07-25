using Fitabase.Azure.ApiManagement;
using Fitabase.Azure.ApiManagement.ClientProxy;
using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Fitabase.Azure.ApiManagement.Utilities;
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

            //FilePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
            //FilePath = @"C:\Users\inter\Desktop\FitabaseAPI/bodyTrace.json";

            _SwaggerReader = new SwaggerUrlReader(UrlPath);
            _SwaggerObject = _SwaggerReader.GetSwaggerObject();
        }



        #region Publish an API

        [TestMethod]
        public void PublishSwaggerAPI()
        {
            try
            {
                APIConfiguration configuration = new APIConfiguration(_SwaggerReader);
                APIPublisher publiser = new APIPublisher(configuration);
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
            catch (SwaggerResourceException ex)
            {
                PrintMessage.Debug(this, ex.Message);
            }
        }

        #endregion Publish an API

    }
}
