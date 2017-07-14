using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Subscription : EntityBase
    {
        public Subscription() : base("Subscription")
        {
        }

        protected override string UriIdFormat { get { return "/subscriptions/"; } }

        /// <summary>
        /// The user resource identifier of the subscription owner. 
        /// The value is a valid relative URL in the format of users/{uid} where {uid} is a user identifier.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// The product resource identifier of the subscribed product. 
        /// The value is a valid relative URL in the format of products/{pid} where {pid} is a product identifier.
        /// </summary>
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        /// <summary>
        /// The state of the subscription. 
        /// </summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionState State { get; set; }

        /// <summary>
        /// The primary subscription key. If omitted from the request it will be automatically generated. Maximum length is 256 characters.
        /// </summary>
        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        /// <summary>
        /// The secondary subscription key. If omitted from the request it will be automatically generated. Maximum length is 256 characters.
        /// </summary>
        [JsonProperty("secondaryKey")]
        public string SecondaryKey { get; set; }

    }
}
