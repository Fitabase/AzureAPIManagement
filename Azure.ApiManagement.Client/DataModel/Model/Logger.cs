using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Logger : EntityBase
    {
        protected override string UriIdFormat => "/loggers";

        public static Logger Create(string loggerType, string name, string description, object credentials, bool isBuffered = true)
        {
            Logger logger = new Logger();
            logger.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.Logger);
            logger.LoggerType = loggerType;
            logger.Name = name;
            logger.Description = description;
            logger.Credentials = credentials;
            logger.IsBuffered = isBuffered;
            return logger;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("loggerType")]
        public string LoggerType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("credentials")]
        public object Credentials { get; set; }

        [JsonProperty("isBuffered")]
        public bool IsBuffered { get; set; }
       
    }
}
