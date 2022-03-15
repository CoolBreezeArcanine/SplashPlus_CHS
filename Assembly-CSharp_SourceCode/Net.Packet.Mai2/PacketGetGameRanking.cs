using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetGameRanking : Packet
	{
		private readonly Action<GameRanking[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetGameRanking(int type, Action<GameRanking[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GameRankingRequestVO, GameRankingResponseVO> netQuery = new NetQuery<GameRankingRequestVO, GameRankingResponseVO>("GetGameRankingApi", 0uL);
			netQuery.Request.type = type;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GameRankingRequestVO, GameRankingResponseVO> netQuery = base.Query as NetQuery<GameRankingRequestVO, GameRankingResponseVO>;
				_onDone(netQuery.Response.gameRankingList ?? Array.Empty<GameRanking>());
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
