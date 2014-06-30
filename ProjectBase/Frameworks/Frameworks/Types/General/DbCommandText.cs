using System.Collections.Generic;

namespace PB.Frameworks.Types.General
{
	public class DbCommandText
	{
		public Dictionary<string, object> DataParameters { get; set; }
		public string CommandText { get; set; }

		/// <summary>
		/// Full or partial db command text with optional data parameters. Data parameters are added with key of @ + index in dataParameters parameter. i.e. @0, @1, etc.
		/// </summary>
		/// <param name="commandText"></param>
		/// <param name="dataParameters"></param>
		public DbCommandText(string commandText, params object[] dataParameters)
		{
			CommandText = commandText;
			DataParameters = new Dictionary<string, object>();
			for (int i = 0; i < dataParameters.Length; i++)
				DataParameters.Add("@" + i, dataParameters[i]);
		}

		public static implicit operator DbCommandText(string commandText)
		{
			return new DbCommandText(commandText);
		}
	}
}
