using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// A declaration of which security schemes are applied for the API as a whole. 
    /// The list of values describes alternative security schemes that can be used 
    /// (that is, there is a logical OR between the security requirements). Individual 
    /// operations can override this definition.
    /// </summary>
    public class Security
    {
        Dictionary<string, string[]> Name { get; set; }
    }

}
