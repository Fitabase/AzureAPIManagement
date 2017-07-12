using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    class SwaggerApiCaster
    {
        public SwaggerObject Swagger;

        public API Cast()
        {
            string name = "name";
            string description = "";
            string serviceUrl = "";
            string path = "";
            List<string> protocols = null;
            AuthenticationSettings authentication = null;
            SubscriptionKeyParameterNames customNames = null;

            API api = new API(name, description,
                   serviceUrl, path,
                   protocols,
                   authentication,
                   customNames);
            return null;
        }
    }
}
