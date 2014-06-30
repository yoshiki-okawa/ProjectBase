using System;

namespace PB.Frameworks.Types.General
{
	public class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(double percent, string message)
		{
			Percent = percent;
			Message = message;
		}

		public double Percent { get; set; }
		public string Message { get; set; }
	}
}
