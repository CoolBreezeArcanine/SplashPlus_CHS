using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameRankingResponseVO : VOSerializer
	{
		public long type;

		public GameRanking[] gameRankingList;
	}
}
