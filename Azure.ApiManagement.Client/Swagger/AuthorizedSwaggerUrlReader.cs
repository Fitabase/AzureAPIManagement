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
		public ApimOption ApimOption { get; private set; }
		
		public AuthorizedSwaggerUrlReader(string url, ApimOption header) : base(url)
        {
			ApimOption = header;
		}

		private HttpRequestMessage OnHandleRequest()
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, this.ResourcePath);
			foreach (KeyValuePair<HeaderOption, string> pair in ApimOption.Headers)
			{
				request.Headers.Add(pair.Key.ToDescriptionString(), pair.Value);
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
				//request = new HttpRequestMessage(HttpMethod.Get, this.ResourcePath);
				//request.Headers.Add("Ocp-Apim-Subscription-Key", "362453116f0948dea2461856d29b310f");
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
