using Microsoft.Owin.Hosting;
using PB.Frameworks.Common.General;
using System;
using System.ServiceProcess;

namespace PB.Services
{
	public class WinService : ServiceBase
	{
		public IDisposable webApp = null;
		public WinService()
		{
			ServiceName = "PBWinService";
		}

		public static void Main(string[] args)
		{
			var serviceToRun = new WinService();
			if (Environment.UserInteractive)
			{
				// run the service as a console for debugging purpose during development phase.
				serviceToRun.OnStart(null);

				Console.WriteLine("Server running at {0}", Config.Instance.BaseWebApiAddress);
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();

				serviceToRun.OnStop();
			}
			else
			{
				ServiceBase.Run(serviceToRun);
			}
		}

		// Start the Windows service.
		protected override void OnStart(string[] args)
		{
			webApp = WebApp.Start<Startup>(Config.Instance.BaseWebApiAddress);
		}

		protected override void OnStop()
		{
			if (webApp != null)
				webApp.Dispose();
		}
	}
}
