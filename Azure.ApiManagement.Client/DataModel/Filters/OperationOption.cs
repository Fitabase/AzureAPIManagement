using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.DataModel.Filters
{
	public enum OperationOption
	{
		[Description("ge")]
		GE
		,
		[Description("le")]
		LE
		,
		[Description("eq")]
		EQ
		,
		[Description("ne")]
		NE
		,
		[Description("gt")]
		GT
		,
		[Description("lt")]
		LT

	}

	public static class OperationOptionExtension
	{
		public static string ToDescriptionString(this OperationOption options)
		{
			DescriptionAttribute[] attributes = (DescriptionAttribute[])options.GetType().GetField(options.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : string.Empty;

		}
	}


}
