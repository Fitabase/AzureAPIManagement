using Newtonsoft.Json;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    /// <summary>
    /// Represents the result of an failed service operation
    /// https://msdn.microsoft.com/en-us/library/azure/dn776332.aspx#error
    /// </summary>
    public class ErrorOperationResult : OperationResult
    {
        /// <summary>
        /// The error body containing the details of the error.
        /// </summary>
        [JsonProperty("error")]
        public ErrorData Error { get; set; }

        public override bool IsSuccessfull()
        {
            return false;
        }
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
        public ErrorValidationDetails ValidationDetails { get; set; }
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
