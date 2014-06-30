using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Resources;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PB.Frameworks.Common.General.Exceptions
{
	public static class ExceptionHelper
	{
		public static Exception BuildUtilsException(string msgName, params object[] args)
		{
			return BuildException(msgName, UtilsResource.ResourceManager.GetString(msgName), args);
		}

		public static Exception BuildTypesException(string msgName, params object[] args)
		{
			return BuildException(msgName, TypesResource.ResourceManager.GetString(msgName), args);
		}

		public static Exception BuildDALException(string msgName, params object[] args)
		{
			return BuildException(msgName, DataAccessLayerResource.ResourceManager.GetString(msgName), args);
		}

		public static Exception BuildBLLException(string msgName, params object[] args)
		{
			return BuildException(msgName, BusinessLogicLayerResource.ResourceManager.GetString(msgName), args);
		}

		private static Exception BuildException(string msgName, string msg, params object[] args)
		{
			var argsNew = args;
			if (args != null && args.Length > 0)
			{
				argsNew = new object[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i] == null)
					{
						argsNew[i] = String.Empty;
					}
					else if (args[i] is IEnumerable && args[i].GetType() != typeof(string))
					{
						List<string> list = new List<string>(GlobalConsts.MAX_UI_ERROR_LIST + 1);
						foreach (object obj in args[i] as IEnumerable)
						{
							if (list.Count >= GlobalConsts.MAX_UI_ERROR_LIST)
							{
								list.Add(GlobalConsts.DOTDOTDOT + Environment.NewLine + UtilsResource.REFER_TO_EVENT_LOG);
								break;
							}
							list.Add(obj == null ? String.Empty : obj.ToString());
						}

						argsNew[i] = String.Join(",", list);
					}
					else
					{
						argsNew[i] = args[i].ToString();
					}
				}
			}
			var ex = new Exception(msg.FormatStr(argsNew));
			ex.Data.Add(GlobalConsts.EXCEPTION_MESSAGE_NAME_KEY, msgName);

			return ex;
		}
	}
}
