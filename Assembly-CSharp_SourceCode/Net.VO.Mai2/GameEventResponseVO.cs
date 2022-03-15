using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameEventResponseVO : VOSerializer
	{
		public int type;

		public GameEvent[] gameEventList;
	}
}
