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
        private string Host { get; set; }


        private string GetToken()
        {
            // Programmatically generate the access token used to call the API Management REST API.
            // See "To programmatically create an access token" for instructions on how to get
            // the values for id and key:
            // https://msdn.microsoft.com/library/azure/5b13010a-d202-4af5-aabf-7ebc26800b3d#ProgrammaticallyCreateToken
            // id - the value from the identifier text box in the credentials section of the
            //      API Management REST API tab of the Security section.
            string id = "<your identifier value here>";

            // key - either the primary or secondary key from that same tab.
            string key = "<either the primary or secondary key here>";
            // expiry - the expiration date and time of the generated access token. In this example
            //          the expiry is one day from the time the sample is run.

            DateTime expiry = DateTime.UtcNow.AddDays(1);

            // To programmatically create the access token so that we can authenticate and call the REST APIs,
            // call this method. If you pasted in the access token from the publisher portal then you can
            // comment out this line.
            return Utility.CreateSharedAccessToken(id, key, expiry);
        }

        protected HttpRequestMessage GetRequest(string url, string method, string urlQuery = null, object data = null)
        {
            var requestUri = this.BuildRequestUri(url, urlQuery);

            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Set the SharedAccessSignature header
            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.ApiManagement.AccessToken, this.GetToken());

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

                response.EnsureSuccessStatusCode();
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
            var baseUri = new UriBuilder(uri);

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(urlQuery);

            // API Management REST Service requires version query parameter
            httpValueCollection.Add(Constants.ApiManagement.Url.VersionQuery, Constants.ApiManagement.Versions.Feb2014);

            // Url encodes the whole HttpValueCollection
            baseUri.Query = httpValueCollection.ToString();

            return baseUri.Uri;

        }

        #endregion
    }
}
