using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetGameTournamentInfo : Packet
	{
		private readonly Action<GameTournamentInfo[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetGameTournamentInfo(Action<GameTournamentInfo[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GameTournamentInfoRequestVO, GameTournamentInfoResponseVO> query = new NetQuery<GameTournamentInfoRequestVO, GameTournamentInfoResponseVO>("GetGameTournamentInfoApi", 0uL);
			Create(query);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GameTournamentInfoRequestVO, GameTournamentInfoResponseVO> netQuery = base.Query as NetQuery<GameTournamentInfoRequestVO, GameTournamentInfoResponseVO>;
				_onDone(netQuery.Response.gameTournamentInfoList ?? Array.Empty<GameTournamentInfo>());
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
