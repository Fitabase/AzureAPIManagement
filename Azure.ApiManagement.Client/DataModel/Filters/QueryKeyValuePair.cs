using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Filters
{

	public class QueryKeyValuePair : FilterExpression
	{
		public string Key { get; private set; }
		public string Value { get; private set; }

		public QueryKeyValuePair(string key, string value)
		{
			this.Key = key;
			this.Value = value;
		}



		public override string GetStringExpression()
		{
			return string.Format("{0}={1}", Key, Value);
		}
	}

}
