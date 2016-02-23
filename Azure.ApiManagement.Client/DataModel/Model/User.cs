using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class User
    {
        /// <summary>
        /// First name. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Email address. Must not be empty and must be unique within the service instance. Maximum length 254 characters.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Password for user.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Specifies whether the user is active or not. Blocked users cannot authenticate on the developer portal or call any APIs of the products to which they are subscribed.
        /// </summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserState State { get; set; }

        /// <summary>
        /// Optional note about a user set by the administrator.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
