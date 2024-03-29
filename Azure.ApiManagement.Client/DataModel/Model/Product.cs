﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections;
using Fitabase.Azure.ApiManagement.Model.Exceptions;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Product : EntityBase
    {
        protected override string UriIdFormat { get { return "/products/"; } }
        

        public static Product Create(string name, string description,
                        ProductState state = ProductState.notPublished,
                        bool subscriptionRequired = true,
                        bool? approvalRequired = true,
                        int? subscriptionsLimit = 0,
                        string terms = null,
                        string productId = null)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidEntityException("Product's name is required");

            if (String.IsNullOrEmpty(description) && description.Length > 1000)
                throw new InvalidEntityException("Product configuration is not valid. 'Description' is required and must not exceed 1000 characters.");

            if (!subscriptionRequired)
            {
                if (approvalRequired.HasValue || subscriptionsLimit.HasValue)
                    throw new InvalidEntityException("Product configuration is not valid. Subscription requirement cannot be false while either Subscription limit or Approval required is set");
            }

            Product product = new Product();
            product.Id = productId ?? EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.PRODUCT);
            product.DisplayName = name;
            product.Description = description;
            product.Terms = terms;
            product.State = state;
            product.SubscriptionRequired = subscriptionRequired;
            product.ApprovalRequired = approvalRequired;
            product.SubscriptionsLimit = subscriptionsLimit;
            return product;

        }

        
        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("terms", NullValueHandling = NullValueHandling.Ignore)]
        public string Terms { get; set; }                       // Product terms of use.

        [JsonProperty("subscriptionRequired", NullValueHandling = NullValueHandling.Ignore)]
        public bool SubscriptionRequired { get; set; }          // Whether a product subscription is required for accessing APIs included in this product

        [JsonProperty("approvalRequired", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ApprovalRequired { get; set; }             // whether subscription approval is required.

        [JsonProperty("subscriptionsLimit", NullValueHandling = NullValueHandling.Ignore)]
        public int? SubscriptionsLimit { get; set; }            // Whether the number of subscriptions a user can have to this product at the same time.

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductState State { get; set; }                 // whether product is published or not.

        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups {get; set;}

    }
    

   
}
