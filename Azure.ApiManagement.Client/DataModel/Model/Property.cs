using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Property : EntityBase
    {
        public Property()
        {
        }

        protected override string UriIdFormat { get { return "/properties/"; } }
    }
}
