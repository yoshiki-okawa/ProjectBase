using PB.Frameworks.Common.General;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace PB.Frameworks.Common.Extensions
{
	public static class GenericExtensions
	{
		private static HashSet<Type> immutableTypes = new HashSet<Type>
		{
			typeof(String), typeof(DateTime), typeof(TimeSpan), typeof(Boolean), typeof(Byte), typeof(SByte),
			typeof(Int16), typeof(Int32), typeof(Int64), typeof(IntPtr), typeof(UInt16), typeof(UInt32), typeof(UInt64),
			typeof(UIntPtr), typeof(Decimal), typeof(Double), typeof(Single)
		};

		// Types
		public static bool IsImmutable(this Type t)
		{
			return immutableTypes.Contains(t);
		}

		// Deep clone
		public static T DeepClone<T>(this T obj)
		{
			return (T)DeepCloneInternal(obj);
		}

		private static object DeepCloneInternal(object obj)
		{
			return DeepCloner(obj.GetType())(obj);
		}

		private static Dictionary<Type, Func<object, object>> cloners = new Dictionary<Type, Func<object, object>>();

		private static Func<object, object> DeepCloner(Type t)
		{
			Func<object, object> cloner;
			lock (cloners)
			{
				if (cloners.TryGetValue(t, out cloner))
					return cloner;
			}
			var expressions = new List<Expression>();
			var variables = new List<ParameterExpression>();
			ParameterExpression prm = Expression.Parameter(typeof(object), "prm");
			if (t.IsPrimitive || IsImmutable(t) || (t.GetConstructor(Type.EmptyTypes) == null && !t.IsArray && !t.IsValueType))
			{
				expressions.Add(prm);
			}
			else
			{
				ParameterExpression var1 = Expression.Variable(t);
				if (!t.IsArray)
					expressions.Add(Expression.Assign(var1, Expression.Convert(Expression.New(t), t)));
				else
					expressions.Add(Expression.Assign(var1, Expression.Convert(Expression.NewArrayInit(t.GetElementType()), t)));
				ParameterExpression var2 = Expression.Variable(t);
				expressions.Add(Expression.Assign(var2, Expression.Convert(prm, t)));
				variables.Add(var1);
				variables.Add(var2);

				if (t.IsArray)
					expressions.Add(Expression.Assign(var1, GetArrayExpression(t, var2)));

				foreach (FieldInfo fi in GlobalUtils.GetAllFields(t))
				{
					if (fi.FieldType.IsPrimitive || IsImmutable(fi.FieldType) || (fi.FieldType.GetConstructor(Type.EmptyTypes) == null && !fi.FieldType.IsArray && !fi.FieldType.IsValueType))
					{
						Expression exp = Expression.Assign(Expression.Field(var1, fi), Expression.Field(var2, fi));
						expressions.Add(exp);
					}
					else if (fi.FieldType.IsArray)
					{
						ParameterExpression var3 = Expression.Variable(fi.FieldType);
						expressions.Add(Expression.Assign(var3, Expression.Field(var2, fi)));
						variables.Add(var3);
						expressions.Add(Expression.Assign(Expression.Field(var1, fi), GetArrayExpression(fi.FieldType, var3)));
					}
					else
					{
						ParameterExpression var3 = Expression.Variable(fi.FieldType);
						expressions.Add(Expression.Assign(var3, Expression.Field(var2, fi)));
						variables.Add(var3);

						Expression exp = Expression.IfThen(Expression.NotEqual(var3, Expression.Constant(null)),
							Expression.Assign(Expression.Field(var1, fi),
							Expression.Convert(
								Expression.Call(Expression.Call(typeof(GenericExtensions).GetMethod("DeepCloner", BindingFlags.NonPublic | BindingFlags.Static),
									Expression.Call(var3, typeof(object).GetMethod("GetType"))), typeof(Func<object, object>).GetMethod("Invoke"), var3),
									fi.FieldType)));

						expressions.Add(exp);
					}
				}
				expressions.Add(var1);

			}
			Expression resultExpression = Expression.Convert(Expression.Block(variables, expressions), typeof(object));
			cloner = (Func<object, object>)Expression.Lambda<Func<object, object>>(resultExpression, prm).Compile();
			lock (cloners)
				cloners.Add(t, cloner);
			return cloner;
		}

		private static Expression GetComplexExpression(Type t, ParameterExpression target)
		{
			var expressions = new List<Expression>();
			var variables = new List<ParameterExpression>();
			var current = Expression.Variable(t);
			variables.Add(current);
			expressions.Add(Expression.Assign(current, Expression.Convert(Expression.New(t), t)));
			var targetTyped = Expression.Variable(t);
			variables.Add(targetTyped);
			expressions.Add(Expression.Assign(targetTyped, Expression.Convert(target, t)));
			
			foreach (FieldInfo fi in GlobalUtils.GetAllFields(t))
			{
				if (fi.FieldType.IsPrimitive || IsImmutable(fi.FieldType) || (fi.FieldType.GetConstructor(Type.EmptyTypes) == null && !fi.FieldType.IsArray && !fi.FieldType.IsValueType))
				{
					Expression exp = Expression.Assign(Expression.Field(current, fi), Expression.Field(targetTyped, fi));
					expressions.Add(exp);
				}
				else if (fi.FieldType.IsArray)
				{
					ParameterExpression var3 = Expression.Variable(fi.FieldType);
					expressions.Add(Expression.Assign(var3, Expression.Field(targetTyped, fi)));
					variables.Add(var3);
					expressions.Add(Expression.Assign(Expression.Field(current, fi), GetArrayExpression(fi.FieldType, var3)));
				}
				else
				{
					ParameterExpression var3 = Expression.Variable(fi.FieldType);
					expressions.Add(Expression.Assign(var3, Expression.Field(targetTyped, fi)));
					variables.Add(var3);

					Expression exp = Expression.IfThen(Expression.NotEqual(var3, Expression.Constant(null)),
						Expression.Assign(Expression.Field(current, fi),
						Expression.Convert(
							Expression.Call(Expression.Call(typeof(GenericExtensions).GetMethod("DeepCloner", BindingFlags.NonPublic | BindingFlags.Static),
								Expression.Call(var3, typeof(object).GetMethod("GetType"))), typeof(Func<object, object>).GetMethod("Invoke"), var3),
								fi.FieldType)));

					expressions.Add(exp);
				}
			}

			expressions.Add(current);
			return Expression.Block(variables, expressions);
		}

		private static Expression GetArrayExpression(Type t, ParameterExpression target)
		{
			var expressions = new List<Expression>();
			var variables = new List<ParameterExpression>();
			Type elemType = t.GetElementType();
			if (elemType.IsPrimitive || IsImmutable(elemType) || (elemType.GetConstructor(Type.EmptyTypes) == null && !elemType.IsArray && !elemType.IsValueType))
			{
				expressions.Add(Expression.Convert(Expression.Call(Expression.Convert(target, typeof(Array)), typeof(Array).GetMethod("Clone")), t));
				return Expression.Block(expressions);
			}
			var current = Expression.Variable(t);
			variables.Add(current);
			var targetArray = Expression.Variable(t);
			variables.Add(targetArray);
			expressions.Add(Expression.Assign(targetArray, Expression.Convert(target, t)));
			var length = Expression.Call(targetArray, typeof(Array).GetMethod("GetLength"), Expression.Constant(0));
			Expression exp = Expression.IfThen(Expression.NotEqual(targetArray, Expression.Constant(null)),
				Expression.Assign(current,
					Expression.NewArrayBounds(
						elemType,
						length)));
			expressions.Add(exp);
			var @break = Expression.Label();
			var i = Expression.Variable(typeof(int));
			expressions.Add(Expression.Assign(i, Expression.Constant(-1)));
			variables.Add(i);
			var currentValue = Expression.Variable(typeof(object));
			variables.Add(currentValue);
			Expression innerLoop1 = Expression.Assign(currentValue, Expression.Convert(Expression.ArrayAccess(targetArray, i), typeof(object)));
			Expression rightExp;
			if (elemType.IsArray)
			{
				rightExp = GetArrayExpression(elemType, currentValue);
			}
			else if (elemType.IsValueType)
			{
				rightExp = GetComplexExpression(elemType, currentValue);
			}
			else
			{
				rightExp = Expression.Call(
							Expression.Call(
								typeof(GenericExtensions).GetMethod("DeepCloner", BindingFlags.NonPublic | BindingFlags.Static),
								Expression.Call(currentValue, typeof(object).GetMethod("GetType"))),
							typeof(Func<object, object>).GetMethod("Invoke"),
							currentValue);
			}
			Expression innerLoop2 = Expression.IfThen(
				Expression.NotEqual(currentValue, Expression.Constant(null)),
				Expression.Assign(
					Expression.ArrayAccess(current, i),
					Expression.Convert(
							rightExp,
						elemType)));
			Expression innerLoop = Expression.Block(innerLoop1, innerLoop2);

			Expression loop = Expression.IfThen(Expression.NotEqual(current, Expression.Constant(null)),
				Expression.Loop(
					Expression.IfThenElse(
					  Expression.LessThan(Expression.PreIncrementAssign(i), length),
					  innerLoop,
					  Expression.Break(@break)),
				  @break));
			expressions.Add(loop);
			expressions.Add(current);
			return Expression.Block(variables, expressions);
		}
	}
}
