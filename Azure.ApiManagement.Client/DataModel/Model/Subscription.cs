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
        public static Subscription Create(string userId, string productId, string name,
                                           string primaryKey = null, string secondaryKey = null,
                                           SubscriptionState state = SubscriptionState.submitted)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(userId) 
                    && userId.StartsWith(Constants.IdPrefixTemplate.USER))
                    throw new ArgumentException("subscription's userId is required");
                if (String.IsNullOrWhiteSpace(productId)
                    && userId.StartsWith(Constants.IdPrefixTemplate.PRODUCT))
                    throw new ArgumentException("subscription's product is required");
                if (String.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("subscription's name is required");

                Subscription subscription = new Subscription();
                subscription.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.SUBSCRIPTION);
                subscription.UserId = userId;
                subscription.ProductId = productId;
                subscription.Name = name;
                subscription.PrimaryKey = primaryKey;
                subscription.SecondaryKey = secondaryKey;
                subscription.State = state;

                return subscription;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        protected override string UriIdFormat { get { return "/subscriptions/"; } }
        


        [JsonProperty("userId")]
        public string UserId { get; set; }      // User (user id path) for whom subscription is being created in form /users/{uid}

        [JsonProperty("productId")]
        public string ProductId { get; set; }       // Product (product id path) for which subscription is being created in form /products/{productid} 

        [JsonProperty("name")]
        public string Name { get; set; }            // Subscription name.
        
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionState State { get; set; }    // The state of the subscription. 
        
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }       // Subscription creation date. 

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }         // Subscription activation date.

        [JsonProperty("expirationDate")]
        public DateTime? ExpirationDate { get; set; }    // Subscription expiration date.

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }           // Date when subscription was cancelled or expired.

        [JsonProperty("notifiactionDate")]
        public DateTime? NotificationDate { get; set; }  // Upcoming subscription expiration notification date.

        [JsonProperty("stateComment")]
        public string StateComment { get; set; }        // Optional subscription comment added by an administrator.

        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }          // Primary subscription key. If not specified during request key will be generated automatically.
        
        [JsonProperty("secondaryKey")]
        public string SecondaryKey { get; set; }        // Secondary subscription key. If not specified during request key will be generated automatically.

    }



    public enum SubscriptionState
    {
        active,     // subscription is active
        suspended,  // subscription is blocked, and the subscriber cannot call any APIs of the product.
        submitted,  // subscription request has been made by the developer, but has not yet been approved or rejected.
        rejected,   // subscription request has been denied by an administrator
        cancelled,  // subscription has been cancelled by the developer or administrator.
        expired,    // subscription reached its expiration date and was deactivated.
    }
}
