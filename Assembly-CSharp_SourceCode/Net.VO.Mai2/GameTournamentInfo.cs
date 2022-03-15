using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct GameTournamentInfo
	{
		public int tournamentId;

		public string tournamentName;

		public int rankingKind;

		public int scoreType;

		public string noticeStartDate;

		public string noticeEndDate;

		public string startDate;

		public string endDate;

		public string entryStartDate;

		public string entryEndDate;

		public GameTournamentMusic[] gameTournamentMusicList;
	}
}
