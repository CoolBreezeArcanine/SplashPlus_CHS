using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameTournamentInfoResponseVO : VOSerializer
	{
		public long length;

		public GameTournamentInfo[] gameTournamentInfoList;
	}
}
