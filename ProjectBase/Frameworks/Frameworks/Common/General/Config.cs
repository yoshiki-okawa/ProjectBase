using System;
using System.Configuration;

namespace PB.Frameworks.Common.General
{
	public class Config
	{
		private static object locker = new object();
		private static Config instance = null;
		public static Config Instance { get { return instance ?? GetNewConfig(); } }
		private static Config GetNewConfig()
		{
			lock (locker)
			{
				Config.instance = new Config()
				{
					PasswordHashIterations = 10000,
					SessionInactiveLifetime = 15,
					CookieLifetime = 14,
					SessionInactiveCheckInterval = 5,
					GetDateFunction = "CURRENT_TIMESTAMP",
					RegistrationRequiresEmailVerification = false
				};
				foreach (string key in ConfigurationManager.AppSettings.AllKeys)
				{
					if (String.Equals(key, "DbType", StringComparison.InvariantCultureIgnoreCase))
					{
						GlobalEnums.DbType dbType;
						Enum.TryParse<GlobalEnums.DbType>(ConfigurationManager.AppSettings[key], true, out dbType);
						Config.instance.DbType = dbType;
					}
					else if (String.Equals(key, "ConnectionString", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.ConnectionString = ConfigurationManager.AppSettings[key];
					}
					else if (String.Equals(key, "PasswordHashIterations", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.PasswordHashIterations = Convert.ToInt32(ConfigurationManager.AppSettings[key]);
						if (Config.instance.PasswordHashIterations < 10000)
							Config.instance.PasswordHashIterations = 10000;
					}
					else if (String.Equals(key, "SessionInactiveLifetime", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.SessionInactiveLifetime = Convert.ToDouble(ConfigurationManager.AppSettings[key]);
					}
					else if (String.Equals(key, "SessionInactiveCheckInterval", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.SessionInactiveCheckInterval = Convert.ToDouble(ConfigurationManager.AppSettings[key]);
					}
					else if (String.Equals(key, "CookieLifetime", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.CookieLifetime = Convert.ToDouble(ConfigurationManager.AppSettings[key]);
					}
					else if (String.Equals(key, "BaseWebApiAddress", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.BaseWebApiAddress = ConfigurationManager.AppSettings[key].TrimEnd('/');
					}
					else if (String.Equals(key, "BaseWebUIAddress", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.BaseWebUIAddress = ConfigurationManager.AppSettings[key].TrimEnd('/');
					}
					else if (String.Equals(key, "GetDateFunction", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.GetDateFunction = ConfigurationManager.AppSettings[key];
					}
					else if (String.Equals(key, "FromEmail", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.FromEmail = ConfigurationManager.AppSettings[key];
					}
					else if (String.Equals(key, "RegistrationRequiresEmailVerification", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.RegistrationRequiresEmailVerification = Convert.ToBoolean(ConfigurationManager.AppSettings[key]);
					}
					else if (String.Equals(key, "WebsiteName", StringComparison.InvariantCultureIgnoreCase))
					{
						Config.instance.WebsiteName = ConfigurationManager.AppSettings[key];
					}
				}
			}
			return Config.instance;
		}

		public string ConnectionString { get; private set; }
		public GlobalEnums.DbType DbType { get; private set; }
		public int PasswordHashIterations { get; private set; }
		public double SessionInactiveLifetime { get; set; }
		public double SessionInactiveCheckInterval { get; set; }
		public double CookieLifetime { get; set; }
		public string BaseWebApiAddress { get; set; }
		public string BaseWebUIAddress { get; set; }
		public string GetDateFunction { get; set; }
		public string FromEmail { get; set; }
		public bool RegistrationRequiresEmailVerification { get; set; }
		public string WebsiteName { get; set; }
	}
}
