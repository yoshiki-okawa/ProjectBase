using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Common.General;
using PB.Frameworks.Common.General.Exceptions;
using PB.Frameworks.Resources;
using PB.Frameworks.Types.General;
using PB.Frameworks.Types.Interfaces;
using PB.Frameworks.Utils.Cache;
using PB.Frameworks.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace PB.DataAccess
{
	public class DALDbEntity : IDALDbEntity
	{
		private const string LAST_INSERTED_ID_SQL = "scope_identity()";
		private const string LAST_INSERTED_ID_SQLITE = "last_insert_rowid()";
		private const string INSERT_SQL = "insert into {0} ({1},date_created,date_modified) values ({2},{3},{3});select id, date_modified from {0} where id = {4};";
		private const string UPDATE_SQL = "update {0} set {1},date_modified = {2} where id = @id and date_modified = @date_modified_old";
		private const string DELETE_SQL = "delete from {0} where {1}";
		private const string SELECT_SQL = "select {0} from {1} where {2}";
		private const string SELECT_SQL_MINIMAL = "select {0}";
		private const string EXISTS_SQL = "select exists(select 0 from {0} where {1})";

		private readonly Dictionary<Type, DbType> typeMap = new Dictionary<Type, DbType>
		{
			{typeof(byte), DbType.Byte},
			{typeof(sbyte), DbType.SByte},
			{typeof(short), DbType.Int16},
			{typeof(ushort), DbType.UInt16},
			{typeof(int), DbType.Int32},
			{typeof(uint), DbType.UInt32},
			{typeof(long), DbType.Int64},
			{typeof(ulong), DbType.UInt64},
			{typeof(float), DbType.Single},
			{typeof(double), DbType.Double},
			{typeof(decimal), DbType.Decimal},
			{typeof(bool), DbType.Boolean},
			{typeof(string), DbType.String},
			{typeof(char), DbType.StringFixedLength},
			{typeof(Guid), DbType.Guid},
			{typeof(DateTime), DbType.DateTime},
			{typeof(DateTimeOffset), DbType.DateTimeOffset},
			{typeof(byte[]), DbType.Binary},
			{typeof(byte?), DbType.Byte},
			{typeof(sbyte?), DbType.SByte},
			{typeof(short?), DbType.Int16},
			{typeof(ushort?), DbType.UInt16},
			{typeof(int?), DbType.Int32},
			{typeof(uint?), DbType.UInt32},
			{typeof(long?), DbType.Int64},
			{typeof(ulong?), DbType.UInt64},
			{typeof(float?), DbType.Single},
			{typeof(double?), DbType.Double},
			{typeof(decimal?), DbType.Decimal},
			{typeof(bool?), DbType.Boolean},
			{typeof(char?), DbType.StringFixedLength},
			{typeof(Guid?), DbType.Guid},
			{typeof(DateTime?), DbType.DateTime},
			{typeof(DateTimeOffset?), DbType.DateTimeOffset},
			{typeof(System.Data.Linq.Binary), DbType.Binary}
		};

		private IDbTransaction currentTrans = null;

		/// <summary>
		/// Execute an action in a transaction. Nested transaction cannot be created in different thread.
		/// </summary>
		public void ExecuteInTransaction(Action action)
		{
			Func<IDbCommand, object> actionWithCommand = x => { action(); return null; };
			ExecuteInTransaction(actionWithCommand);
		}

		/// <summary>
		/// Execute an action in a transaction. Nested transaction cannot be created in different thread.
		/// </summary>
		private T ExecuteInTransaction<T>(Func<IDbCommand, T> func)
		{
			bool isInTrans = currentTrans != null;
			IDbConnection connection = DbConnectionHelper.GetDbConnection();

			if (!isInTrans)
				currentTrans = connection.BeginTransaction();

			try
			{
				T returnVal = default(T);
				using (IDbCommand command = connection.CreateCommand())
				{
					command.Prepare();
					command.Transaction = currentTrans;
					returnVal = func(command);
				}
				if (!isInTrans)
				{
					currentTrans.Commit();
					currentTrans.Dispose();
					currentTrans = null;
				}
				return returnVal;
			}
			catch
			{
				if (!isInTrans)
				{
					currentTrans.Rollback();
					currentTrans.Dispose();
					currentTrans = null;
				}
				throw;
			}
		}

		public T ExecuteScalar<T>(CommandType commandType, DbCommandText commandText)
		{
			return ExecuteInTransaction(command =>
			{
				command.CommandType = commandType;
				command.CommandText = commandText.CommandText;
				AddParametersFromDbCommandText(command, commandText);
				return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
			});
		}

		public IDataReader ExecuteReader(CommandType commandType, DbCommandText commandText)
		{
			return ExecuteInTransaction(command =>
			{
				command.CommandType = commandType;
				command.CommandText = commandText.CommandText;
				AddParametersFromDbCommandText(command, commandText);
				return command.ExecuteReader();
			});
		}

		public int ExecuteNonQuery(CommandType commandType, DbCommandText commandText)
		{
			return ExecuteInTransaction(command =>
			{
				command.CommandType = commandType;
				command.CommandText = commandText.CommandText;
				AddParametersFromDbCommandText(command, commandText);
				return command.ExecuteNonQuery();
			});
		}

		public DateTime GetDbDateTime()
		{
			return ExecuteScalar<DateTime>(CommandType.Text, SELECT_SQL_MINIMAL.FormatStr(Config.Instance.GetDateFunction));
		}

		public void InsertDbEntity(IDbEntity de)
		{
			de.Validate(GlobalEnums.ActionType.DbInsert);

			Type genericType = typeof(DbEntityDbInfo<>).MakeGenericType(de.GetType());
			IDbEntityDbInfo dbInfo = (IDbEntityDbInfo)GlobalCache.GetCache(genericType).First();
			Dictionary<string, object> dataParameters = BuildDataParameters(de, dbInfo);
			dataParameters.Remove("@id");
			dataParameters.Remove("@date_created");
			dataParameters.Remove("@date_modified");
			string values = String.Join(",", dataParameters.Keys);
			string fieldNames = values.Replace("@", String.Empty);
			string text = INSERT_SQL.FormatStr(dbInfo.DbTable.TableName, fieldNames, values, Config.Instance.GetDateFunction,
				Config.Instance.DbType == GlobalEnums.DbType.SQL ? LAST_INSERTED_ID_SQL : LAST_INSERTED_ID_SQLITE);

			var commandText = new DbCommandText(text);
			commandText.DataParameters = dataParameters;
			using (IDataReader dr = ExecuteReader(CommandType.Text, commandText))
			{
				dr.Read();
				de.Id = Convert.ToInt32(dr.GetValue(0));
				de.DateCreated = Convert.ToDateTime(dr.GetValue(1));
				de.DateModified = Convert.ToDateTime(dr.GetValue(1));
			}
		}

		public void UpdateDbEntity(IDbEntity deOld, IDbEntity deNew)
		{
			deNew.Validate(GlobalEnums.ActionType.DbUpdate, deOld);
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			if (deNew.Id <= 0)
				LogAndThrowException(connection, "DB_ENTITY_MUST_HAVE_ID", EventLogEntryType.Error, DataAccessLayerResource.DB_OPERATION_UPDATE);

			Type genericType = typeof(DbEntityDbInfo<>).MakeGenericType(deNew.GetType());
			IDbEntityDbInfo dbInfo = (IDbEntityDbInfo)GlobalCache.GetCache(genericType).First();
			Dictionary<string, object> dataParameters = BuildDataParameters(deNew, dbInfo);
			string sets = String.Join(",", dataParameters.Keys.Where(x => x != "@id").Select(x => String.Format("{0} = {1}", x.TrimStart('@'), x)));
			dataParameters.Add("@date_modified_old", deNew.DateModified);
			string text = UPDATE_SQL.FormatStr(dbInfo.DbTable.TableName, sets, Config.Instance.GetDateFunction);
			DbCommandText commandText = new DbCommandText(text);
			commandText.DataParameters = dataParameters;
			int affectedRows = ExecuteNonQuery(CommandType.Text, commandText);
			if (affectedRows == 0)
			{
				IDbEntity de = SelectDbEntityById(deNew.GetType(), deOld.Id, "date_modified");
				if (de == null)
					LogAndThrowException(connection, "DB_ENTITY_NOT_EXIST", EventLogEntryType.Error, dbInfo.DbTable.TableName, deOld.Id);

				DateTime dbDateModified = de.DateModified;
				LogAndThrowException(connection, "DATE_MODIFIED_NOT_MATCH", EventLogEntryType.Error, dbInfo.DbTable.TableName, deOld.Id, dbDateModified, deNew.DateModified);
			}
		}

		protected Dictionary<string, object> BuildDataParameters(IDbEntity de, IDbEntityDbInfo dbInfo)
		{
			var dataParameters = new Dictionary<string, object>();
			foreach (DbEntityMemberInfo memberInfo in dbInfo.DbEntityMemberInfos)
				dataParameters.Add("@" + memberInfo.DbFieldAttribute.FieldName, memberInfo.GetValue(de));
			return dataParameters;
		}

		public void DeleteDbEntity(IDbEntity de)
		{
			de.Validate(GlobalEnums.ActionType.DbDelete);
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			if (de.Id <= 0)
				LogAndThrowException(connection, "DB_ENTITY_MUST_HAVE_ID", EventLogEntryType.Error, DataAccessLayerResource.DB_OPERATION_DELETE);
			DeleteDbEntityById(de.GetType(), de.Id);
		}

		public void DeleteDbEntityById(Type t, int id)
		{
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			if (id <= 0)
				LogAndThrowException(connection, "ID_NOT_SPECIFIED_DELETE", EventLogEntryType.Error, DataAccessLayerResource.DB_OPERATION_DELETE);
			if (!typeof(IDbEntity).IsAssignableFrom(t))
				LogAndThrowException(connection, "TYPE_NOT_SUPPORTED", EventLogEntryType.Error, t.FullName, DataAccessLayerResource.DB_OPERATION_DELETE);

			DeleteDbEntities(t, new DbCommandText("id = @0", id));
		}

		public void DeleteDbEntities(Type t, DbCommandText commandText)
		{
			Type genericType = typeof(DbEntityDbInfo<>).MakeGenericType(t);
			IDbEntityDbInfo dbInfo = (IDbEntityDbInfo)GlobalCache.GetCache(genericType).First();
			ExecuteNonQuery(CommandType.Text, new DbCommandText(DELETE_SQL.FormatStr(dbInfo.DbTable.TableName, commandText.CommandText), commandText.DataParameters.Values.ToArray()));
		}

		public IDbEntity SelectDbEntity(Type t, DbCommandText commandText)
		{
			return SelectDbEntity(t, commandText, GlobalConsts.STAR);
		}

		public IDbEntity SelectDbEntityById(Type t, int id)
		{
			return SelectDbEntity(t, new DbCommandText("id = @0", id), GlobalConsts.STAR);
		}

		public IDbEntity SelectDbEntityById(Type t, int id, string selectFields)
		{
			return SelectDbEntity(t, new DbCommandText("id = @0", id), selectFields);
		}

		public IDbEntity SelectDbEntity(Type t, DbCommandText commandText, string selectFields)
		{
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			if (!typeof(IDbEntity).IsAssignableFrom(t))
				LogAndThrowException(connection, "TYPE_NOT_SUPPORTED", EventLogEntryType.Error, t.FullName, DataAccessLayerResource.DB_OPERATION_SELECT);

			IDbEntity de = (IDbEntity)GlobalUtils.CreateInstance(t);
			Type genericType = typeof(DbEntityDbInfo<>).MakeGenericType(t);
			IDbEntityDbInfo dbInfo = (IDbEntityDbInfo)GlobalCache.GetCache(genericType).First();
			Dictionary<string, DbEntityMemberInfo> memberInfos = dbInfo.DbEntityMemberInfos.ToDictionary(x => x.DbFieldAttribute.FieldName.ToLowerInvariant());
			commandText.CommandText = SELECT_SQL.FormatStr(selectFields, dbInfo.DbTable.TableName, String.IsNullOrEmpty(commandText.CommandText) ? "1 = 1" : commandText.CommandText);
			using (IDataReader dr = ExecuteReader(CommandType.Text, commandText))
			{
				while (dr.Read())
				{
					for (int i = 0; i < dr.FieldCount; i++)
					{
						string fieldName = dr.GetName(i).ToLowerInvariant();
						DbEntityMemberInfo memberInfo;
						if (!memberInfos.TryGetValue(fieldName, out memberInfo))
							continue;
						memberInfo.SetValue(de, dr.GetValue(i));
					}
				}
			}
			if (de.Id == 0)
				return null;
			return de;
		}

		public bool ExistsDbEntity(Type t, DbCommandText commandText)
		{
			bool val = false;
			Type genericType = typeof(DbEntityDbInfo<>).MakeGenericType(t);
			IDbEntityDbInfo dbInfo = (IDbEntityDbInfo)GlobalCache.GetCache(genericType).First();
			commandText.CommandText = EXISTS_SQL.FormatStr(dbInfo.DbTable.TableName, String.IsNullOrEmpty(commandText.CommandText) ? "1 = 1" : commandText.CommandText);
			val = ExecuteScalar<bool>(CommandType.Text, commandText);
			return val;
		}

		private void AddParametersFromDbCommandText(IDbCommand command, DbCommandText commandText)
		{
			foreach (KeyValuePair<string, object> prm in commandText.DataParameters)
			{
				IDbDataParameter prmNew = command.CreateParameter();
				prmNew.ParameterName = prm.Key;
				prmNew.Value = prm.Value;
				Type t = prmNew.Value.GetType();
				if (t.IsEnum)
					t = typeof(int);
				prmNew.DbType = typeMap[t];
				command.Parameters.Add(prmNew);
			}
		}

		private void LogAndThrowException(IDbConnection connection, string msgName, EventLogEntryType entryType, params object[] args)
		{
			Exception ex = ExceptionHelper.BuildDALException(msgName, args);
			if (connection != null)
				ExceptionLogger.LogAndThrowException(ex, entryType);
			else
				throw ex;
		}
	}
}
