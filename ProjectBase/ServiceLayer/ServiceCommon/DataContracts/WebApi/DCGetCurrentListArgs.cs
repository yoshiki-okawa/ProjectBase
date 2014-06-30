using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCGetCurrentListArgs
	{
		[DataMember(Order = 1, IsRequired = true)]
		public string SessionKey { get; set; }
	}
}
