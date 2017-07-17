using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Report : EntityBase
    {
        public static Report Create()
        {
            try
            {
                Report report = new Report();
                report.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.REPORT);
                return report;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        protected override string UriIdFormat { get { return "/reports/"; } }
    }
}
