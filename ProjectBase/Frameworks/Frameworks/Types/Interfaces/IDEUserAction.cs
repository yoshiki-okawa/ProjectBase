
using PB.Frameworks.Common.General;
namespace PB.Frameworks.Types.Interfaces
{
	public interface IDEUserAction : IDbEntity
	{
		int UserId { get; set; }
		GlobalEnums.UserActionType Type { get; set; }
		string VerificationCode { get; set; }
	}
}
