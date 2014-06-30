using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCResetPasswordArgs
	{
		[DataMember(Order = 1, IsRequired = true)]
		[StringLength(int.MaxValue, MinimumLength = 6)]
		public string Password { get; set; }

		[DataMember(Order = 2, IsRequired = true)]
		public int Id { get; set; }

		[DataMember(Order = 3, IsRequired = true)]
		public string VerificationCode { get; set; }
	}
}
