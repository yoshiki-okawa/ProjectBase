using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PB.BusinessLogic;
using PB.DataAccess;
using PB.Frameworks.Common.General;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.General;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Data;

namespace PB.UnitTests
{
	public class TestsBase
	{
		protected IBLLUserAuth bua;
		protected IDALDbEntity dde;
		protected string adminPassword;
		protected DEUser admin;

		[TestInitialize]
		public virtual void TestInitialize()
		{
			UnityHelper.Current.RegisterType<IDEUser, DEUser>();
			UnityHelper.Current.RegisterType<IContext, Context>();
			UnityHelper.Current.RegisterType<IBLLUserAuth, BLLUserAuth>();
			UnityHelper.Current.RegisterType<IDALDbEntity, DALDbEntity>();
			dde = UnityHelper.Current.Resolve<IDALDbEntity>();
			bua = UnityHelper.Current.Resolve<IBLLUserAuth>();
			dde.ExecuteNonQuery(CommandType.Text, @"
PRAGMA foreign_keys = ON;
drop table if exists entity;
create table entity(
	id						INTEGER			PRIMARY KEY,
	date_created		datetime			NOT NULL,
	date_modified		datetime			NOT NULL,
	test_int				INTEGER			NOT NULL,
	test_date_time		datetime			NOT NULL,
	test_decimal		decimal(24,9)	NOT NULL,
	test_text			TEXT				NOT NULL
);
drop table if exists user;
create table user(
	id						INTEGER			PRIMARY KEY,
	date_created		datetime			NOT NULL,
	date_modified		datetime			NOT NULL,
	name					TEXT				NOT NULL UNIQUE,
	email					TEXT				NOT NULL UNIQUE,
	password				TEXT				NOT NULL,
	is_active			BOOL				NOT NULL,
	last_log_in_ip		TEXT				NULL /* for sample plugin */
);
drop table if exists user_action;
create table user_action(
	id						INTEGER			PRIMARY KEY,
	date_created		datetime			NOT NULL,
	date_modified		datetime			NOT NULL,
	user_id				INTEGER			NOT NULL,
	type					INTEGER			NOT NULL,
	verification_code	TEXT				NOT NULL,
	FOREIGN KEY(user_id) REFERENCES user(user_id)
);
drop table if exists event_log;
create table event_log(
	id							INTEGER			PRIMARY KEY,
	date_created		datetime			NOT NULL,
	date_modified			datetime				NOT NULL,
	description				text				NOT NULL,
	entry_type				int				NOT NULL,
	user_id					int				NOT NULL,
	process_name			text				NOT NULL,
	source_machine_name	text				NOT NULL,
	process_machine_name	text				NOT NULL,
	source_ip				text				NOT NULL,
	process_ip				text				NOT NULL,
	managed_thread_id		int				NOT NULL
);");

			if (!dde.ExistsDbEntity(typeof(DEUser), new DbCommandText(String.Empty)))
			{
				adminPassword = GlobalUtils.GetRandomBase64String();
				Console.WriteLine("generated password: {0}", adminPassword);
				admin = new DEUser
				{
					Name = "admin",
					Email = "admin@test.com",
					Password = adminPassword,
					IsActive = true
				};
				bua.CreateUser(admin);
				IContext ctx = UnityHelper.Current.Resolve<IContext>();
				ctx.User = admin;
				UnityHelper.Current.RegisterInstance<IContext>(ctx);
				DEUser system = new DEUser
				{
					Name = "system",
					Email = "system@test.com",
					Password = String.Empty,
					IsActive = true
				};
				bua.CreateUser(system);
			}
		}
	}
}
