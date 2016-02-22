using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public interface IOperationResult
    {
        HttpStatusCode StatusCode { get; set; }

        string StatusMessage { get; set; }

        bool IsSuccessfull();
    }

    public class OperationResult : IOperationResult
    {
        public HttpStatusCode StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public virtual bool IsSuccessfull()
        {
            return true;
        }
    }
}
