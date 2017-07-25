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
        /// This method is used to publish a swagger to APIM
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
                catch (Exception)
                {
                    if (FailQueue == null)
                        FailQueue = new Queue<APIOperation>();
                    FailQueue.Enqueue(operation);
                }
            }

        }
        
    }
}
