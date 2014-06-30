using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCResult
	{
		[DataMember(Order = 1)]
		public bool Success { get; set; }
	}
}
