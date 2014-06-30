using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCLoadUserDetailsResult : DCResult
	{
		[DataMember(Order = 2)]
		public string UserName { get; set; }

		[DataMember(Order = 3)]
		public string Email { get; set; }
	}
}
