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
        private string OutputFolder { get; set; }


        public APIPubliser(string inputFile, string outputFolder)
        {
            this.InputFile = inputFile;
            this.OutputFolder = outputFolder;
            Client = new ManagementClient();
        }


        /// <summary>
        /// Publish the api
        /// </summary>
        public void Publish()
        {
            if(InputFile == null)
            {
                throw new ArgumentException("FilePath is required");
            }
            var swagger = new JsonFileReader().GetSwaggerFromFile(InputFile);
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(swagger));
            //swagger.Host = Client.GetEndpoint();
            var api = new APIComposer(swagger).Compose();


            List<APIOperation> list = api.Operations.ToList();
            PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(list.ElementAt(0).Responses));
            
            
            //Documents.Write(OutputFolder + Documents.API_DOC, api);
            //Documents.Write(OutputFolder + Documents.API_OPERATION_DOC, api.Operations);
        }
    }
}
