using Fitabase.Azure.ApiManagement.Model.Exceptions;
using System;
using System.Net;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    public class SwaggerUrlReader : AbstractSwaggerReader
    {
        public SwaggerUrlReader(string resourcePath) : base(resourcePath)
        {
        }
        

        public override string GetSwaggerJson()
        {
            if(!IsUrlFormat(this.ResourcePath))
            {
                throw new SwaggerResourceException("Invalid URL Format");
            }
            
            string json = null;
            try
            {
                WebClient client = new WebClient();
                json = client.DownloadString(this.ResourcePath);
            } catch(Exception ex)
            {
                throw new SwaggerResourceException(ex.Message);
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
