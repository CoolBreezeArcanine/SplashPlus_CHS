using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketPing : Packet
	{
		private readonly Action _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketPing(Action onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<PingRequestVO, PingResponseVO> query = new NetQuery<PingRequestVO, PingResponseVO>("Ping", 0uL);
			Create(query);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
				_onDone();
				break;
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}
	}
}
