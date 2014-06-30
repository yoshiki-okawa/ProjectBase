using PB.Frameworks.Common.Extensions;
using PB.Frameworks.Common.General;
using PB.Frameworks.Common.General.Exceptions;
using PB.Frameworks.Types.Interfaces;
using PB.Frameworks.Utils.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PB.Frameworks.Utils.Cache
{
	public static class GlobalCache<T> where T : ICacheable, new()
	{
		public static IList<T> GetCache()
		{
			return (IList<T>)GlobalCache.GetCache(typeof(T));
		}

		public static void SetCache(IList<T> values)
		{
			GlobalCache.SetCache(typeof(T), (IList<object>)values);
		}

		public static void Clear()
		{
			GlobalCache.Clear(typeof(T));
		}
		public static void ClearAll()
		{
			GlobalCache.ClearAll();
		}
	}

	public static class GlobalCache
	{
		private static Dictionary<Type, IList<object>> cache = new Dictionary<Type, IList<object>>();
		private static Dictionary<string, IList<object>> cache2 = new Dictionary<string, IList<object>>();
		public static IList<object> GetCache(Type t)
		{
			if (!typeof(ICacheable).IsAssignableFrom(t))
				ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildUtilsException("NOT_SUPPORTED_CACHE", t.FullName), EventLogEntryType.Error);

			IList<object> results = null;
			lock (cache)
			{
				cache.TryGetValue(t, out results);
			}
			if (results == null)
			{
				object obj = GlobalUtils.CreateInstance(t);
				results = ((ICacheable)obj).LoadAll();
				SetCache(t, results);
			}
			return results == null ? null : results.DeepClone();
		}

		public static void SetCache(Type t, IList<object> values)
		{
			if (!typeof(ICacheable).IsAssignableFrom(t))
				ExceptionLogger.LogAndThrowException(ExceptionHelper.BuildUtilsException("NOT_SUPPORTED_CACHE", t.FullName), EventLogEntryType.Error);

			lock (cache)
			{
				cache[t] = values;
			}
		}

		public static void ClearAll()
		{
			lock (cache)
			{
				cache.Clear();
			}
		}

		public static void Clear(Type t)
		{
			lock (cache)
			{
				if (cache.ContainsKey(t))
					cache.Remove(t);
			}
		}
	}
}
