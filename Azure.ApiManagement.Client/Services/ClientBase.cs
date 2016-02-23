using SmallStepsLabs.Azure.ApiManagement.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            // combine uri with query
            var requestUri = this.BuildRequestUri(url, urlQuery);

            var request = new HttpRequestMessage(new HttpMethod(method), requestUri);

            // set the access control header
            var token = Utility.CreateSharedAccessToken(this.ServiceIdentifier, this.AccessKey, DateTime.UtcNow.AddDays(1));
            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.ApiManagement.AccessToken, token);

            // embed data for request
            if (data != null)
            {
                request.Content = new StringContent(Utility.SerializeToJson(data), Encoding.UTF8, Constants.MimeTypes.ApplicationJson);
            }

            return request;
        }

        protected async Task<TResponse> ExecuteRequestAsync<TResponse>(HttpRequestMessage request,
             HttpStatusCode succesCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpClient httpClient = null;

            try
            {
                // http rest client
                httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(String.Format(Constants.ApiManagement.Url.ServiceFormat, this.Host)),
                };

                // async making request
                var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);

                // async reading response stream
                var result = await response.Content.ReadAsStringAsync();

                var resultMediaType = response.Content?.Headers?.ContentType?.MediaType;

                if (response.StatusCode == succesCode)
                {
                    // generic operation result
                    if (typeof(TResponse) == typeof(bool))
                        return (TResponse)(object)true;

                    return this.ParseResponse<TResponse>(resultMediaType, result);
                }

                // faulted state
                var errorEx = this.ParseResponse<HttpResponseException>(resultMediaType, result);
                errorEx.StatusCode = response.StatusCode;
                throw errorEx;
            }
            finally
            {
                // Cleanup
                if (request != null)
                {
                    request.Dispose();
                    request = null;
                }

                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }


        protected void EntityStateUpdate(HttpRequestMessage request, string value)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            request.Headers.Add(Constants.ApiManagement.Headers.ETagMatch, value);
        }


        #region Private Helpers

        private Uri BuildRequestUri(string uri, string urlQuery)
        {
            if (urlQuery == null) urlQuery = String.Empty;

            // decodes url-encoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(urlQuery);

            // API Management REST Service requires version query parameter
            httpValueCollection.Add(Constants.ApiManagement.Url.VersionQuery, Constants.ApiManagement.Versions.Feb2014);

            // Url encodes the whole HttpValueCollection
            uri = String.Format("{0}?{1}", uri, httpValueCollection.ToString());

            return new Uri(uri, UriKind.Relative);
        }

        private TResponse ParseResponse<TResponse>(string resultMediaType, string result)
        {
            switch (resultMediaType)
            {
                case Constants.MimeTypes.ApplicationJson:
                    return Utility.DeserializeToJson<TResponse>(result);

                default:
                    throw new FormatException(String.Format("Response content format {0} is not supported.", resultMediaType));
            }
        }

        #endregion
    }
}
