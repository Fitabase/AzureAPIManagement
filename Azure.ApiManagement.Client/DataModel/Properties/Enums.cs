using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{

    public enum ProductState
    {
        notPublished,   // Non-published products are visible only to administrators.
        published       // Published products are discoverable by developers on the developer portal.
    }

}
