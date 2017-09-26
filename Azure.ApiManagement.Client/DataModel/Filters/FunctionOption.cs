using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Filters
{
	/// <summary>
	/// This class represents function options that can be performed in filter
	/// </summary>
	public enum FunctionOption
	{
		[Description("substringof")]
		SUBSTRING_OF,

		[Description("contains")]
		CONTAINS,

		[Description("startswith")]
		START_WITH,

		[Description("endswith")]
		END_WITH
		
	}

	public static class FunctionOptionExtension
	{
		public static string ToDescriptionString(this FunctionOption options)
		{
			DescriptionAttribute[] attributes = (DescriptionAttribute[])options.GetType().GetField(options.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : string.Empty;

		}
	}
}
