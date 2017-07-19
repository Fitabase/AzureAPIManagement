using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class UrlContentReader : AbstractSwaggerReader
    {
        public UrlContentReader(string resourcePath) : base(resourcePath)
        {
        }
        

        public override string GetSwaggerJson()
        {
            if(!IsUrlFormat(this.ResourcePath))
            {
                throw new SwaggerResourceException("Invalid URL");
            }
            string json = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    json = client.DownloadString(ResourcePath);
                }
            } catch(Exception)
            {
            }

            return json;
        }


        private bool IsUrlFormat(string input)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(input, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
    }
}
