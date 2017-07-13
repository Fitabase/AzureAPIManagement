using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public interface ISwaggerFileReader
    {
        /// <summary>
        /// Read a file and convert to string
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        SwaggerObject GetSwaggerFromFile(string filePath);
    }
}
