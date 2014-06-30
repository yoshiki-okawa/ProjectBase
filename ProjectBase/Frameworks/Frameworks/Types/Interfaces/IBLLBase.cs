using PB.Frameworks.Types.General;
using System;

namespace PB.Frameworks.Types.Interfaces
{
	public interface IBLLBase
	{
		/// <summary>
		/// Unique name of this instance displayed in UI. Default value is full name of type.
		/// </summary>
		string Name { get; set; }

		event EventHandler<ProgressChangedEventArgs> ProgressChanged;
		void Execute();
		void Stop();
		Tuple<string, double, string> GetCurrentProgress();
	}
}
