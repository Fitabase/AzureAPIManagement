using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections;
using Fitabase.Azure.ApiManagement.Model.Exceptions;

namespace Fitabase.Azure.ApiManagement.Model
{

    public class UpdateProduct : UpdateEntityBase
    {
        public UpdateProduct(string id) : base(id) {}

        public override Hashtable GetUpdateProperties()
        {
            Hashtable parameters = new Hashtable();

            if (!String.IsNullOrWhiteSpace(Name))
                parameters.Add("name", Name);
            if (!String.IsNullOrWhiteSpace(Description))
                parameters.Add("description", Description);
            if (!String.IsNullOrWhiteSpace(Terms))
                parameters.Add("terms", Terms);
            if (SubscriptionRequired != null)
                parameters.Add("subscriptionRequired", SubscriptionRequired);
            if (ApprovalRequired != null)
                parameters.Add("approvalRequired", ApprovalRequired);
            if (SubscriptionsLimit != null)
                parameters.Add("subscriptionLimit", SubscriptionsLimit);
            if (State != null)
                parameters.Add("state", State);

            return parameters;
        }


        
        public string Name { get; set; }
        public string Description { get; set; }
        public string Terms { get; set; }                       
        public bool? SubscriptionRequired { get; set; }         
        public bool? ApprovalRequired { get; set; }            
        public int? SubscriptionsLimit { get; set; }           
        public ProductState? State { get; set; }                

       
    }

    public class Product : EntityBase
    {
        protected override string UriIdFormat { get { return "/products/"; } }
        

        public static Product Create(string name, string description, string terms = null,
                        ProductState state = ProductState.notPublished,
                        bool subscriptionRequired = true,
                        bool apporvalRequired = false,
                        int? subscriptionsLimit = null)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("Product's name is required");

            Product product = new Product();
            product.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.PRODUCT);
            product.Name = name;
            product.Description = description;
            product.Terms = terms;
            product.State = state;
            product.SubscriptionRequired = subscriptionRequired;
            product.ApprovalRequired = apporvalRequired;
            product.SubscriptionsLimit = subscriptionsLimit;
            return product;

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
