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
            UrlPath = urls[2];

            //FilePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json";
            //FilePath = @"C:\Users\inter\Desktop\FitabaseAPI/bodyTrace.json";

            _SwaggerReader = new SwaggerUrlReader(UrlPath);
            _SwaggerObject = _SwaggerReader.GetSwaggerObject();
        }



        //#region APIResponseBuilder TestCases

        //[TestMethod]
        //public void APIResponseBuilder()
        //{

        //    Dictionary<string, Schema> schemas = null;
        //    APIBuilder.APISampleDataBuilder builder = new APIBuilder().GetBuilder(schemas);
        //    builder.Build();
        //}

        //#endregion APIResponseBuilder TestCases




        #region APIServiceUrlBuilder TestCases

        [TestMethod]
        public void ServiceUrlBuilder()
        {
            PathData pathdata =_SwaggerObject.Paths.First().Value;

            APIBuilder.APIServiceUrlBuilder serviceBuilder = new APIBuilder().GetBuilder(pathdata);

            OperationMethod operationMethod = serviceBuilder.GetOperationMethod();
            PrintMessage.Debug(this, operationMethod);

            PrintMessage.Debug(this, operationMethod.OperationId);

            RequestMethod requestMethod = serviceBuilder.GetRequestMethod();
            PrintMessage.Debug(this, requestMethod.ToString());

            Parameter[] parameters = operationMethod.Parameters;
            PrintMessage.Debug(this, parameters);

            string url = serviceBuilder.BuildURN();
            PrintMessage.Debug(this, url);
        }

        


        [TestMethod]
        public void GetPathData()
        {
            Dictionary<string, PathData> paths = _SwaggerObject.Paths;
            foreach(KeyValuePair<string, PathData> path in paths)
            {
                Assert.IsNotNull(path.Key);
                Assert.IsNotNull(path.Value);
                PrintMessage.Debug(this, path.Key);
                PrintMessage.Debug(this, path.Value);
            }
        }

        #endregion APIServiceUrlBuilder TestCases





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
            catch(SwaggerResourceException ex)
            {
                PrintMessage.Debug(this, ex.Message);
            }
        }

        #endregion Publish an API



#region SwaggerObject TestCases

        [TestMethod]
        public void GetSwaggerFromUrl()
        {
            SwaggerObject swagger = new SwaggerUrlReader(UrlPath).GetSwaggerObject();
            Assert.IsNotNull(swagger);

            PrintMessage.Debug(this, swagger);
        }

        [TestMethod]
        public void Dictionary()
        {
            PrintMessage.Debug(this, _SwaggerObject.Definitions);
        }
        
    }
#endregion SwaggerObject TestCases
}
