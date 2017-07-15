using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Product : EntityBase
    {
        protected override string UriIdFormat { get { return "/products/"; } }
        

        public Product(string name, string description, string terms = null,
                        ProductState state = ProductState.notPublished,
                        bool subscriptionRequired = true,
                        bool apporvalRequired = false,
                        int? subscriptionsLimit = null)
        {
            this.Name = name;
            this.Description = description;
            this.Terms = terms;
            this.State = state;
            this.SubscriptionRequired = subscriptionRequired;
            this.ApprovalRequired = apporvalRequired;
            this.SubscriptionsLimit = subscriptionsLimit;
        }

        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }                       // Product terms of use.

        [JsonProperty("subscriptionRequired")]
        public bool SubscriptionRequired { get; set; }          // Whether a product subscription is required for accessing APIs included in this product

        [JsonProperty("approvalRequired")]
        public bool? ApprovalRequired { get; set; }             // whether subscription approval is required.

        [JsonProperty("subscriptionsLimit")]
        public int? SubscriptionsLimit { get; set; }            // Whether the number of subscriptions a user can have to this product at the same time.

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductState State { get; set; }                 // whether product is published or not.

        [JsonProperty("groups")]
        public List<Group> Groups {get; set;}

    }
    

   
}
