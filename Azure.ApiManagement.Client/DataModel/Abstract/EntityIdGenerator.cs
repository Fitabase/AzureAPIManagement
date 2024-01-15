using System;
using System.Text;

namespace Fitabase.Azure.ApiManagement.Model
{
    class EntityIdGenerator
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
