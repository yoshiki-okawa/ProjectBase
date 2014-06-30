using Microsoft.VisualStudio.TestTools.UnitTesting;
using PB.Frameworks.Common.General;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.Interfaces;
using System;

namespace PB.UnitTests
{
	[TestClass]
	public class BLLUserAuthTests : TestsBase
	{
		[TestMethod]
		public void TestLoginSuccess()
		{
			IContext ctx = bua.Login("admin", adminPassword, true);
			Assert.AreEqual(admin.Id, ctx.User.Id);
			Assert.AreEqual(admin.Name, ctx.User.Name);
			Assert.AreEqual(admin.IsActive, ctx.User.IsActive);
		}

		[TestMethod]
		public void TestLoginFail()
		{
			bool isOK = false;
			try
			{
				IContext ctx = bua.Login("admin", adminPassword + "a", true);
			}
			catch (Exception ex)
			{
				Assert.AreEqual("USERNAME_OR_PASSWORD_INCORRECT", ex.Data[GlobalConsts.EXCEPTION_MESSAGE_NAME_KEY]);
				isOK = true;
			}
			if (!isOK)
				Assert.Fail("Expected exception");
			DEEventLog el = (DEEventLog)dde.SelectDbEntityById(typeof(DEEventLog), 1);
		}
	}
}
