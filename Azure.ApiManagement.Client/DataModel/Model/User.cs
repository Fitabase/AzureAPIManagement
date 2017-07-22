using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections;
using Fitabase.Azure.ApiManagement.Model.Exceptions;

namespace Fitabase.Azure.ApiManagement.Model
{
    public enum UserState
    {
        active,
        blocked
    }

    public class UpdateUser : UpdateEntityBase
    {
        public UpdateUser(string id) : base(id)
        {
        }

        public override Hashtable GetUpdateProperties()
        {
            Hashtable parameters = new Hashtable();
            if (!String.IsNullOrWhiteSpace(FirstName))
                parameters.Add("firstName", FirstName);
            if (!String.IsNullOrWhiteSpace(LastName))
                parameters.Add("lastName", LastName);
            if (!String.IsNullOrWhiteSpace(Email))
                parameters.Add("email", Email);
            if (!String.IsNullOrWhiteSpace(Password))
                parameters.Add("password", Password);
            return parameters;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class UserIdentity
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }



    public class User : EntityBase
    {
        protected override string UriIdFormat { get { return "/users/"; } }
        
        public static User Create(string firstName, string lastName,
                    string email, string password,
                    UserState state = UserState.active, string note = "")
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
            user.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.USER);
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.Password = password;
            user.State = state;
            user.Note = note;
            return user;

        }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("registrationDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RegistrationDate { get; set; }

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserState State { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
        
        [JsonProperty("identities")]
        public UserIdentity[] Identities { get; set; }

        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }
    }

}
