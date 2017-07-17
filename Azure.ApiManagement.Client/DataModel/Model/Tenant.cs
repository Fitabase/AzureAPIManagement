using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Tenant : EntityBase
    {
        public static Tenant Create()
        {
            try
            {
                Tenant tenant = new Tenant();
                tenant.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.TENANT);

                return tenant;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        protected override string UriIdFormat { get { return "/tenants/"; } }

        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }
        [JsonProperty("secondaryKey")]
        public string SecondaryKey { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
    }
}
