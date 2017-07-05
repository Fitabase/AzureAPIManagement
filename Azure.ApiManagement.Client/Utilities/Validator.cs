using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement
{
    internal class Validator
    {
        internal static void ValidateProduct(Product product)
        {
            if (String.IsNullOrEmpty(product.Name) && product.Name.Length > 100)
                throw new InvalidEntityException("Product configuration is not valid. 'Name' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(product.Description) && product.Description.Length > 1000)
                throw new InvalidEntityException("Product configuration is not valid. 'Description' is required and must not exceed 1000 characters.");

            // Cannot provide values for approvalRequired and subscriptionsLimit when subscriptionRequired is set to false in the request payload
            if (!product.SubscriptionRequired)
            {
                if (product.ApprovalRequired.HasValue || product.SubscriptionsLimit.HasValue)
                    throw new InvalidEntityException("Product configuration is not valid. Subscription requirement cannot be false while either Subscription limit or Approval required is set");
            }

        }

        internal static void ValidateUser(User user)
        {
            if (String.IsNullOrEmpty(user.FirstName) && user.FirstName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'FirstName' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(user.LastName) && user.LastName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'LastName' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(user.Email) && user.Email.Length > 254)
                throw new InvalidEntityException("User configuration is not valid. 'Email' is required and must not exceed 254 characters.");
        }
    }
}
