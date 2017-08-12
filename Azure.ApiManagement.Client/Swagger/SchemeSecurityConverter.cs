using Swashbuckle.Swagger.Model;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class SchemeSecurityConverter : JsonCreationConverter<SecurityScheme>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected override SecurityScheme Create(Type objectType, JObject jObject)
        {
            if (jObject["type"] == null)
                return null;

            var value = jObject["type"].Value<string>();
            if (value == "basic")
                return new BasicAuthScheme();       // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.Swagger/Model/BasicAuthScheme.cs
            if (value == "apiKey")
                return new ApiKeyScheme();          // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/c431c6161b1777b194752b68664dd99d235264e7/src/Swashbuckle.AspNetCore.Swagger/Model/ApiKeyScheme.cs
            if (value == "oauth2")
                return new OAuth2Scheme();          // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/c431c6161b1777b194752b68664dd99d235264e7/src/Swashbuckle.AspNetCore.Swagger/Model/OAuth2Scheme.cs

            return null;
        }
    }
}
