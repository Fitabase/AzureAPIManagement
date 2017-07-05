using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.ClientProxy
{
    internal static class BaseUrls
    {
        internal static string BaseUrl => "https://unittestsfitabase.management.azure-api.net";
        internal static string Products => BaseUrl + "/products";
        internal static string Subscriptions => BaseUrl + "/subscriptions";
        internal static string Apis => BaseUrl + "/apis";
    }
}
