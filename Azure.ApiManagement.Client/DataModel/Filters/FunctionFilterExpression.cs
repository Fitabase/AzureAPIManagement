using Fitabase.Azure.ApiManagement.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Filters
{
	public class FunctionFilterExpression : FilterExpression
	{
		public QueryKeyValuePair Pair { get; private set; }
		public FunctionOption Option { get; private set; }

		public FunctionFilterExpression(FunctionOption option, QueryKeyValuePair pair)
		{
			this.Option = option;
			this.Pair = pair;
		}

		public override string GetStringExpression()
		{
			return string.Format("{0}({1}, '{2}')", Option.ToDescriptionString(), Pair.Key, Pair.Value);
		}
	}
}
