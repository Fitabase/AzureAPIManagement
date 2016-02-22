using System;
using System.Net.Http;

namespace SmallStepsLabs.Azure.ApiManagement.Utilities
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class RestCallAttribute : Attribute
    {
        public string UrlMapping { get; set; }

        public string Method { get; set; }

        internal RestCallAttribute(string urlMap, string method)
        {
            this.UrlMapping = urlMap;
            this.Method = method;
        }
    }
}
