using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Fitabase.Azure.ApiManagement.Swagger;
using Fitabase.Azure.ApiManagement.Swagger.Models;
using System;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    /// <summary>
    /// </summary>
    public class APIPublisher
    {
        private ManagementClient Client { get; set; }
        private APIConfiguration Configuration { get; set; }
        private Queue<APIOperation> FailQueue { get; set; }


        public APIPublisher(APIConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Client = new ManagementClient();
        }


        
        /// <summary>
        /// 
        /// </summary>
        public void Publish()
        {
            if (Configuration == null)
                throw new InvalidEntityException("Missing API Configuration");
            
            SwaggerObject swagger = this.Configuration.SwaggerReader.GetSwaggerObject();    // Get Swagger Components

            // Get API entity from Swagger
            API api = new APIComposer(swagger).Compose();
            PublishAPI(api);
            PublishAPIOperations(api);

            
            //WriteToFile(Configuration.OutputFolder + Documents.API_DOC, api);
            //WriteToFile(Configuration.OutputFolder + Documents.API_OPERATION_DOC, api.Operations);
        }
        
        private void Print(object ob)
        {
            PrintMessage.Debug(this.GetType().Name, ob);
        }
        
        private void PublishAPI(API api)
        {
            Client.CreateAPI(api);
        }

        private void PublishAPIOperations(API api)
        {
            foreach (APIOperation operation in api.Operations)
            {
                try
                {
                    Client.CreateAPIOperation(api, operation);
                }
                catch (Exception ex)
                {
                    PrintMessage.Debug(this.GetType().Name, ex);
                    if (FailQueue == null)
                        FailQueue = new Queue<APIOperation>();
                    FailQueue.Enqueue(operation);
                }
            }

        }
        


        //private void WriteToFile(string ouputFile, object obj)
        //{
        //    if(Configuration.WriteToFile)
        //    {
        //        Documents.Write(ouputFile, obj);
        //    }
        //}
    }
}
