using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserScoreRanking : Packet
	{
		private readonly Action<UserScoreRanking[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserScoreRankingResponseVO> _responseVOs;

		private readonly List<int> _rankingIds;

		private int _listIndex;

		private readonly int _listLength;

		public PacketGetUserScoreRanking(ulong userId, Action<UserScoreRanking[]> onDone, Action<PacketStatus> onError = null)
		{
			_listLength = Singleton<ScoreRankingManager>.Instance.ScoreRankings.Count;
			if (_listLength <= 0)
			{
				return;
			}
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserScoreRankingResponseVO>();
			_rankingIds = new List<int>();
			foreach (KeyValuePair<int, ScoreRankingForMusicSeq> scoreRanking in Singleton<ScoreRankingManager>.Instance.ScoreRankings)
			{
				_rankingIds.Add(scoreRanking.Key);
			}
			NetQuery<UserScoreRankingRequestVO, UserScoreRankingResponseVO> netQuery = new NetQuery<UserScoreRankingRequestVO, UserScoreRankingResponseVO>("GetUserScoreRankingApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.competitionId = _rankingIds[_listIndex++];
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserScoreRankingRequestVO, UserScoreRankingResponseVO> netQuery = base.Query as NetQuery<UserScoreRankingRequestVO, UserScoreRankingResponseVO>;
				_responseVOs.Add(netQuery.Response);
				if (_listIndex < _listLength)
				{
					netQuery.Request.competitionId = _rankingIds[_listIndex++];
					Create(netQuery);
				}
				else
				{
					_onDone(_responseVOs.Select((UserScoreRankingResponseVO i) => i.userScoreRanking).ToArray());
				}
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}
	}
}
