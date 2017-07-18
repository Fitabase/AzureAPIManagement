using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Model
{
    public class Property : EntityBase
    {
        public static Property Create()
        { 
            Property property = new Property();
            property.Id = EntityIdGenerator.GenerateIdSignature(Constants.IdPrefixTemplate.PROPERTY);

            return property;
        }

        protected override string UriIdFormat { get { return "/properties/"; } }
    }
}
