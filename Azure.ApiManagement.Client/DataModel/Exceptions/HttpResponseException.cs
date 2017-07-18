using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// Represents the result of an failed service operation
    /// https://msdn.microsoft.com/en-us/library/azure/dn776332.aspx#error
    /// </summary>
    public class HttpResponseException : Exception
    {

        public ErrorResponse ErrorResponse
        {
            get
            {
                return JsonConvert.DeserializeObject<ErrorResponse>(Message);
            }
        }

        public HttpResponseException(string message, Exception exception, HttpStatusCode statusCode) 
                : base(message, exception)
        {
            this.StatusCode = statusCode;
        }

        //public HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        
        public HttpStatusCode StatusCode { get; set; }
        
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public ErrorData ErrorData { get; set; }

    }

    /// <summary>
    /// This class represents the error-body representation.
    /// https://msdn.microsoft.com/en-us/library/azure/dn776332.aspx#error-body
    /// </summary>
    public class ErrorData
    {
        /// <summary>
        /// Service-defined error code. This code serves as a sub-status for the HTTP error code specified in the response.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Description of the error.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// In case of validation errors this field contains a list of invalid fields sent in the request.
        /// </summary>
        [JsonProperty("details")]
        public List<ErrorValidationDetails> ValidationDetails { get; set; }
    }

    /// <summary>
    /// This class represents the error-detail representation.
    /// https://msdn.microsoft.com/en-us/library/azure/dn776332.aspx#ErrorDetail
    /// </summary>
    public class ErrorValidationDetails
    {
        /// <summary>
        /// Property level error code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Human readable representation of the property-level error.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Optional. Property name.
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }
    }
}
