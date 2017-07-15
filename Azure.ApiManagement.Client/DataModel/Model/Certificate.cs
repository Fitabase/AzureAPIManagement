using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Certificate : EntityBase
    {
        public Certificate()
        {
        }

        protected override string UriIdFormat { get { return "/certificates/"; } }
    }
}
