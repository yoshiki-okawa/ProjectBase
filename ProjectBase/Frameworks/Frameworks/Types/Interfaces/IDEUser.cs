
namespace PB.Frameworks.Types.Interfaces
{
	public interface IDEUser : IDbEntity
	{
		string Name { get; set; }
		string Email { get; set; }
		string Password { get; set; }
		bool IsActive { get; set; }
	}
}
