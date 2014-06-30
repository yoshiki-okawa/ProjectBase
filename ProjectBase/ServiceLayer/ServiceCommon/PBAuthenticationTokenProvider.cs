using Microsoft.Owin.Security.Infrastructure;

namespace PB.Services
{
	public class PBAuthenticationTokenProvider : AuthenticationTokenProvider
	{
		public override void Create(AuthenticationTokenCreateContext context)
		{
			base.Create(context);

			// Use Cookie instead.
			string token = context.SerializeTicket();
			context.Response.Cookies.Append("access_token", token);
			// set dummy token.
			context.SetToken("dummy token");
		}
	}
}
