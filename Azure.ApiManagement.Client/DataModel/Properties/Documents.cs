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

        public static string API_DOC = @"\api_doc.json";

        public static string API_OPERATION_DOC = @"\apioperation_doc.json";




        public static void Write(string outputFile, Object obj)
        {
            using (FileStream fileStream = new FileStream(outputFile, FileMode.Append, FileAccess.Write))
            {

                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    string header = "";
                    if (obj is ICollection)
                    {
                        header = obj.GetType().GetGenericArguments().Single().ToString();
                    }
                    else
                    {
                        header = obj.GetType().ToString();
                    }
                    streamWriter.WriteLine(header + " JSON:\t\t\t\t\t" + DateTime.Now);
                    string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    streamWriter.WriteLine(json + "\n\n\n");
                }
            }
        }
    }
}
