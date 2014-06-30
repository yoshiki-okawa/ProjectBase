using Microsoft.Practices.Unity;
using PB.BusinessLogic;
using PB.DataAccess;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.DatabaseEntities;
using PB.Frameworks.Types.General;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Http;

namespace PB.Services
{
	public static class DependencyHelper
	{
		private static List<Type> pluginBLLBaseTypes = new List<Type>();
		private static Type pluginBLLUserAuth = null;
		private static Type pluginDEUser = null;
		private static Type pluginDEUserAction = null;
		private static Type pluginContext = null;
		private static List<Assembly> pluginApiControllerAssemblies = new List<Assembly>();

		static DependencyHelper()
		{
			Initialize();
		}

		private static void Initialize()
		{
			if (!Directory.Exists("plugins"))
				return;

			string[] pluginPaths = Directory.GetFiles("plugins", "*.dll", SearchOption.AllDirectories);
			foreach (string pluginPath in pluginPaths)
			{
				Assembly assembly = Assembly.LoadFrom(pluginPath);
				Type[] types = assembly.GetTypes();
				bool hasApiController = false;
				foreach (Type t in types)
				{
					if (typeof(IDEUser).IsAssignableFrom(t))
						pluginDEUser = t;
					if (typeof(IDEUserAction).IsAssignableFrom(t))
						pluginDEUser = t;
					if (typeof(IContext).IsAssignableFrom(t))
						pluginContext = t;
					if (typeof(BLLUserAuth).IsAssignableFrom(t))
						pluginBLLUserAuth = t;
					if (typeof(ApiController).IsAssignableFrom(t))
						hasApiController = true;
				}
				if (hasApiController)
					pluginApiControllerAssemblies.Add(assembly);
			}
		}

		public static void RegisterAllDependencies()
		{
			UnityHelper.Current.RegisterType<IDALDbEntity, DALDbEntity>();
			if (pluginBLLUserAuth == null)
				UnityHelper.Current.RegisterType<IBLLUserAuth, BLLUserAuth>();
			else
				UnityHelper.Current.RegisterType(typeof(IBLLUserAuth), pluginBLLUserAuth);

			if (pluginDEUser == null)
				UnityHelper.Current.RegisterType<IDEUser, DEUser>();
			else
				UnityHelper.Current.RegisterType(typeof(IDEUser), pluginDEUser);

			if (pluginDEUserAction == null)
				UnityHelper.Current.RegisterType<IDEUserAction, DEUserAction>();
			else
				UnityHelper.Current.RegisterType(typeof(IDEUserAction), pluginDEUserAction);

			if (pluginContext == null)
				UnityHelper.Current.RegisterType<IContext, Context>();
			else
				UnityHelper.Current.RegisterType(typeof(IContext), pluginContext);
		}

		public static List<Assembly> GetPluginApiControllerAssemlies()
		{
			return pluginApiControllerAssemblies;
		}
	}
}
