using System;

namespace PB.Frameworks.Types.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DbFieldAttribute : Attribute
	{
		public string FieldName { get; set; }
	}
}
