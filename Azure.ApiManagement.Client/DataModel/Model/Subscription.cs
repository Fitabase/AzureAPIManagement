using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;

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
    

    public class UpdateSubscription
    {
        public UpdateSubscription(string subscriptionId)
        {
            this.Id = subscriptionId;
            
        }

        public Hashtable GetUpdateProperties()
        {
            if (String.IsNullOrWhiteSpace(Id))
                throw new ArgumentException("subscription id is required");

            Hashtable parameters = new Hashtable();
            if(!String.IsNullOrWhiteSpace(this.UserId))
            {
                parameters.Add("userId", "/users/" + this.UserId);
            }
            if (!String.IsNullOrWhiteSpace(this.ProductId))
            {
                parameters.Add("productId", "/products/" + this.ProductId);
            }
            if (this.ExpirationDate != null)
            {
                parameters.Add("expirationDate", this.ExpirationDate);
            }
            return parameters;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class Subscription : EntityBase
    {
        public static Subscription Create(string userId, string productId, string name,
                                           SubscriptionDateSettings dateSettings = null,
                                           SubscriptionState state = SubscriptionState.submitted,
                                           string primaryKey = null, string secondaryKey = null)
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
                subscription.UserId = "/users/" + userId;
                subscription.ProductId = "/products/" + productId;
                subscription.Name = name;
                subscription.PrimaryKey = primaryKey;
                subscription.SecondaryKey = secondaryKey;
                subscription.State = state;
                subscription.StartDate = dateSettings.StartDate;
                subscription.ExpirationDate = dateSettings.ExpirationDate;

                PrintMessage.Debug("subscription", subscription.StartDate.ToString());
                PrintMessage.Debug("subscription", subscription.ExpirationDate.ToString());

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
