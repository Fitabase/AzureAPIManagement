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

        private Configuration Configuration { get; set; }

        public APIPubliser(string inputFile, string outputFolder, 
                            Configuration configuration = null)
        {
            if (configuration == null)
                configuration = new Configuration();

            this.InputFile = inputFile;
            this.OutputFolder = outputFolder;
            this.Configuration = configuration;
            this.Client = new ManagementClient();
            this.Configuration = configuration;
        }


        
        public void Publish()
        {
            if(InputFile == null)
            {
                throw new ArgumentException("FilePath is required");
            }
            var swagger = new JsonFileReader().GetSwaggerFromFile(InputFile);
            swagger.Host = Client.GetEndpoint();
            var api = new APIComposer(swagger).Compose();

            
            Client.CreateAPI(api);
            foreach(APIOperation operation in api.Operations)
            {
                Client.CreateAPIOperation(api.Id, operation);
            }
            WriteToFile(OutputFolder + Documents.API_DOC, api);
            WriteToFile(OutputFolder + Documents.API_OPERATION_DOC, api.Operations);
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
