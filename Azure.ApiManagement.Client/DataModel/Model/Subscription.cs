using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using Fitabase.Azure.ApiManagement.Model.Exceptions;

namespace Fitabase.Azure.ApiManagement.Model
{

    public class SubscriptionDateSettings
    {
        public SubscriptionDateSettings(DateTime startDate, DateTime expirationDate)
        {
            this.StartDate = startDate;
            this.ExpirationDate = expirationDate;
        }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }    
    }
    
    

    public class Subscription : EntityBase
    {
        public static Subscription Create(string userId, string productId, string name,
                                           SubscriptionDateSettings dateSettings = null,
                                           SubscriptionState state = SubscriptionState.submitted,
                                           string primaryKey = null, string secondaryKey = null)
        {
            if (String.IsNullOrWhiteSpace(userId) 
                && userId.StartsWith(Constants.IdPrefixTemplate.USER))
                throw new InvalidEntityException("subscription's userId is required");
            if (String.IsNullOrWhiteSpace(productId)
                && userId.StartsWith(Constants.IdPrefixTemplate.PRODUCT))
                throw new InvalidEntityException("subscription's product is required");
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("subscription's name is required");

            Subscription subscription = new Subscription();
            subscription.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.SUBSCRIPTION);
            subscription.UserId = userId;
            subscription.ProductId = productId;
            subscription.Name = name;
            subscription.PrimaryKey = primaryKey;
            subscription.SecondaryKey = secondaryKey;
            subscription.State = state;
            subscription.StartDate = dateSettings.StartDate;
            subscription.ExpirationDate = dateSettings.ExpirationDate;
            
            return subscription;
        }

        protected override string UriIdFormat { get { return "/subscriptions/"; } }


        private string userId;
        [JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
        public string UserId                    // User (user id path) for whom subscription is being created in form /users/{uid}
        {
            get
            {
                if (String.IsNullOrWhiteSpace(userId))
                    return null;
                if (this.userId.Contains("/users/"))
                    return userId;
                return "/users/" + this.userId;
            }
            set
            {
                this.userId = value;
            }
        }

        private string productId;
        [JsonProperty("productId", NullValueHandling = NullValueHandling.Ignore)]
        public string ProductId                          // Product (product id path) for which subscription is being created in form /products/{productid} 
        {
            get
            {
                if (String.IsNullOrWhiteSpace(productId))
                    return null;
                if (this.productId.Contains("/products/"))
                    return this.productId;
                return "/products/" + this.productId;
            }
            set
            {
                this.productId = value;
            }      
        }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }            // Subscription name.
        
        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionState? State { get; set; }    // The state of the subscription. 
        
        [JsonProperty("createdDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDate { get; set; }       // Subscription creation date. 

        [JsonProperty("startDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? StartDate { get; set; }         // Subscription activation date.

        [JsonProperty("expirationDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ExpirationDate { get; set; }    // Subscription expiration date.

        [JsonProperty("endDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EndDate { get; set; }           // Date when subscription was cancelled or expired.

        [JsonProperty("notifiactionDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? NotificationDate { get; set; }  // Upcoming subscription expiration notification date.

        [JsonProperty("stateComment", NullValueHandling = NullValueHandling.Ignore)]
        public string StateComment { get; set; }        // Optional subscription comment added by an administrator.

        [JsonProperty("primaryKey", NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryKey { get; set; }          // Primary subscription key. If not specified during request key will be generated automatically.
        
        [JsonProperty("secondaryKey", NullValueHandling = NullValueHandling.Ignore)]
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
