using PB.Frameworks.Common.General;
using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.Interfaces;
using System;

namespace PB.Frameworks.Types.DatabaseEntities
{
	[Serializable]
	[DbTable(TableName = "entity")]
	public class DbEntity : IDbEntity
	{
		[DbField(FieldName = "id")]
		public int Id { get; set; }

		[DbField(FieldName = "date_created")]
		/// <summary>
		/// Date and time when this db entity was first created.
		/// </summary>
		public DateTime DateCreated { get; set; }

		[DbField(FieldName = "date_modified")]
		/// <summary>
		/// Date and time when this db entity was modified. This is used for preventing concurrent updates.
		/// http://en.wikipedia.org/wiki/Timestamp-based_concurrency_control
		/// </summary>
		public DateTime DateModified { get; set; }

		public virtual void Validate(GlobalEnums.ActionType actionType, params object[] args)
		{
		}
	}
}
