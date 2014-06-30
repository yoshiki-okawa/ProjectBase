using PB.Frameworks.Resources;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCRegisterArgs
	{
		[DataMember(Order = 1, IsRequired = true)]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression("^[0-9a-zA-Z_\\.\\-]+$", ErrorMessageResourceName = "DEUSER_INVALID_NAME", ErrorMessageResourceType = typeof(TypesResource))]
		public string UserName { get; set; }

		[DataMember(Order = 2, IsRequired = true)]
		[StringLength(254, MinimumLength = 3)]
		[RegularExpression("^\\S+@\\S+\\.\\S+$", ErrorMessageResourceName = "DEUSER_INVALID_EMAIL", ErrorMessageResourceType = typeof(TypesResource))]
		public string Email { get; set; }

		[DataMember(Order = 3, IsRequired = true)]
		[StringLength(int.MaxValue, MinimumLength = 6)]
		public string Password { get; set; }
	}
}
