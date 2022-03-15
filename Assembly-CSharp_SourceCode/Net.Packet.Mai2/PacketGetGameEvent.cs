using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetGameEvent : Packet
	{
		private readonly Action<GameEvent[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetGameEvent(Action<GameEvent[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GameEventRequestVO, GameEventResponseVO> netQuery = new NetQuery<GameEventRequestVO, GameEventResponseVO>("GetGameEventApi", 0uL);
			netQuery.Request.type = 1;
			netQuery.Request.isAllEvent = false;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GameEventRequestVO, GameEventResponseVO> netQuery = base.Query as NetQuery<GameEventRequestVO, GameEventResponseVO>;
				_onDone(netQuery.Response.gameEventList ?? Array.Empty<GameEvent>());
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
