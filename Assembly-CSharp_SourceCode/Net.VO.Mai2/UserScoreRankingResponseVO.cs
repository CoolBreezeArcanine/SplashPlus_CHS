using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserScoreRankingResponseVO : VOSerializer
	{
		public ulong userId;

		public UserScoreRanking userScoreRanking;
	}
}
