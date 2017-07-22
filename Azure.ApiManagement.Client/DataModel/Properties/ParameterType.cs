using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{

    public enum ParameterType
    {
        NUMBER, STRING, BOOLEAN, DATETIME, INTEGER
    }

    public static class TemplateParameterType
    {
        public static string GetType(ParameterType type)
        {
            return type.ToString().ToLower();
        }

        
    }
}
