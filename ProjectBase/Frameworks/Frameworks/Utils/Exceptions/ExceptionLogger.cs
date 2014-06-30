using Microsoft.Practices.Unity;
using PB.Frameworks.Common.General;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;

namespace PB.Frameworks.Utils.Exceptions
{
	public static class ExceptionLogger
	{
		public static void LogAndThrowException(Exception ex, EventLogEntryType entryType)
		{
			try
			{
				var el = new DEEventLog();
				el.EntryType = entryType;
				var ctx = UnityHelper.Current.Resolve<IContext>();
				if (ctx.User != null)
					el.UserId = ctx.User.Id;
				el.DateCreated = DateTime.UtcNow;
				el.Description = ex.ToString() + Environment.NewLine + new StackTrace(true);
				el.ProcessName = Process.GetCurrentProcess().ProcessName;
				el.ProcessIp = GlobalUtils.GetLocalIPAddress().ToString();
				el.ProcessMachineName = Process.GetCurrentProcess().MachineName;
				el.SourceIp = ctx.Ip ?? String.Empty;
				el.SourceMachineName = ctx.MachineName ?? String.Empty;
				el.ManagedThreadId = Thread.CurrentThread.ManagedThreadId;

				var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
				dde.InsertDbEntity(el);
			}
			catch { }

			throw ex;
		}
	}
}
