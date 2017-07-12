using Fitabase.Azure.ApiManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.ApiManagement.Test
{
    [TestClass]
    public class SwaggerTest
    {
        [TestMethod]
        public void GetSwaggerObject()
        {
            var parser = new SwaggerParser();
            var swagger = parser.Root;

            //PrintMessage.Debug(this.GetType().Name, swagger.swagger);
            //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(swagger.info));
            //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(swagger.host));
            //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(swagger.schemes));

            var paths = swagger.paths;
            foreach (KeyValuePair<string, PathData> c in paths)
            {
                //PrintMessage.Debug(this.GetType().Name, c.Key);
                var value = c.Value;
                var response = value.post.responses;
                //PrintMessage.Debug(this.GetType().Name, Utility.SerializeToJson(response));
                foreach(KeyValuePair<string, SwaggerResponse> r in response)
                {
                    PrintMessage.Debug(this.GetType().Name, r.Key);
                    PrintMessage.Debug(this.GetType().Name, r.Value.description);
                }
            }
        }

    }
}
