using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections;
using Fitabase.Azure.ApiManagement.Model.Exceptions;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class User : EntityBase
    {
        protected override string UriIdFormat { get { return "/users/"; } }
        
        public static User Create(string firstName, string lastName,
                    string email, string password,
                    UserState state = UserState.active, 
                    string note = "",
                    string userId = null)
        {

            if (String.IsNullOrEmpty(firstName) && firstName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'FirstName' is required and must not exceed 100 characters.");
            if (String.IsNullOrEmpty(lastName) && lastName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'LastName' is required and must not exceed 100 characters.");
            if (String.IsNullOrEmpty(email) && email.Length > 254)
                throw new InvalidEntityException("User configuration is not valid. 'Email' is required and must not exceed 254 characters.");
            if (String.IsNullOrWhiteSpace(password))
                throw new InvalidEntityException("Invalid password: " + password);

            User user = new User();
            user.Id = userId ?? EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.USER);
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.Password = password;
            user.State = state;
            user.Note = note;
            return user;

        }

        [JsonProperty("firstName", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("registrationDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RegistrationDate { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserState State { get; set; }

        [JsonProperty("note", NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }
        
        [JsonProperty("identities", NullValueHandling = NullValueHandling.Ignore)]
        public UserIdentity[] Identities { get; set; }

        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }
    }

}
