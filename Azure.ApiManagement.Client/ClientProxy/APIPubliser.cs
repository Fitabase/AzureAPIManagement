using Fitabase.Azure.ApiManagement.DataModel.Properties;
using Fitabase.Azure.ApiManagement.Model;
using Fitabase.Azure.ApiManagement.Swagger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    public class APIPubliser
    {
        private ManagementClient Client { get; set; }
        private string InputFile { get; set; }
        private string OutputFile { get; set; }


        public APIPubliser(string inputFile, string outputFile)
        {
            this.InputFile = inputFile;
            this.OutputFile = outputFile;
            Client = new ManagementClient();
        }


        /// <summary>
        /// Publish the api
        /// </summary>
        public void Publish()
        {
            if(InputFile == null)
            {
                throw new ArgumentException("Filepath is required");
            }
            var swagger = new SwaggerJsonFileReader().GetSwaggerFromFile(InputFile);
            swagger.Host = Client.GetEndpoint();
            var apiCollection = new SwaggerApiComposer(swagger).Compose();
            
            foreach (API api in apiCollection)
            {
                Documents.Write(Documents.API_DOC, api);
                Documents.Write(Documents.API_OPERATION_DOC, api.Operations);
            }
        }
    }
}
