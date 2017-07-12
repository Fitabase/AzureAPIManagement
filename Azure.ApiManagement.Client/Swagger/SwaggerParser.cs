using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    public class SwaggerParser
    {
        
        public SwaggerObject SwaggerObject { get; set; }
        public string Json { get; set; }
        public SwaggerObject Root { get; set; }

        public SwaggerParser(string filePath = @"C:\Repositories\AzureAPIManagement\Azure.ApiManagement.Test\SwaggerObject.json")
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File doesn't exist");
            }
            Json = File.ReadAllText(filePath);
            //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(Json));
            //this.SwaggerObject = Utility.DeserializeToJson<SwaggerObject>(Json);
            //this.SwaggerObject = JsonConvert.DeserializeObject<SwaggerObject>(Json);

            var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            Root = DataContractJsonSerializerHelper.GetObject<SwaggerObject>(Json, settings);
        }
        public static class DataContractJsonSerializerHelper
        {
            public static T GetObject<T>(string json, DataContractJsonSerializer serializer = null)
            {
                using (var stream = GenerateStreamFromString(json))
                {
                    var obj = (serializer ?? new DataContractJsonSerializer(typeof(T))).ReadObject(stream);
                    return (T)obj;
                }
            }

            public static T GetObject<T>(string json, DataContractJsonSerializerSettings settings)
            {
                return GetObject<T>(json, new DataContractJsonSerializer(typeof(T), settings));
            }

            private static MemoryStream GenerateStreamFromString(string value)
            {
                return new MemoryStream(Encoding.Unicode.GetBytes(value ?? ""));
            }
        }



    }


}
