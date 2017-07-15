using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class EntityIdGenerator
    {
        public static string GenerateIdSignature(string prefixId)
        {
            return new StringBuilder()
                        .Append(prefixId).Append("_")
                        .Append(Guid.NewGuid().ToString("N"))
                        .ToString();
        }

    }
}
