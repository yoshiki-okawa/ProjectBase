using System;

namespace PB.Frameworks.Types.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DbTableAttribute : Attribute
	{
		public string TableName { get; set; }
	}
}
