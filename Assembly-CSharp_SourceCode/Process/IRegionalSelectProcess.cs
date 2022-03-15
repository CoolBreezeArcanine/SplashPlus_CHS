using System.Collections.Generic;

namespace Process
{
	public interface IRegionalSelectProcess
	{
		List<UserMapData> GetUserMapDatas(int index);

		int SelectCursolIndex(int index);
	}
}
