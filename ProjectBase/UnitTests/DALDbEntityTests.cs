using Microsoft.VisualStudio.TestTools.UnitTesting;
using PB.DataAccess;
using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.General;
using System;
using System.Data;

namespace PB.UnitTests
{
	[DbTable(TableName = "test_entity")]
	public class TestDbEntity : DbEntity
	{
		[DbField(FieldName = "test_int")]
		public int TestInt { get; set; }
		[DbField(FieldName = "test_date_time")]
		public DateTime TestDateTime { get; set; }
		[DbField(FieldName = "test_decimal")]
		public decimal TestDecimal { get; set; }
		[DbField(FieldName = "test_text")]
		public string TestString { get; set; }
	}

	[TestClass]
	public class DALDbEntityTests : TestsBase
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			DbEntityDbInfo<DbEntity> dbInfo = new DbEntityDbInfo<DbEntity>();
			dde.ExecuteNonQuery(CommandType.Text, @"
drop table if exists test_entity;
create table test_entity(
	id						INTEGER			PRIMARY KEY,
	date_created		datetime			NOT NULL,
	date_modified		datetime			NOT NULL,
	test_int				INTEGER			NOT NULL,
	test_date_time		datetime			NOT NULL,
	test_decimal		decimal(24,9)	NOT NULL,
	test_text			TEXT				NOT NULL
);");
		}

		[TestMethod]
		public void TestInsertDbEntity()
		{
			DateTime now = DateTime.Now;
			TestDbEntity entity = new TestDbEntity
			{
				TestInt = 123,
				TestDateTime = now,
				TestDecimal = 100M,
				TestString = "test"
			};
			dde.InsertDbEntity(entity);
			dde.InsertDbEntity(entity);
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandType = CommandType.Text;
				command.CommandText = @"select * from test_entity;";
				using (IDataReader reader = command.ExecuteReader())
				{
					int i = 0;
					while (reader.Read())
					{
						i++;
						Assert.AreEqual(i, Convert.ToInt32(reader.GetValue(0)), "id");
						Assert.AreEqual(123, Convert.ToInt32(reader.GetValue(3)), "test_int");
						Assert.AreEqual(now, Convert.ToDateTime(reader.GetValue(4)), "test_date_time");
						Assert.AreEqual(100M, Convert.ToDecimal(reader.GetValue(5)), "test_decimal");
						Assert.AreEqual("test", Convert.ToString(reader.GetValue(6)), "test_text");
					}
					Assert.AreEqual(2, i, "expected number of rows");
				}
			}
		}

		[TestMethod]
		public void TestUpdateDbEntity()
		{
			DateTime now = DateTime.Now;
			TestDbEntity entity = new TestDbEntity
			{
				TestInt = 123,
				TestDateTime = now,
				TestDecimal = 100M,
				TestString = "test"
			};
			dde.InsertDbEntity(entity);
			DbEntity entityOld = entity.DeepClone();
			entity.TestInt = 124;
			entity.TestDateTime = now.AddDays(1).AddHours(1).AddMinutes(1).AddSeconds(1).AddMilliseconds(1);
			entity.TestDecimal = 101M;
			entity.TestString = "test2";
			dde.UpdateDbEntity(entityOld, entity);
			IDbConnection connection = DbConnectionHelper.GetDbConnection();
			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandType = CommandType.Text;
				command.CommandText = @"select * from test_entity;";
				using (IDataReader reader = command.ExecuteReader())
				{
					int i = 0;
					while (reader.Read())
					{
						i++;
						Assert.AreEqual(1, Convert.ToInt32(reader.GetValue(0)), "id");
						Assert.AreEqual(124, Convert.ToInt32(reader.GetValue(3)), "test_int");
						Assert.AreEqual(now.AddDays(1).AddHours(1).AddMinutes(1).AddSeconds(1).AddMilliseconds(1), Convert.ToDateTime(reader.GetValue(4)), "test_date_time");
						Assert.AreEqual(101M, Convert.ToDecimal(reader.GetValue(5)), "test_decimal");
						Assert.AreEqual("test2", Convert.ToString(reader.GetValue(6)), "test_text");
					}
					Assert.AreEqual(1, 1, "expected number of rows");
				}
			}
		}

		[TestMethod]
		public void TestDeleteDbEntity()
		{
			DateTime now = DateTime.Now;
			TestDbEntity entity = new TestDbEntity
			{
				TestInt = 123,
				TestDateTime = now,
				TestDecimal = 100M,
				TestString = "test"
			};
			dde.InsertDbEntity(entity);
			dde.DeleteDbEntity(entity);
			Assert.AreEqual(null, dde.SelectDbEntityById(typeof(TestDbEntity), entity.Id));
			Assert.AreEqual(false, dde.ExistsDbEntity(typeof(TestDbEntity), new DbCommandText("id = @0", entity.Id)));
		}
	}
}
