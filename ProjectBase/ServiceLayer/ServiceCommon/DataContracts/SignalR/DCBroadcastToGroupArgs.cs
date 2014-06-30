using System.ComponentModel.DataAnnotations;

namespace PB.Services.DataContracts
{
	public class DCBroadcastToGroupArgs
	{
		[MaxLength(1000), Required]
		public string Group { get; set; }
		[MaxLength(1000), Required]
		public string Message { get; set; }
	}
}
