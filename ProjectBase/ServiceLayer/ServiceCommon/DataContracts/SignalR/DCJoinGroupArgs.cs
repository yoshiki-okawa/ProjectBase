using System.ComponentModel.DataAnnotations;

namespace PB.Services.DataContracts
{
	public class DCJoinGroupArgs
	{
		[MaxLength(1000), Required]
		public string Group { get; set; }
	}
}
