using Fitabase.Azure.ApiManagement.DataModel.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Filters
{
	/// <summary>
	/// This class preresents a filter on query APIM resources
	/// </summary>
    public class QueryFilterExpression
    {
		public FilterExpression Filter { get; set; }
		public int? Skip { get; set; }
		public int? Top { get; set; }
		

		
		public String GetFilterQuery()
		{
			List<QueryKeyValuePair> queryList = new List<QueryKeyValuePair>();
			if (Filter != null)
				queryList.Add(new QueryKeyValuePair(Constants.ApiManagement.Url.FilterQuery, Filter.GetStringExpression()));
			if (Skip.HasValue && Skip> 0)
				queryList.Add(new QueryKeyValuePair(Constants.ApiManagement.Url.SkipFilterQuery, Skip.ToString()));
			if (Top.HasValue && Top > 0)
				queryList.Add(new QueryKeyValuePair(Constants.ApiManagement.Url.TopFilterQuery, Top.ToString()));

			return GetFilterQuery(queryList);
		}


		/// <summary>
		/// Construct query filter string from a list of QueryKeyValuePair
		/// </summary>
		/// <param name="pairs"></param>
		/// <returns></returns>
        private string GetFilterQuery(List<QueryKeyValuePair> pairs)
        {
            if (pairs == null || pairs.Count == 0)
                return "";										// return empty filter string

            StringBuilder builder = new StringBuilder();
			int i = 0;
			while (i < (pairs.Count - 1))                     // If there is more than 1 pair.
			{
				builder.Append(pairs.ElementAt(i).GetStringExpression()).Append("&");
				i++;
			}
			
            builder.Append(pairs.ElementAt(i).GetStringExpression());		// Append last query pair. 

            return builder.ToString();
        }

    }


}
