using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Filters
{
	public class FilterQueryConstants
	{

		/// <summary>
		/// https://docs.microsoft.com/en-us/rest/api/apimanagement/Api/ListByService
		/// </summary>
		public class Api
		{
			public readonly static string Id = "id";
			public readonly static string Name = "name";
			public readonly static string Description = "description";
			public readonly static string ServiceUrl = "serviceUrl";
			public readonly static string Path = "path";
			
		}

		public class Operation
		{
			public readonly static string Name = "name";
			public readonly static string Description = "description";
			public readonly static string Method = "method";
			public readonly static string Urltemplate = "urlTemplate";
			
		}
	}
}
