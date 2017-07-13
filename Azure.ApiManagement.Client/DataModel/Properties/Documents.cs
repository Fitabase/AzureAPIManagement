using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Properties
{
    public static class Documents
    {
        public static string BASE = @"C:\Users\inter\Desktop\";

        public static string API_DOC => BASE + "api_doc.json";

        public static string API_OPERATION_DOC => BASE + "apioperation_doc.json";




        public static void Write(string outputFile, Object obj)
        {
            using (StreamWriter file = File.CreateText(outputFile))
            {
                string header = "";
                if(obj is ICollection)
                {
                    header = obj.GetType().GetGenericArguments().Single().ToString();
                } else
                {
                    header = obj.GetType().ToString();
                }
                file.WriteLine(header + " JSON");
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                file.WriteLine(json);
            }
        }
    }
}
