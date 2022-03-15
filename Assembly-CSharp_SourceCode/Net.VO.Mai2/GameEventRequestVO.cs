using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameEventRequestVO : VOSerializer
	{
		public int type;

		public bool isAllEvent;
	}
}
