namespace PB.Frameworks.Common.General
{
	public static class GlobalEnums
	{
		public enum ActionType
		{
			None = 0,
			DbInsert = 1,
			DbUpdate = 2,
			DbDelete = 3
		}

		public enum UserActionType
		{
			None = 0,
			Registration = 1,
			PasswordReset = 2
		}

		public enum DbType
		{
			None = 0,
			SQL = 1,
			SQLite = 2
		}
	}
}
