using Fitabase.Azure.ApiManagement.DataModel.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Filters
{
	public class OperationFilterExpression : FilterExpression
	{
		public OperationOption Option { get; set; }
		public QueryKeyValuePair Pair { get; set; }

		public OperationFilterExpression(OperationOption option, QueryKeyValuePair pair) {
			this.Option = option;
			this.Pair = pair;
		}

		public override string GetStringExpression()
		{
			return string.Format("{0} {1} '{2}'", Pair.Key, Option.ToDescriptionString(), Pair.Value);
		}
	}
}
