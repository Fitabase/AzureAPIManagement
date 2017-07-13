using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class SwaggerObject
    {
        [JsonProperty("swagger")]
        public string Swagger { get; set; }
        [JsonProperty("info")]
        public SwaggerInfo Info { get; set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("schemes")]
        public string[] Schemes { get; set; }
        [JsonProperty("paths")]
        public Dictionary<string, PathData> Paths { get; set; }
        //[JsonProperty("definitions")]
        //public List<Definition> Definitions { get; set; }



        public string GetDefinition()
        {
            return "";
        }

        public class Definition
        {

          
        }


        /// <summary>
        /// This class contains api path data
        /// </summary>
        public class PathData
        {
            [JsonProperty("post")]
            public PostMethod Post { get; set; }

            [JsonProperty("get")]
            public GetMethod Get { get; set; }




            public class GetMethod
            {
                
            }


            /// <summary>
            /// 
            /// </summary>
            public class PostMethod
            {
                [JsonProperty("tags")]
                public string[] Tags { get; set; }

                [JsonProperty("operationId")]
                public string OperationId { get; set; }

                [JsonProperty("consumes")]
                public string[] Consumes { get; set; }

                [JsonProperty("produces")]
                public string[] Produces { get; set; }

                [JsonProperty("responses")]
                public Dictionary<string, SwaggerResponse> Responses { get; set; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class SwaggerInfo
        {
            [JsonProperty("version")]
            public string Version { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
        }



        /// <summary>
        /// Contains response info for code
        /// </summary>
        public class SwaggerResponse
        {
            [JsonProperty("description")]
            public string Description { get; set; }

        }

    }
}
