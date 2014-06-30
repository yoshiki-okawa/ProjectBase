using PB.Frameworks.Common.General;
using PB.Frameworks.Common.General.Exceptions;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Threading;

namespace PB.DataAccess
{
	public static class DbConnectionHelper
	{
		private static ThreadLocal<IDbConnection> dbConnection = new ThreadLocal<IDbConnection>();
		public static IDbConnection GetDbConnection()
		{
			IDbConnection connection = dbConnection.Value;
			if (connection == null || connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
			{
				if (connection != null && (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken))
				{
					connection.Dispose();
				}

				if (Config.Instance.DbType == GlobalEnums.DbType.SQLite)
				{
					connection = new SQLiteConnection(Config.Instance.ConnectionString);
				}
				else if (Config.Instance.DbType == GlobalEnums.DbType.SQL)
				{
					connection = new SqlConnection(Config.Instance.ConnectionString);
				}
				if (connection == null)
					throw ExceptionHelper.BuildDALException("NOT_SUPPORTED_DB_TYPE", Config.Instance.DbType.ToString());
				dbConnection.Value = connection;
				connection.Open();
			}
			return connection;
		}
	}
}
