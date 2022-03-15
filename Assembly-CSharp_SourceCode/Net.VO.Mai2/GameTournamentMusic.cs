using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct GameTournamentMusic
	{
		public int tournamentId;

		public int musicId;

		public int level;

		public bool isFirstLock;
	}
}
