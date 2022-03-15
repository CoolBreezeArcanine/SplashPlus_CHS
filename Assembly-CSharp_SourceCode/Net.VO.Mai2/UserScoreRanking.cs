using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserScoreRanking
	{
		public int tournamentId;

		public long totalScore;

		public int ranking;

		public string rankingDate;
	}
}
