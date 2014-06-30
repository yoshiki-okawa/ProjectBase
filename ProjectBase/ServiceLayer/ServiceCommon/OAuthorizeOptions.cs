using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;

namespace PB.Services
{
	public class OAuthorizeOptions
	{
		public static OAuthAuthorizationServerOptions Options = new OAuthAuthorizationServerOptions
		{
			TokenEndpointPath = new PathString("/api/token"),
			Provider = new PBOAuthProvider("self"),
			//AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
			AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
			AllowInsecureHttp = false,
			AccessTokenProvider = new PBAuthenticationTokenProvider()
		};
	}
}
