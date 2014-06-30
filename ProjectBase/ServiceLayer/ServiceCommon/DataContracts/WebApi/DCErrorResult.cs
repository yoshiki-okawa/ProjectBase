using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCErrorResult : DCResult
	{
		[DataMember(Order = 2)]
		public string ErrorMessage { get; set; }

		[DataMember(Order = 3)]
		public string ErrorCode { get; set; }
	}
}
