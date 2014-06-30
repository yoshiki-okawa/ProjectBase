using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PB.Services
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class OAuthorizeAttribute : AuthorizeAttribute
	{
		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			base.OnAuthorization(actionContext);

			if (actionContext.ControllerContext == null || actionContext.ControllerContext.Controller == null)
				HandleUnauthorizedRequest(actionContext);

			var controller = actionContext.ControllerContext.Controller as ApiController;
			if (controller == null || controller.User == null || controller.User.Identity == null)
				HandleUnauthorizedRequest(actionContext);

			var ctx = UnityHelper.Current.Resolve<IContext>();
			var jss = new JavaScriptSerializer();
			// If primary id is Bearer and authenticated, OK to continue.
			if (controller.User.Identity.AuthenticationType == OAuthDefaults.AuthenticationType && controller.User.Identity.IsAuthenticated)
			{
				if (SetContextUser(ctx, (ClaimsIdentity)controller.User.Identity, jss))
					return;
			}

			// Replace current identity with Bearer one if any.
			bool isAuthenticated = false;
			foreach (var identity in actionContext.Request.GetOwinContext().Authentication.User.Identities)
			{
				if (identity.AuthenticationType == OAuthDefaults.AuthenticationType && identity.IsAuthenticated)
				{
					controller.User = new ClaimsPrincipal(identity);
					if (SetContextUser(ctx, (ClaimsIdentity)identity, jss))
						isAuthenticated = true;
					break;
				}
			}
			if (!isAuthenticated)
				HandleUnauthorizedRequest(actionContext);
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
