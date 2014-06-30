using PB.Frameworks.Types.General;
using System;
using System.Data;

namespace PB.Frameworks.Types.Interfaces
{
	public interface IDALDbEntity
	{
		void InsertDbEntity(IDbEntity de);

		void UpdateDbEntity(IDbEntity deOld, IDbEntity deNew);

		void DeleteDbEntity(IDbEntity de);

		void DeleteDbEntities(Type t, DbCommandText commandText);

		void DeleteDbEntityById(Type t, int id);

		IDbEntity SelectDbEntity(Type t, DbCommandText commandText);

		IDbEntity SelectDbEntityById(Type t, int id);

		IDbEntity SelectDbEntity(Type t, DbCommandText commandText, string selectFields);

		bool ExistsDbEntity(Type t, DbCommandText commandText);

		/// <summary>
		/// Execute an action in a transaction. Nested transaction cannot be created in different thread.
		/// </summary>
		void ExecuteInTransaction(Action action);

		T ExecuteScalar<T>(CommandType commandType, DbCommandText commandText);

		IDataReader ExecuteReader(CommandType commandType, DbCommandText commandText);

		int ExecuteNonQuery(CommandType commandType, DbCommandText commandText);

		DateTime GetDbDateTime();
	}
}
