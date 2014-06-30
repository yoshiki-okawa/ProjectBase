using PB.Frameworks.Resources;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCRequestPasswordResetArgs
	{
		[DataMember(Order = 1, IsRequired = true)]
		[StringLength(254, MinimumLength = 3)]
		[RegularExpression("^\\S+$", ErrorMessageResourceName = "EMAIL_OR_USERNAME_INCORRECT", ErrorMessageResourceType = typeof(BusinessLogicLayerResource))]
		public string EmailOrUserName { get; set; }
	}
}
