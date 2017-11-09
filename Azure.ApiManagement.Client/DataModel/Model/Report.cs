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
            Report report = new Report();
            report.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.REPORT);
            return report;
        }

        protected override string UriIdFormat { get { return "/reports/"; } }
    }
}
