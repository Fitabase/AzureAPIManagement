using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class SwaggerResourceException : Exception
    {
        public SwaggerResourceException(string message) : base(message) { }
    }
}
