using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class Product
    {
        /// <summary>
        /// Resource identifier. Uniquely identifies the product within the current API Management service instance. 
        /// The value is a valid relative URL in the format of products/{pid} where {pid} is a product identifier. This property is read-only.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the product. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the product. Must not be empty. May include HTML formatting tags. Maximum length is 1000 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Product terms of use. Developers trying to subscribe to the product will be presented and required to accept these terms before they can complete the subscription process.
        /// </summary>
        public string Terms { get; set; }

        /// <summary>
        /// Specifies whether a product subscription is required for accessing APIs included in this product. 
        /// If true, the product is referred to as protected and a valid subscription key is required for a request to an API included in the product to succeed. 
        /// If false, the product is referred to as open and requests to an API included in the product can be made without a subscription key. 
        /// If this property is omitted when creating a new product the default is true.
        /// </summary>
        public bool SubscriptionRequired { get; set; }

        /// <summary>
        /// Specifies whether subscription approval is required. 
        /// If false, new subscriptions will be approved automatically enabling developers to call the product’s APIs immediately after subscribing. 
        /// If true, administrators must manually approve the subscription before the developer can any of the product’s APIs.
        /// Can be present only if the subscriptionRequired property is present with a value of true.
        /// </summary>
        public bool ApprovalRequired { get; set; }

        /// <summary>
        /// Specifies the number of subscriptions a user can have to this product at the same time. 
        /// Set to null or omit to allow unlimited per user subscriptions.
        /// Can be present only if the subscriptionRequired property is present with a value of true.
        /// </summary>
        public byte SubscriptionsLimit { get; set; }

        /// <summary>
        /// Specifies whether the product is published or not. Published products are discoverable by developers on the developer portal. Non-published products are visible only to administrators.
        //  The allowable values for product state are published and notPublished.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// An array of Group entities that have visibility to the product.This property is optional and is only included in responses when the request has an expandGroups query parameter with a value of true.
        /// </summary>
        // Public IEnumerable<Groups> Groups {get; set;}
    }
}
