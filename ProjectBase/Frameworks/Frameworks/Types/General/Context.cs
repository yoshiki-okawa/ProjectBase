using PB.Frameworks.Types.Interfaces;

namespace PB.Frameworks.Types.General
{
	public class Context : IContext
	{
		public IDEUser User { get; set; }
		public string SessionKey { get; set; }
		public string Ip { get; set; }
		public string MachineName { get; set; }
	}
}
