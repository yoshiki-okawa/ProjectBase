using System;

namespace PB.Frameworks.Common.Extensions
{
	public static class StringExtensions
	{
		public static string FormatStr(this string str, params object[] args)
		{
			return String.Format(str, args);
		}
	}
}
