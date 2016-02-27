using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public enum ProductState
    {
        /// <summary>
        /// Non-published products are visible only to administrators.
        /// </summary>
        notPublished,

        /// <summary>
        /// Published products are discoverable by developers on the developer portal.
        /// </summary>
        published
    }

    public enum UserState
    {
        active,
        blocked
    }

    public enum SubscriptionState
    {
        /// <summary>
        /// subscription is active.
        /// </summary>
        active,

        /// <summary>
        ///  subscription is blocked, and the subscriber cannot call any APIs of the product.
        /// </summary>
        suspended,

        /// <summary>
        ///  subscription request has been made by the developer, but has not yet been approved or rejected.
        /// </summary>
        submitted,

        /// <summary>
        /// subscription request has been denied by an administrator.
        /// </summary>
        rejected,

        /// <summary>
        ///  subscription has been cancelled by the developer or administrator.
        /// </summary>
        cancelled,

        /// <summary>
        ///  subscription reached its expiration date and was deactivated.
        /// </summary>
        expired,
    }
}
