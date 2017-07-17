using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public enum UserState
    {
        active,
        blocked
    }



    public class User : EntityBase
    {
        protected override string UriIdFormat { get { return "/users/"; } }
        
        public static User Create(string firstName, string lastName,
                    string email, string password,
                    UserState state = UserState.active, string note = "")
        {
            try
            {

                if (String.IsNullOrWhiteSpace(firstName) || firstName.Length > 100)
                    throw new ArgumentException("Invalid firstname: " + firstName);
                if (String.IsNullOrWhiteSpace(lastName) || lastName.Length > 100)
                    throw new ArgumentException("Invalid lastname: " + lastName);
                if (String.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Invalid email: " + email);
                if (String.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Invalid password: " + password);

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
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

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

    public class UserIdentity
    {
        [JsonProperty("provider")]
        public string Provider { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
