using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    /// <summary>
    /// This class
    /// </summary>
    public class APIPubliser
    {
        private ManagementClient Client { get; set; }
        private APIConfiguration Configuration { get; set; }

        public APIPubliser(APIConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Client = new ManagementClient();
            this.Configuration = configuration;
        }


        
        /// <summary>
        /// 
        /// </summary>
        public void Publish()
        {
            if (Configuration == null)
                throw new InvalidEntityException("Missing API Configuration");

            // Get Swagger Components
            SwaggerObject swagger = this.Configuration.SwaggerReader.GetSwaggerObject();

            //PrintMessage.Debug(this, swagger);


            // Get API entity from Swagger
            API api = new APIComposer(swagger).Compose();
            //PrintMessage.Debug(this, api);
            Client.CreateAPI(api);


            //foreach(APIOperation operation in api.Operations)
            //{
            //    Client.CreateAPIOperation(api.Id, operation);
            //}
            //WriteToFile(Configuration.OutputFolder + Documents.API_DOC, api);
            //WriteToFile(Configuration.OutputFolder + Documents.API_OPERATION_DOC, api.Operations);
        }

        private void WriteToFile(string ouputFile, object obj)
        {
            if(Configuration.WriteToFile)
            {
                Documents.Write(ouputFile, obj);
            }
        }
    }
}
