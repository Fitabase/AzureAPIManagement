using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.Model
{
    public class User : EntityBase
    {
        protected override string UriIdFormat { get { return "/users/"; } }

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
        /// User registration date, in ISO 8601 format
        /// </summary>
        [JsonProperty("registrationDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RegistrationDate { get; set; }

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

        /// <summary>
        /// An array of Group entities that have visibility to the user.
        /// This property is optional and is only included in responses when the request has an expandGroups query parameter with a value of true.
        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }
    }
}
