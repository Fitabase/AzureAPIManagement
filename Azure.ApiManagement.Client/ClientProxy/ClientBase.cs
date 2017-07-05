using Fitabase.Azure.ApiManagement.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Fitabase.Azure.ApiManagement
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

        protected HttpRequestMessage BuildRequest(string url, string method, NameValueCollection query = null)
        {
            // combine uri with query
            var requestUri = this.BuildRequestUri(url, query);
            var request = new HttpRequestMessage(new HttpMethod(method), requestUri);


            // set the access control header
            var token = Utility.CreateSharedAccessToken(this.ServiceIdentifier, this.AccessKey, DateTime.UtcNow.AddDays(1));
            
            request.Headers.Authorization = new AuthenticationHeaderValue(Constants.ApiManagement.AccessToken, token);



            return request;
        }

        protected void BuildRequestContent(HttpRequestMessage request, object data, string mimeType = null)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (data == null) return;

            // embed data for request
            switch (mimeType)
            {
                case Constants.MimeTypes.ApplicationXmlPolicy:
                    request.Content = new StringContent(data.ToString(), Encoding.UTF8, Constants.MimeTypes.ApplicationXmlPolicy);
                    return;

                case Constants.MimeTypes.ApplicationJson:
                    request.Content = new StringContent(Utility.SerializeToJson(data), Encoding.UTF8, Constants.MimeTypes.ApplicationJson);
                    return;

                default: goto case Constants.MimeTypes.ApplicationJson;
            }
        }

        protected async Task<bool> ExecuteRequestAsync(HttpRequestMessage request,
             HttpStatusCode succesCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            // async making request
            var response = await this.ExecuteRequestInternalAsync(request, cancellationToken);

            // async reading response stream
            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == succesCode)
                return true;

            // faulted state
            var resultMediaType = response.Content?.Headers?.ContentType?.MediaType;
            var errorEx = this.ParseResponse<HttpResponseException>(resultMediaType, result);
            errorEx.StatusCode = response.StatusCode;
            throw errorEx;

        }

        protected async Task<TEntity> ExecuteRequestAsync<TEntity>(HttpRequestMessage request,
            HttpStatusCode succesCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            // async making request
            var response = await this.ExecuteRequestInternalAsync(request, cancellationToken);

            
            // async reading response stream
            var result = await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine("Result: " + result);

            var resultMediaType = response.Content?.Headers?.ContentType?.MediaType;

            if (response.StatusCode == succesCode)
                return this.ParseResponse<TEntity>(resultMediaType, result);

            // faulted state
            var errorEx = this.ParseResponse<HttpResponseException>(resultMediaType, result);
            errorEx.StatusCode = response.StatusCode;
            throw errorEx;
        }


        protected void EntityStateUpdate(HttpRequestMessage request, string value)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            request.Headers.Add(Constants.ApiManagement.Headers.ETagMatch, value);
        }


        #region Private Helpers

        private async Task<HttpResponseMessage> ExecuteRequestInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
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
                return response;
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

        private Uri BuildRequestUri(string uri, NameValueCollection query)
        {
            // decodes url-encoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(String.Empty);

            // integrate existing query 
            if (query != null) httpValueCollection.Add(query);

            // API Management REST Service requires version query parameter
            httpValueCollection.Add(Constants.ApiManagement.Url.VersionQuery, Constants.ApiManagement.Versions.Feb2014);
            
            // Url encodes the whole HttpValueCollection
            uri = String.Format("{0}?{1}", uri, httpValueCollection.ToString());

            Uri damn =  new Uri(uri, UriKind.Relative);
            
            return damn;
        }

        private TResponse ParseResponse<TResponse>(string resultMediaType, string result)
        {
            switch (resultMediaType)
            {
                case Constants.MimeTypes.ApplicationJson:
                    return Utility.DeserializeToJson<TResponse>(result);

                case Constants.MimeTypes.ApplicationXmlPolicy:
                    return (TResponse)(object)result;

                default:
                    throw new FormatException(String.Format("Response content format {0} is not supported.", resultMediaType));
            }
        }

        #endregion
    }
}
