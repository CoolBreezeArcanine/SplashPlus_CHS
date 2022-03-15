using System;
using Net.VO.Mai2;

namespace Manager.Operation
{
	public class OperationData
	{
		private const string ServerUrn = "Maimai2Servlet/";

		private string _serverUri;

		public bool IsDumpUpload;

		public bool IsAou;

		public int RebootInterval;

		public GameSetting GameSetting;

		public GameRanking[] GameRankings = Array.Empty<GameRanking>();

		public GameEvent[] GameEvents = Array.Empty<GameEvent>();

		public GameTournamentInfo[] GameTournamentInfos = Array.Empty<GameTournamentInfo>();

		public GameCharge[] GameCharges = Array.Empty<GameCharge>();

		public bool IsUpdate;

		public string ServerUri
		{
			get
			{
				return _serverUri;
			}
			set
			{
				_serverUri = value + "Maimai2Servlet/";
			}
		}
	}
}
