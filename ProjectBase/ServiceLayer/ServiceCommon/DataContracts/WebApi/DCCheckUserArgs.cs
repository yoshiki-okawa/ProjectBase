using PB.Frameworks.Resources;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCCheckUserArgs
	{
		[DataMember(Order = 1, IsRequired = false)]
		[StringLength(50, MinimumLength = 3)]
		[RegularExpression("^[0-9a-zA-Z_\\.\\-]+$", ErrorMessageResourceName = "DEUSER_INVALID_NAME", ErrorMessageResourceType = typeof(TypesResource))]
		public string UserName { get; set; }

		[DataMember(Order = 2, IsRequired = false)]
		[EmailAddress(ErrorMessageResourceName = "DEUSER_INVALID_EMAIL", ErrorMessageResourceType = typeof(TypesResource))]
		public string Email { get; set; }
	}
}
