using Microsoft.Practices.Unity;
using PB.BusinessLogic;
using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.Interfaces;
using System;

namespace SamplePlugins
{
	[Serializable]
	public class DECustomUser : DEUser, IDbEntity
	{
		[DbField(FieldName = "last_log_in_ip")]
		public string LastLogInIp { get; set; }
	}

	public class BLLCustomUserAuth : BLLUserAuth
	{
		public override IContext Login(string emailOrUserName, string password, bool throwException)
		{
			IContext ctx = base.Login(emailOrUserName, password, throwException);
			if (ctx == null)
				return ctx;
			// Keep track of the last ip used for user login.
			var dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			var user = (DECustomUser)dde.SelectDbEntityById(typeof(DECustomUser), ctx.User.Id);
			var userOld = user.DeepClone();
			user.LastLogInIp = ctx.Ip;
			dde.UpdateDbEntity(userOld, user);

			return ctx;
		}
	}
}
