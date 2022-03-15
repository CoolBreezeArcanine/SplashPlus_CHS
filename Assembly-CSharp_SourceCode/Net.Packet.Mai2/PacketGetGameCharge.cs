using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetGameCharge : Packet
	{
		private readonly Action<GameCharge[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetGameCharge(Action<GameCharge[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GameChargeRequestVO, GameChargeResponseVO> netQuery = new NetQuery<GameChargeRequestVO, GameChargeResponseVO>("GetGameChargeApi", 0uL);
			netQuery.Request.isAll = false;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GameChargeRequestVO, GameChargeResponseVO> netQuery = base.Query as NetQuery<GameChargeRequestVO, GameChargeResponseVO>;
				_onDone(netQuery.Response.gameChargeList ?? Array.Empty<GameCharge>());
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
