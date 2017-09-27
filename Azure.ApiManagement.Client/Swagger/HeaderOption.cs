using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
	public enum HeaderOption
	{
		[Description("Ocp-Apim-Subscription-Key")]
		Ocp_Apim_Subscription_Key
	}

	public static class HeaderOptionExtension
	{
		public static string ToDescriptionString(this HeaderOption option)
		{
			DescriptionAttribute[] attributes = (DescriptionAttribute[])option.GetType().GetField(option.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attributes.Length > 0 ? attributes[0].Description : string.Empty;

		}
	}
}
