using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class User : EntityBase
    {
        protected override string UriIdFormat { get { return "/users/"; } }

        //public User() { }

        //public User(string firstName, string lastName,
        //            string email, string password,
        //            UserState state = UserState.active, string note = "") : this()
        //{
        //    if (String.IsNullOrWhiteSpace(firstName) || firstName.Length > 100)
        //        throw new ArgumentException("Invalid firstname: " + firstName);
        //    if (String.IsNullOrWhiteSpace(lastName) || lastName.Length > 100)
        //        throw new ArgumentException("Invalid lastname: " + lastName);
        //    if (String.IsNullOrWhiteSpace(email))
        //        throw new ArgumentException("Invalid email: " + email);
        //    if (String.IsNullOrWhiteSpace(password))
        //        throw new ArgumentException("Invalid password: " + password);

        //    this.FirstName = firstName;
        //    this.LastName = lastName;
        //    this.Email = email;
        //    this.Password = password;
        //    this.State = state;
        //    this.Note = note;

        //}
        
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("registrationDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RegistrationDate { get; set; }

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserState State { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("groups", NullValueHandling = NullValueHandling.Ignore)]
        public List<Group> Groups { get; set; }
    }
}
