using PB.Frameworks.Common.General;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.Interfaces;

namespace PB.Frameworks.Types.DatabaseEntities
{
	[DbTable(TableName = "user_action")]
	public class DEUserAction : DbEntity, IDEUserAction
	{
		[DbField(FieldName = "user_id")]
		public int UserId { get; set; }

		[DbField(FieldName = "type")]
		public GlobalEnums.UserActionType Type { get; set; }

		[DbField(FieldName = "verification_code")]
		public string VerificationCode { get; set; }
	}
}
