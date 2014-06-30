using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCBLLBase
	{
		[DataMember(Order = 1)]
		public string Name { get; set; }

		[DataMember(Order = 2)]
		public double ProgressPercent { get; set; }

		[DataMember(Order = 3)]
		public string ProgressMessage { get; set; }
	}
}
