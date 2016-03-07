using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fitabase.Azure.ApiManagement
{
    internal static class Utility
    {
        // To programmatically create an access token
        // https://msdn.microsoft.com/library/azure/5b13010a-d202-4af5-aabf-7ebc26800b3d#ProgrammaticallyCreateToken
        // Required inputs:
        // id - the value from the identifier text box in the credentials section of the
        //      API Management REST API tab of the Security section.
        // key - either the primary or secondary key from that same tab.
        // expiry - the expiration date and time for the generated access token.
        internal static string CreateSharedAccessToken(string id, string key, DateTime expiry)
        {
            using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                string dataToSign = id + "\n" + expiry.ToString("O", CultureInfo.InvariantCulture);
                string x = string.Format("{0}\n{1}", id, expiry.ToString("O", CultureInfo.InvariantCulture));
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                var signature = Convert.ToBase64String(hash);
                string encodedToken = string.Format("uid={0}&ex={1:o}&sn={2}", id, expiry, signature);
                return encodedToken;
            }
        }


        internal static string SerializeToJson<TAnything>(TAnything value)
        {
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver()
                {
                    IgnoreSerializableInterface = true
                }
            };
            var formatting = Formatting.None;
            var writer = new StringWriter();

            var serializer = JsonSerializer.Create(settings);
            var jsonWriter = new JsonTextWriter(writer) { Formatting = formatting };

            serializer.Serialize(jsonWriter, value);
            return writer.GetStringBuilder().ToString();
        }


        internal static TAnything DeserializeToJson<TAnything>(String value)
        {
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new DefaultContractResolver()
                {
                    IgnoreSerializableInterface = true
                }
            };
            var reader = new StringReader(value);

            var serializer = JsonSerializer.Create(settings);
            var jsonReader = new JsonTextReader(reader);

            return (TAnything)serializer.Deserialize(jsonReader, typeof(TAnything));
        }

        internal static void ValidateProduct(Product product)
        {
            if (String.IsNullOrEmpty(product.Name) && product.Name.Length > 100)
                throw new InvalidEntityException("Product configuration is not valid. 'Name' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(product.Description) && product.Description.Length > 1000)
                throw new InvalidEntityException("Product configuration is not valid. 'Description' is required and must not exceed 1000 characters.");

            // Cannot provide values for approvalRequired and subscriptionsLimit when subscriptionRequired is set to false in the request payload
            if (!product.SubscriptionRequired)
            {
                if (product.ApprovalRequired.HasValue || product.SubscriptionsLimit.HasValue)
                    throw new InvalidEntityException("Product configuration is not valid. Subscription requirement cannot be false while either Subscription limit or Approval required is set");
            }

        }

        internal static void ValidateUser(User user)
        {
            if (String.IsNullOrEmpty(user.FirstName) && user.FirstName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'FirstName' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(user.LastName) && user.LastName.Length > 100)
                throw new InvalidEntityException("User configuration is not valid. 'LastName' is required and must not exceed 100 characters.");

            if (String.IsNullOrEmpty(user.Email) && user.Email.Length > 254)
                throw new InvalidEntityException("User configuration is not valid. 'Email' is required and must not exceed 254 characters.");
        }
    }
}
