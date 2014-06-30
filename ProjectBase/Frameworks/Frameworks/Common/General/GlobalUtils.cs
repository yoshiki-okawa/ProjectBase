using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace PB.Frameworks.Common.General
{
	public static class GlobalUtils
	{
		private static Dictionary<Type, Delegate> constractors = new Dictionary<Type, Delegate>();
		public static T CreateInstance<T>()
		{
			return (T)CreateInstance(typeof(T));
		}

		public static object CreateInstance(Type t)
		{
			Delegate constractor = null;
			lock (constractors)
			{
				constractors.TryGetValue(t, out constractor);
			}
			if (constractor == null)
			{
				ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
				constractor = (Func<object>)Expression.Lambda(Expression.Convert(Expression.New(constructor), t)).Compile();
				constractors[t] = constractor;
			}
			return ((Func<object>)constractor)();
		}

		public static IEnumerable<FieldInfo> GetAllFields(Type t)
		{
			if (t == null)
				return Enumerable.Empty<FieldInfo>();

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
		}

		public static IEnumerable<PropertyInfo> GetAllProperties(Type t)
		{
			if (t == null)
				return Enumerable.Empty<PropertyInfo>();

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return t.GetProperties(flags).Concat(GetAllProperties(t.BaseType));
		}

		public static IEnumerable<MemberInfo> GetAllMembers(Type t)
		{
			return GetAllFields(t).Cast<MemberInfo>().Concat(GetAllProperties(t).Cast<MemberInfo>()).Distinct();
		}

		public static IPAddress GetLocalIPAddress()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return IPAddress.None;
			}

			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			return host
				 .AddressList
				 .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
		}

		public static string GetRandomBase64String()
		{
			return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
		}

		public static bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		public static void SendEmail(string to, string subject, string body)
		{
			SmtpClient client = new SmtpClient();
			MailMessage mail = new MailMessage(Config.Instance.FromEmail, to);
			mail.Subject = subject;
			mail.Body = body;
			mail.IsBodyHtml = true;
			client.Send(mail);
		}
	}
}
