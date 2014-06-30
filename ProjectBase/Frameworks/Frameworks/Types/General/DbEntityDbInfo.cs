using PB.Frameworks.Common.General;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PB.Frameworks.Types.General
{
	public class DbEntityDbInfo<T> : ICacheable, IDbEntityDbInfo where T : IDbEntity
	{
		public DbTableAttribute DbTable { get; set; }
		public List<DbEntityMemberInfo> DbEntityMemberInfos { get; set; }

		public List<object> LoadAll()
		{
			var cache = new List<object>();
			var dbInfo = new DbEntityDbInfo<T>();
			Type t = typeof(T);
			dbInfo.DbTable = t.GetCustomAttribute<DbTableAttribute>(true);
			dbInfo.DbEntityMemberInfos = new List<DbEntityMemberInfo>();
            HashSet<string> memberNames = new HashSet<string>();
			foreach (MemberInfo mi in GlobalUtils.GetAllMembers(t))
			{
				var dbFieldAttribute = mi.GetCustomAttribute<DbFieldAttribute>();
                if (dbFieldAttribute != null && !memberNames.Contains(mi.Name))
				{
					var fieldInfo = new DbEntityMemberInfo()
					{
						DbFieldAttribute = dbFieldAttribute,
						MemberInfo = mi
					};
					dbInfo.DbEntityMemberInfos.Add(fieldInfo);
                    memberNames.Add(mi.Name);
				}
			}
			cache.Add(dbInfo);
			return cache;
		}
	}
}
