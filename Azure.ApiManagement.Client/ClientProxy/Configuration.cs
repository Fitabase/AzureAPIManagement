using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    public class Configuration
    { 
        public bool WriteToFile { get; set; }

        public Configuration()
        {
            WriteToFile = false;
        }
    }
}
