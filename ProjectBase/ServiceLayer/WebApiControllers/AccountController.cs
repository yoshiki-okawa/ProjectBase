using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Interfaces;
using PB.Services;
using PB.Services.DataContracts;
using System.Net.Http;
using System.Web.Http;

namespace PB.WebApiControllers
{
	public class AccountController : ApiController
	{
		// POST api/account/register
		[AllowAnonymous]
		[HttpPost]
		public DCRegisterResult Register([FromBody]DCRegisterArgs args)
		{
			IDEUser user = UnityHelper.Current.Resolve<IDEUser>();
			user.Name = args.UserName;
			user.Email = args.Email;
			user.Password = args.Password;
			args.Password = null;

			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bua.CreateUser(user);

			return new DCRegisterResult { Success = true };
		}

		// POST api/account/verify
		[AllowAnonymous]
		[HttpPost]
		public DCVerifyResult Verify([FromBody]DCVerifyArgs args)
		{
			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bua.VerifyCreateUser(args.Id, args.VerificationCode);

			return new DCVerifyResult { Success = true };
		}

		// POST api/account/checkUser
		[AllowAnonymous]
		[HttpPost]
		public DCCheckUserResult CheckUser([FromBody]DCCheckUserArgs args)
		{
			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bool result = bua.CheckUser(args.UserName, args.Email);

			return new DCCheckUserResult { Success = result };
		}

		// POST api/account/requestPasswordReset
		[AllowAnonymous]
		[HttpPost]
		public DCRequestPasswordResetResult RequestPasswordReset([FromBody]DCRequestPasswordResetArgs args)
		{
			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bua.RequestPasswordReset(args.EmailOrUserName);

			return new DCRequestPasswordResetResult { Success = true };
		}

		// POST api/account/resetPassword
		[AllowAnonymous]
		[HttpPost]
		public DCResetPasswordResult ResetPassword([FromBody]DCResetPasswordArgs args)
		{
			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bua.ResetPassword(args.Password, args.Id, args.VerificationCode);

			return new DCResetPasswordResult { Success = true };
		}

		// POST api/account/logout
		[OAuthorize]
		[HttpPost]
		public bool Logout()
		{
			IBLLUserAuth bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			bua.Logout();
			Authentication.SignOut();

			return true;
		}

		// POST api/account/loadUserDetails
		[OAuthorize]
		[HttpPost]
		public DCLoadUserDetailsResult LoadUserDetails()
		{
			var ctx = UnityHelper.Current.Resolve<IContext>();

			return new DCLoadUserDetailsResult { Success = true, UserName = ctx.User.Name, Email = ctx.User.Email };
		}

		private IAuthenticationManager Authentication
		{
			get { return Request.GetOwinContext().Authentication; }
		}
	}
}
