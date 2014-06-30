
using PB.Frameworks.Types.General;
namespace PB.Frameworks.Types.Interfaces
{
	public interface IBLLUserAuth
	{
		IContext Login(string emailOrUserName, string password, bool throwException);
		void Logout();
		void CreateUser(IDEUser user);
		bool UserExists(DbCommandText commandText);
		void VerifyCreateUser(int id, string verificationCode);
		bool CheckUser(string userName, string email);
		void RequestPasswordReset(string emailOrUserName);
		void ResetPassword(string password, int id, string verificationCode);
	}
}
