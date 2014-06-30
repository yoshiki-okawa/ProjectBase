using PB.Frameworks.Types.Attributes;
using PB.Frameworks.Types.General;
using System.Collections.Generic;

namespace PB.Frameworks.Types.Interfaces
{
	public interface IDbEntityDbInfo
	{
		DbTableAttribute DbTable { get; set; }
		List<DbEntityMemberInfo> DbEntityMemberInfos { get; set; }
	}
}
