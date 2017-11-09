using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    internal static class Constants
    {
        internal static readonly string HTTP = "http://";
        internal static readonly string HTTPS = "https://";


        internal static class IdPrefixTemplate
        {
            internal static readonly string API          = "api";
            internal static readonly string APIOPERATION = "operation";
            internal static readonly string GROUP        = "group";
            internal static readonly string PRODUCT      = "product";
            internal static readonly string PROPERTY     = "property";
            internal static readonly string REPORT       = "report";
            internal static readonly string SUBSCRIPTION = "subscription";
            internal static readonly string TENANT       = "tenant";
            internal static readonly string USER         = "user";
            internal static readonly string Logger       = "logger";
        }


        internal static readonly string FITABSE_TOKEN = "FitabaseToken";
        //internal static readonly string FITABASE_API_VERSION = "Fitabase-API-Version";

        

        internal static class ApiManagement
        {
            internal static readonly string AccessToken = "SharedAccessSignature";


            internal static class Url
            {
                internal static readonly string ServiceFormat = "https://{0}.management.azure-api.net/";

                internal static readonly string VersionQuery = "api-version";

                internal static readonly string FilterQuery = "$filter";
            }

            internal static class Versions
            {
                internal static readonly string Feb2014 = "2014-02-14-preview";
            }

            internal static class Headers
            {
                internal static readonly string ETagMatch = "If-Match";
            }
        }

        internal static class MimeTypes
        {
            ///<summary>JavaScript Object Notation JSON; Defined in RFC 4627</summary>
            public const string ApplicationJson = "application/json";

            public const string ApplicationXmlPolicy = "application/vnd.ms-azure-apim.policy+xml";
        }
    }
}
