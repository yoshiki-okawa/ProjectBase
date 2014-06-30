using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PB.Services.DataContracts
{
	[DataContract]
	public class DCGetCurrentListResult : DCResult
	{
		[DataMember(Order = 2)]
		public List<DCBLLBase> BLLBases { get; set; }
	}
}
