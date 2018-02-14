using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Policy : EntityBase
    {
        
        /// <summary>
        /// Policies as XML (same as in Azure Portal settings in Code View mode)
        /// </summary>
        [JsonProperty("policyContent")]
        public string PolicyContent { get; set; }

        protected override string UriIdFormat => "/policy";
    }
}
