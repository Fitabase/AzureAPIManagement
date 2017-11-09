using System;

namespace Fitabase.Azure.ApiManagement.Model.Exceptions
{
    public class SwaggerResourceException : Exception
    {
        public SwaggerResourceException(string message) : base(message) { }
    }
}
