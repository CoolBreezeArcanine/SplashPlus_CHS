using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketUserLogout : Packet
	{
		private readonly Action _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketUserLogout(ulong userId, Action onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserLogoutRequestVO, UserLogoutResponseVO> netQuery = new NetQuery<UserLogoutRequestVO, UserLogoutResponseVO>("UserLogoutApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
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
