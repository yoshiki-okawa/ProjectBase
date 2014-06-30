using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Common.General;
using PB.Frameworks.Common.General.Exceptions;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Resources;
using PB.Frameworks.Types.General;
using PB.Frameworks.Types.Interfaces;
using PB.Frameworks.Utils.Exceptions;
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PB.BusinessLogic
{
	public class BLLUserAuth : IBLLUserAuth
	{
		private static object createUserLocker = new object();

		public virtual IContext Login(string emailOrUserName, string password, bool throwException)
		{
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
			string filter = String.Format("{0} = @0 and is_active = 1", emailOrUserName.Contains("@") ? "email" : "name");
			var user = (IDEUser)dde.SelectDbEntity(userType, new DbCommandText(filter, emailOrUserName));
			if (user == null || (emailOrUserName.ToLowerInvariant() == GlobalConsts.SYSTEM.ToLowerInvariant() && !emailOrUserName.Contains("@")))
			{
				if (throwException)
					ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("USERNAME_OR_PASSWORD_INCORRECT"), EventLogEntryType.Error);
				else
					return null;
			}

			IContext ctx = null;
			string[] arr = user.Password.Split('$');
			int iterations = Convert.ToInt32(arr[2]);
			using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(arr[1]), iterations))
			{
				if (Convert.ToBase64String(pbkdf2.GetBytes(24)) == arr[0])
				{
					ctx = UnityHelper.Current.Resolve<IContext>();
					ctx.User = user;
					if (iterations != Config.Instance.PasswordHashIterations)
					{
						IDEUser userOld = user.DeepClone();
						using (var pbkdf22 = new Rfc2898DeriveBytes(password, 20, Config.Instance.PasswordHashIterations))
						{
							user.Password = Convert.ToBase64String(pbkdf22.GetBytes(24)) + "$" + Convert.ToBase64String(pbkdf22.Salt) + "$" + Config.Instance.PasswordHashIterations;
							dde.UpdateDbEntity(userOld, user);
						}
					}
				}
				else if (throwException)
				{
					ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("USERNAME_OR_PASSWORD_INCORRECT"), EventLogEntryType.Error);
				}
			}
			password = null;
			user.Password = null;
			return ctx;
		}

		public virtual void Logout()
		{
			var ctx = UnityHelper.Current.Resolve<IContext>();
			ctx.User = null;
		}

		public virtual void CreateUser(IDEUser user)
		{
			try
			{
				var dde = UnityHelper.Current.Resolve<IDALDbEntity>();

				user.IsActive = !Config.Instance.RegistrationRequiresEmailVerification;
				lock (createUserLocker)
				{
					if (UserExists(new DbCommandText("name = @0 or email = @1", user.Name, user.Email)))
						ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("USER_ALREADY_EXISTS", user.Name, user.Email), EventLogEntryType.Error);

					using (var pbkdf22 = new Rfc2898DeriveBytes(user.Password, 20, Config.Instance.PasswordHashIterations))
					{
						user.Password = Convert.ToBase64String(pbkdf22.GetBytes(24)) + "$" + Convert.ToBase64String(pbkdf22.Salt) + "$" + Config.Instance.PasswordHashIterations;
						dde.InsertDbEntity(user);
					}
				}

				if (Config.Instance.RegistrationRequiresEmailVerification)
				{
					var userAction = UnityHelper.Current.Resolve<IDEUserAction>();
					userAction.UserId = user.Id;
					userAction.Type = GlobalEnums.UserActionType.Registration;
					userAction.VerificationCode = GlobalUtils.GetRandomBase64String();
					dde.InsertDbEntity(userAction);
					string url = String.Format("{0}/verify/{1}/{2}", Config.Instance.BaseWebUIAddress, userAction.Id, userAction.VerificationCode);
					GlobalUtils.SendEmail(user.Email, String.Format(BusinessLogicLayerResource.REGISTER_SUBJECT, Config.Instance.WebsiteName), String.Format(BusinessLogicLayerResource.REGISTER_BODY, Config.Instance.WebsiteName, user.Name, url));
				}
			}
			finally
			{
				user.Password = null;
			}
		}

		public virtual void VerifyCreateUser(int id, string verificationCode)
		{
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			Type userActionType = UnityHelper.GetMappedType(typeof(IDEUserAction));
			var userAction = (IDEUserAction)dde.SelectDbEntityById(userActionType, id);
			if (userAction == null || userAction.VerificationCode != verificationCode || userAction.Type != GlobalEnums.UserActionType.Registration || dde.GetDbDateTime() - userAction.DateCreated > TimeSpan.FromDays(1))
				ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("INVALID_VERIFICATION_CODE"), EventLogEntryType.Error);

			Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
			var user = (IDEUser)dde.SelectDbEntityById(userType, userAction.UserId);
			IDEUser userOld = user.DeepClone();
			user.IsActive = true;
			dde.ExecuteInTransaction(() =>
			{
				dde.UpdateDbEntity(userOld, user);
				dde.DeleteDbEntityById(userActionType, userAction.Id);
			});
		}

		public virtual IContext GetSystemUser()
		{
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
			var user = (IDEUser)dde.SelectDbEntity(userType, new DbCommandText("name = @0", GlobalConsts.SYSTEM));
			IContext ctx = UnityHelper.Current.Resolve<IContext>();
			ctx.User = user;
			return ctx;
		}

		public virtual bool UserExists(DbCommandText filter)
		{
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
			return dde.ExistsDbEntity(userType, filter);
		}

		public virtual bool CheckUser(string userName, string email)
		{
			if (String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(email))
				return false;

			var commandText = new DbCommandText(String.Format("{0} = @0", !String.IsNullOrEmpty(userName) ? "name" : "email"), !String.IsNullOrEmpty(userName) ? userName : email);
			return !UserExists(commandText);
		}

		public virtual void RequestPasswordReset(string emailOrUserName)
		{
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
			string filter = String.Format("{0} = @0 and is_active = 1", emailOrUserName.Contains("@") ? "email" : "name");
			var user = (IDEUser)dde.SelectDbEntity(userType, new DbCommandText(filter, emailOrUserName));
			if (user == null)
				ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("EMAIL_OR_USERNAME_INCORRECT"), EventLogEntryType.Error);

			// Delete existing ones from db first. Also everything should be in a transaction.
			Type userActionType = UnityHelper.GetMappedType(typeof(IDEUserAction));
			dde.DeleteDbEntities(userActionType, new DbCommandText("user_id = @0", user.Id));

			var userAction = UnityHelper.Current.Resolve<IDEUserAction>();
			userAction.UserId = user.Id;
			userAction.Type = GlobalEnums.UserActionType.PasswordReset;
			userAction.VerificationCode = GlobalUtils.GetRandomBase64String();
			dde.InsertDbEntity(userAction);
			string url = String.Format("{0}/resetPassword/{1}/{2}", Config.Instance.BaseWebUIAddress, userAction.Id, userAction.VerificationCode);
			GlobalUtils.SendEmail(user.Email, String.Format(BusinessLogicLayerResource.PASSWORD_RESET_SUBJECT, Config.Instance.WebsiteName), String.Format(BusinessLogicLayerResource.PASSWORD_RESET_BODY, Config.Instance.WebsiteName, user.Name, url));
		}

		public virtual void ResetPassword(string password, int id, string verificationCode)
		{
			IDEUser user = null;
			try
			{
				var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
				Type userActionType = UnityHelper.GetMappedType(typeof(IDEUserAction));
				var userAction = (IDEUserAction)dde.SelectDbEntityById(userActionType, id);
				if (userAction == null || userAction.VerificationCode != verificationCode || userAction.Type != GlobalEnums.UserActionType.PasswordReset || dde.GetDbDateTime() - userAction.DateCreated > TimeSpan.FromDays(1))
					ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildBLLException("INVALID_VERIFICATION_CODE"), EventLogEntryType.Error);

				Type userType = UnityHelper.GetMappedType(typeof(IDEUser));
				user = (IDEUser)dde.SelectDbEntityById(userType, userAction.UserId);
				IDEUser userOld = user.DeepClone();
				user.Password = password;

				using (var pbkdf22 = new Rfc2898DeriveBytes(user.Password, 20, Config.Instance.PasswordHashIterations))
				{
					user.Password = Convert.ToBase64String(pbkdf22.GetBytes(24)) + "$" + Convert.ToBase64String(pbkdf22.Salt) + "$" + Config.Instance.PasswordHashIterations;
				}

				dde.ExecuteInTransaction(() =>
				{
					dde.UpdateDbEntity(userOld, user);
					dde.DeleteDbEntityById(userActionType, userAction.Id);
				});
			}
			finally
			{
				if (user != null)
					user.Password = null;
				password = null;
			}
		}
	}
}
