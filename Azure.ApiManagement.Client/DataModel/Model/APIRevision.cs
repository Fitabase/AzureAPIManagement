using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Model
{
	public class APIRevision : EntityBase
	{
		protected override string UriIdFormat => "";

		public string RevisionId
		{
			get
			{
				string id = "";

				if (!string.IsNullOrWhiteSpace(ApiId))
				{
					string[] splits = ApiId.Split('/');
					id = splits[splits.Length - 1];
				}

				return id;
			}
		}

		[JsonProperty("apiId")]
		public string ApiId { get; set; }

		[JsonProperty("apiRevision")]
		public string ApiRevision { get; set; }
		
		[JsonProperty("createdDateTime")]
		public DateTime CreatedOn { get; set; }

		[JsonProperty("updatedDateTime")]
		public DateTime UpdatedOn { get; set; }

		[JsonProperty("isOnline")]
		public bool IsOnline { get; set; }

		[JsonProperty("isCurrent")]
		public bool IsCurrent { get; set; }

		[JsonProperty("privateUrl")]
		public string PrivateUrl { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
	}
}
