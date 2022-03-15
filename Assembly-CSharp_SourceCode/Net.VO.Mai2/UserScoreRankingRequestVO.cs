using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserScoreRankingRequestVO : VOSerializer
	{
		public ulong userId;

		public int competitionId;
	}
}
