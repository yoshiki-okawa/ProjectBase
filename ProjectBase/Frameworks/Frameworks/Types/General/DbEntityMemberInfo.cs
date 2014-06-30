using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PB.Frameworks.Types.General
{
	public class DbEntityMemberInfo
	{
		public MemberInfo MemberInfo { get; set; }
		public Type MemberType
		{
			get
			{
				return MemberInfo is FieldInfo ? ((FieldInfo)MemberInfo).FieldType : ((PropertyInfo)MemberInfo).PropertyType;
			}
		}
		public DbFieldAttribute DbFieldAttribute { get; set; }

		private Func<object, object> getValue = null;
		private Action<object, object> setValue = null;
		public object GetValue(IDbEntity de)
		{
			if (getValue == null)
			{
				var prm = Expression.Parameter(typeof(object), "x");
				getValue = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(prm, MemberInfo.DeclaringType), MemberInfo), typeof(object)), prm).Compile();
			}
			return getValue(de);
		}

		public void SetValue(IDbEntity de, object obj)
		{
			if (setValue == null)
			{
				var prm = Expression.Parameter(typeof(object), "x");
				var prm2 = Expression.Parameter(typeof(object), "y");
				setValue = Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.MakeMemberAccess(Expression.Convert(prm, MemberInfo.DeclaringType), MemberInfo), Expression.Convert(prm2, MemberType)), prm, prm2).Compile();
			}
			if (MemberType.IsEnum)
				obj = Enum.Parse(MemberType, obj.ToString());
			setValue(de, Convert.ChangeType(obj, MemberType));
		}
	}
}
