
namespace PB.Frameworks.Types.Interfaces
{
	public interface IContext
	{
		IDEUser User { get; set; }
		string SessionKey { get; set; }
		string Ip { get; set; }
		string MachineName { get; set; }
	}
}
