using Swashbuckle.Swagger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.ClientProxy
{
    public class ParameterBuilder
    {
        private IParameter _parameter;
        public ParameterBuilder(IParameter parameter)
        {
            this._parameter = parameter;
        }

        public void Build()
        {
            Console.WriteLine();
        }
    }
}
