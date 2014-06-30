using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Security.Cookies;
using Owin;
using PB.Hubs;
using ServiceCommon;
using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace PB.Services
{
	public class Startup
	{
		// This code configures Web API contained in the class Startup, which is additionally specified as the type parameter in WebApplication.Start
		public void Configuration(IAppBuilder app)
		{
			// Configure Web API for Self-Host
			HttpConfiguration config = new HttpConfiguration();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional },
				constraints: null,
				handler: new PBMessageHandler(config)
			);

			// false; otherwise, any object which implements IXmlSerializable will be serialised to XML.
			config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
			config.Filters.Add(new PBValidationFilterAttribute());
			config.Filters.Add(new PBGlobalExceptionFilterttribute());

			config.Services.Replace(typeof(IAssembliesResolver), new PBAssemblyResolver());
			// Enable the application to use a cookie to store information for the signed in user
			// and to use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseCookieAuthentication(new CookieAuthenticationOptions() { CookieSecure = CookieSecureOption.SameAsRequest, ExpireTimeSpan = TimeSpan.FromDays(14) });

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthorizeOptions.Options);

			app.UseWebApi(config);

			// If this is before usewebapi, entire web api will be open for cross origins.
			// Should never use Cors because it will open up a hole for CSRF unless specifying a list of trusted sites.
			// If required, add Microsoft.Owin.Cors from nuget.
			// app.UseCors(CorsOptions.AllowAll);

			var resolver = new DefaultDependencyResolver();
			resolver.Register(typeof(ChatHub), () => new ChatHub());
			IHubPipeline hubPipeline = resolver.Resolve<IHubPipeline>();

			hubPipeline.AddModule(new PBSignalRModule());
			var authorizer = new OAuthorizeSignalRAttribute();
			var module = new AuthorizeModule(authorizer, authorizer);
			hubPipeline.AddModule(module);

			app.MapSignalR(new HubConfiguration { Resolver = resolver });
		}
	}
}
