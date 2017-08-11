using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Model;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class ParameterConverter : JsonCreationConverter<IParameter>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create an instance of IParameter base on field value
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jObject"></param>
        /// <returns>IParameter instance</returns>
        protected override IParameter Create(Type objectType, JObject jObject)
        {
            if (FieldExists("in", jObject) && MatchingField("in", jObject, "body"))
                return new BodyParameter();
            return new NonBodyParameter();
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }

        private bool MatchingField(string fieldName, JObject jObject, string matching)
        {
            return jObject[fieldName].Value<string>() == "body";
        }
    }
}
