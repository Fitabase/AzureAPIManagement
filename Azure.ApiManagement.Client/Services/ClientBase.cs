using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SmallStepsLabs.Azure.ApiManagement
{
    public abstract class ClientBase
    {
        #region Private Settings

        private string Host { get; set; }

        private string ServiceIdentifier { get; set; }

        private string AccessKey { get; set; }

        #endregion

        public ClientBase(string host, string serviceIdentifier, string accessKey)
        {
            if (String.IsNullOrEmpty(host))
                throw new ArgumentException("Host value must be provided to initialize client proxy.", "host");
            if (String.IsNullOrEmpty(serviceIdentifier))
                throw new ArgumentException("Service identified must be provided to initialize client proxy.", "serviceIdentifier");
            if (String.IsNullOrEmpty(accessKey))
                throw new ArgumentException("Service access key must be provided to initialize client proxy.", "accessKey");

            this.Host = host;
            this.ServiceIdentifier = serviceIdentifier;
            this.AccessKey = accessKey;
        }

        protected HttpRequestMessage GetRequest(string url, string method, string urlQuery = null, object data = null)
        {
            var requestUri = this.BuildRequestUri(url, urlQuery);

            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Set the SharedAccessSignature header
            var token = Utility.CreateSharedAccessToken(this.ServiceIdentifier, this.AccessKey, DateTime.UtcNow.AddDays(1));
            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.ApiManagement.AccessToken, token);

            if (data != null)
            {
                request.Content = new StringContent(Utility.SerializeToJson(data), Encoding.UTF8, Constants.MimeTypes.ApplicationJson);
            }

            return request;
        }

        protected async Task<TResponse> ExecuteRequestAsync<TResponse>(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpClient httpClient = null;

            try
            {
                httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(String.Format(Constants.ApiManagement.Url.ServiceFormat, this.Host))
                };

                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

                var result = await response.Content.ReadAsStringAsync();

                return Utility.DeserializeToJson<TResponse>(result);
            }
            finally
            {
                if (httpClient != null)
                    httpClient.Dispose();
            }
        }


        #region Private Helpers

        private Uri BuildRequestUri(string uri, string urlQuery)
        {
            if (urlQuery == null) urlQuery = String.Empty;

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(urlQuery);

            // API Management REST Service requires version query parameter
            httpValueCollection.Add(Constants.ApiManagement.Url.VersionQuery, Constants.ApiManagement.Versions.Feb2014);

            // Url encodes the whole HttpValueCollection
            uri = String.Format("{0}?{1}", uri, httpValueCollection.ToString());

            return new Uri(uri, UriKind.Relative);

        }

        #endregion
    }
}
