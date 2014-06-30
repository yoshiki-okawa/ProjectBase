using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCVerifyArgs
	{
		[DataMember(Order = 1, IsRequired = true)]
		public int Id { get; set; }

		[DataMember(Order = 2, IsRequired = true)]
		public string VerificationCode { get; set; }
	}
}
