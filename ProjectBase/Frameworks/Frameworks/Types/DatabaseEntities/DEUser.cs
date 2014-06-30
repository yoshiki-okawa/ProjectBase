using PB.Frameworks.Common.General;
using PB.Frameworks.Common.General.Exceptions;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace PB.Frameworks.Types.DatabaseEntities
{
	[DbTable(TableName = "user")]
	public class DEUser : DbEntity, IDEUser
	{
		[DbField(FieldName = "name")]
		public string Name { get; set; }

		[DbField(FieldName = "email")]
		public string Email { get; set; }

		[DbField(FieldName = "password")]
		public string Password { get; set; }

		[DbField(FieldName = "is_active")]
		public bool IsActive { get; set; }

		public override void Validate(GlobalEnums.ActionType actionType, params object[] args)
		{
			base.Validate(actionType, args);

			if (String.IsNullOrEmpty(Name))
				throw ExceptionHelper.BuildTypesException("DEUSER_NO_NAME");

			if (String.IsNullOrEmpty(Email))
				throw ExceptionHelper.BuildTypesException("DEUSER_NO_EMAIL");

			if (!Regex.IsMatch(Name, "^[0-9a-zA-Z_\\.\\-]+$"))
				throw ExceptionHelper.BuildTypesException("DEUSER_INVALID_NAME");

			if (!GlobalUtils.IsValidEmail(Email))
				throw ExceptionHelper.BuildTypesException("DEUSER_INVALID_EMAIL");
		}
	}
}
