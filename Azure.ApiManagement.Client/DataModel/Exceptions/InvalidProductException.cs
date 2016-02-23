using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class InvalidProductException : Exception
    {
        public InvalidProductException(string message) : base(message) { }
    }
}
