using PB.Frameworks.Types.Attributes;
using System.Diagnostics;

namespace PB.Frameworks.Types.DatabaseEntities
{
	[DbTable(TableName = "event_log")]
	public class DEEventLog : DbEntity
	{
		[DbField(FieldName = "entry_type")]
		public EventLogEntryType EntryType { get; set; }

		[DbField(FieldName = "description")]
		public string Description { get; set; }

		[DbField(FieldName = "user_id")]
		public int UserId { get; set; }

		[DbField(FieldName = "process_name")]
		public string ProcessName { get; set; }

		[DbField(FieldName = "source_machine_name")]
		public string SourceMachineName { get; set; }

		[DbField(FieldName = "process_machine_name")]
		public string ProcessMachineName { get; set; }

		[DbField(FieldName = "source_ip")]
		public string SourceIp { get; set; }

		[DbField(FieldName = "process_ip")]
		public string ProcessIp { get; set; }

		[DbField(FieldName = "managed_thread_id")]
		public int ManagedThreadId { get; set; }
	}
}
