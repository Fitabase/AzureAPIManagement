using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
	public class AuthorizedSwaggerUrlReader : AbstractSwaggerReader
	{
		public List<KeyValuePair<string, string>> HeaderOptions { get; private set; }
		
		public AuthorizedSwaggerUrlReader(string url, List<KeyValuePair<string, string>> headerOptions) : base(url)
        {
			HeaderOptions = headerOptions;
		}

		private HttpRequestMessage OnHandleRequest()
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, this.ResourcePath);
			foreach (var pair in HeaderOptions)
			{
				request.Headers.Add(pair.Key, pair.Value);
			}

			return request;
		}




		protected override string GetSwaggerJson()
		{
			HttpClient httpClient = null;
			HttpRequestMessage request = null;
			HttpResponseMessage response = null;
			try
			{
				httpClient = new HttpClient();
				request = OnHandleRequest();

				response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
				string result = response.Content.ReadAsStringAsync().Result;
				return result;
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
	}
}
