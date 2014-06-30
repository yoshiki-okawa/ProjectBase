using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Threading;

namespace PB.Frameworks.Common.Unity
{
	public static class UnityHelper
	{
		private static ThreadLocal<IUnityContainer> current = new ThreadLocal<IUnityContainer>(() => new UnityContainer());

		public static IUnityContainer Current
		{
			get
			{
				return current.Value;
			}
			set
			{
				current.Value = value;
			}
		}

		public static Type GetMappedType(Type t)
		{
			return Current.Registrations.FirstOrDefault(x => x.RegisteredType == t).MappedToType;
		}

		public static Type GetType(string assemblyQualifiedName)
		{
			return Current.Registrations.FirstOrDefault(x => x.MappedToType.AssemblyQualifiedName == assemblyQualifiedName).MappedToType;
		}
	}
}
