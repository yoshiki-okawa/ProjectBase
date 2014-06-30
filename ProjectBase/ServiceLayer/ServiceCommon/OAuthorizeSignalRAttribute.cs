using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Script.Serialization;

namespace PB.Services
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class OAuthorizeSignalRAttribute : AuthorizeAttribute
	{
		// http://geekswithblogs.net/shaunxu/archive/2014/05/27/set-context-user-principal-for-customized-authentication-in-signalr.aspx
		public override bool AuthorizeHubMethodInvocation(Microsoft.AspNet.SignalR.Hubs.IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
		{
			/**/

			DependencyHelper.RegisterAllDependencies();
			PBMessageHandler.RegisterContext(hubIncomingInvokerContext.Hub.Context.Request);
			base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);

			var connectionId = hubIncomingInvokerContext.Hub.Context.ConnectionId;
			// check the authenticated user principal from environment
			var environment = hubIncomingInvokerContext.Hub.Context.Request.Environment;
			var dataProtectionProvider = new DpapiDataProtectionProvider();
			var secureDataFormat = new TicketDataFormat(dataProtectionProvider.Create());
			var token = hubIncomingInvokerContext.Hub.Context.QueryString.Get(OAuthDefaults.AuthenticationType);
			if (token == null || token.Length == 0)
				return false;
			var ticket = OAuthorizeOptions.Options.AccessTokenFormat.Unprotect(token);
			if (ticket != null && ticket.Identity != null && ticket.Identity.IsAuthenticated)
			{
				environment["server.User"] = new ClaimsPrincipal(ticket.Identity);
				// create a new HubCallerContext instance with the principal generated from token
				// and replace the current context so that in hubs we can retrieve current user identity
				hubIncomingInvokerContext.Hub.Context = new HubCallerContext(new ServerRequest(environment), connectionId);
				return IsUserAuthorized(hubIncomingInvokerContext.Hub.Context.User);
			}
			else
			{
				return false;
			}
		}

		private bool IsUserAuthorized(System.Security.Principal.IPrincipal user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var principal = (ClaimsPrincipal)user;
			var ctx = UnityHelper.Current.Resolve<IContext>();
			var jss = new JavaScriptSerializer();
			// If primary id is Bearer and authenticated, OK to continue.
			if (user.Identity.AuthenticationType == OAuthDefaults.AuthenticationType && user.Identity.IsAuthenticated)
			{
				if (SetContextUser(ctx, (ClaimsIdentity)user.Identity, jss))
					return true;
			}

			foreach (var identity in principal.Identities)
			{
				if (identity.AuthenticationType == OAuthDefaults.AuthenticationType && identity.IsAuthenticated)
				{
					if (SetContextUser(ctx, (ClaimsIdentity)identity, jss))
						return true;
					break;
				}
			}
			return false;
		}

		private bool SetContextUser(IContext ctx, ClaimsIdentity identity, JavaScriptSerializer jss)
		{
			bool success = false;
			Claim nameClaim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData);
			if (nameClaim != null)
			{
				try
				{
					ctx.User = (IDEUser)jss.Deserialize(nameClaim.Value, UnityHelper.GetType(nameClaim.ValueType));
					// Because this is authenticated via token per session, we do not need to check user exists in db or not unless sign-out properly.
					if (ctx.User != null)
						success = true;
				}
				catch { }
			}
			return success;
		}
	}
}
