using Microsoft.Owin;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace PB.Services
{
	public class PBMessageHandler : DelegatingHandler
	{
		public PBMessageHandler(HttpConfiguration config)
		{
			InnerHandler = new HttpControllerDispatcher(config);
		}

		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			DependencyHelper.RegisterAllDependencies();
			RegisterContext(request);
			var response = await base.SendAsync(request, cancellationToken);
			return response;
		}

		public static void RegisterContext(object request)
		{
			IContext ctx = UnityHelper.Current.Resolve<IContext>();
			Tuple<string, int> clinetIp = GetClientIp(request);
			if (clinetIp != null)
			{
				ctx.Ip = clinetIp.Item1 + ":" + clinetIp.Item2;
				try
				{
					ctx.MachineName = Dns.GetHostEntry(clinetIp.Item1).HostName;
				}
				catch { }
			}

			UnityHelper.Current.RegisterInstance<IContext>(ctx);
		}

		private static Tuple<string, int> GetClientIp(object request)
		{
			var reqMsg = request as HttpRequestMessage;
			var owinReq = request as IOwinRequest;
			if (reqMsg != null)
			{
				if (reqMsg.Properties.ContainsKey("MS_HttpContext"))
				{
					return Tuple.Create(((HttpContextWrapper)reqMsg.Properties["MS_HttpContext"]).Request.UserHostAddress, 0);
				}
				else if (reqMsg.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
				{
					var prop = (RemoteEndpointMessageProperty)reqMsg.Properties[RemoteEndpointMessageProperty.Name];
					return Tuple.Create(prop.Address, prop.Port);
				}
				else if (reqMsg.Properties.ContainsKey("MS_OwinContext"))
				{
					OwinContext owinContext = reqMsg.Properties["MS_OwinContext"] as OwinContext;
					if (owinContext != null)
						return Tuple.Create(owinContext.Request.RemoteIpAddress, owinContext.Request.RemotePort.HasValue ? owinContext.Request.RemotePort.Value : 0);
					else
						return null;
				}
				else
				{
					return null;
				}
			}
			else if (owinReq != null)
			{
				return Tuple.Create(owinReq.RemoteIpAddress, owinReq.RemotePort.HasValue ? owinReq.RemotePort.Value : 0);
			}
			else
			{
				return null;
			}
		}
	}
}
