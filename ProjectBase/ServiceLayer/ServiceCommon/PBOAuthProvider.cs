using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Resources;
using PB.Frameworks.Types.General;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PB.Services
{
	public class PBOAuthProvider : OAuthAuthorizationServerProvider
	{
		private readonly string _publicClientId;

		public PBOAuthProvider(string publicClientId)
		{
			if (publicClientId == null)
			{
				throw new ArgumentNullException("publicClientId");
			}

			_publicClientId = publicClientId;
		}

		public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
		{
			return base.AuthorizeEndpoint(context);
		}

		public override Task MatchEndpoint(OAuthMatchEndpointContext context)
		{
			return base.MatchEndpoint(context);
		}

		public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
		{
			return base.ValidateAuthorizeRequest(context);
		}

		public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
		{
			return base.ValidateTokenRequest(context);
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var jss = new JavaScriptSerializer();
			DependencyHelper.RegisterAllDependencies();
			PBMessageHandler.RegisterContext(context.Request);
			var bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			IDEUser user = GetUserFromIdentity(context, jss, bua);

			if (user == null && !String.IsNullOrEmpty(context.UserName) && !String.IsNullOrEmpty(context.Password))
			{
				var ctx = bua.Login(context.UserName, context.Password, false);

				if (ctx != null && ctx.User != null)
					user = ctx.User;
			}

			if (user == null)
			{
				context.SetError("invalid_grant", ServicesResource.USERNAME_OR_PASSWORD_INCORRECT);
				return;
			}

			// Claims are encrypted.
			// "As a side note, the contents of the cookie are protected as you’d expect (signed and encrypted). This protection is by default, which is good." - Brock Allen
			// http://brockallen.com/2013/10/24/a-primer-on-owin-cookie-authentication-middleware-for-the-asp-net-developer/
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.UserData, jss.Serialize(user), user.GetType().AssemblyQualifiedName));
			var oId = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
			var cId = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

			// This AuthenticationProperties is not encrypted.
			AuthenticationTicket ticket = new AuthenticationTicket(oId, GetAuthProps(user, false)); // dummy ticket
			context.Validated(ticket);
			IFormCollection data = await context.Request.ReadFormAsync();
			context.Request.Context.Authentication.SignIn(GetAuthProps(user, Convert.ToBoolean(data["rememberMe"])), cId);
			return;
		}

		private IDEUser GetUserFromIdentity(OAuthGrantResourceOwnerCredentialsContext context, JavaScriptSerializer jss, IBLLUserAuth bua)
		{
			IDEUser user = null;
			if (context.OwinContext.Authentication.User == null || context.OwinContext.Authentication.User.Identity == null || context.OwinContext.Authentication.User.Identities == null || context.OwinContext.Authentication.User.Claims == null)
				return user;

			foreach (var identity in context.OwinContext.Authentication.User.Identities)
			{
				if ((identity.AuthenticationType != OAuthDefaults.AuthenticationType && identity.AuthenticationType != CookieAuthenticationDefaults.AuthenticationType) || !identity.IsAuthenticated)
					continue;

				Claim nameClaim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData);
				if (nameClaim == null)
					continue;
				try
				{
					user = (IDEUser)jss.Deserialize(nameClaim.Value, UnityHelper.GetType(nameClaim.ValueType));
				}
				catch { }
				if (user != null && !bua.UserExists(new DbCommandText("id = @0 and is_active = 1", user.Id)))
					user = null;
			}
			return user;
		}

		public override Task TokenEndpoint(OAuthTokenEndpointContext context)
		{
			foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
			{
				context.AdditionalResponseParameters.Add(property.Key, property.Value);
			}

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			// Resource owner password credentials does not provide a client ID.
			if (context.ClientId == null)
			{
				context.Validated();
			}

			return Task.FromResult<object>(null);
		}

		public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
		{
			if (context.ClientId == _publicClientId)
			{
				Uri expectedRootUri = new Uri(context.Request.Uri, "/");

				if (expectedRootUri.AbsoluteUri == context.RedirectUri)
				{
					context.Validated();
				}
			}

			return Task.FromResult<object>(null);
		}

		private AuthenticationProperties GetAuthProps(IDEUser user, bool isPersistent)
		{
			// This AuthenticationProperties is not encrypted.
			var props = new AuthenticationProperties();
			props.IsPersistent = isPersistent;
			props.Dictionary.Add("user_name", user.Name);

			return props;
		}
	}
}