using PB.Frameworks.Common.General;
using System;

namespace PB.Frameworks.Types.Interfaces
{
	public interface IDbEntity
	{
		int Id { get; set; }
		DateTime DateCreated { get; set; }
		DateTime DateModified { get; set; }
		void Validate(GlobalEnums.ActionType actionType, params object[] args);
	}
}
