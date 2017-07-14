using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Logger : EntityBase
    {
        public Logger() : base("Logger")
        {
        }

        protected override string UriIdFormat { get { return "/loggers/"; } }
    }
}
