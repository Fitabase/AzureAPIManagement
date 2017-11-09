using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Fitabase.Azure.ApiManagement.Model
{
    public enum GroupType
    {
        custom, system, external
    }
    public class Group : EntityBase
    {
        public static Group Create(string name, string description = "", 
                                    GroupType type = GroupType.custom, string externalId = null)
        {
            if (String.IsNullOrEmpty(name))
                throw new InvalidEntityException("group's name is required");
            if (type == GroupType.system)
                throw new InvalidEntityException("group's type can't be set to system");

            Group group = new Group();
            group.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.GROUP);
            group.Name = name;
            group.Description = description;
            group.Type = type;
            group.ExternalId = externalId;
            return group;
        }

        protected override string UriIdFormat {  get { return "/groups/"; } }

        /// <summary>
        /// Name of the entity. Must not be empty. Maximum length is 100 characters.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the entity. Must not be empty. May include HTML formatting tags. Maximum length is 1000 characters.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Returns true if the group is one of the three system groups (Administrators, Developers, or Guests); otherwise false. 
        /// This property is read-only.
        /// </summary>
        [JsonProperty("builtIn")]
        public bool BuiltIn { get; set; }

        /// <summary>
        /// The type of group, which is one of the following values: system, custom, or external. This property is read-only.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("type")]
        public GroupType Type { get; set; }

        /// <summary>
        /// For external groups, this property contains the id of the group from the external identity provider,
        /// e.g. Azure Active Directory; otherwise the value is null. 
        /// This property is read-only.
        /// </summary>
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

    }
}
