using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserOption : Packet
	{
		private readonly Action<UserOption> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserOption(ulong userId, Action<UserOption> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserOptionRequestVO, UserOptionResponseVO> netQuery = new NetQuery<UserOptionRequestVO, UserOptionResponseVO>("GetUserOptionApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserOptionRequestVO, UserOptionResponseVO> netQuery = base.Query as NetQuery<UserOptionRequestVO, UserOptionResponseVO>;
				_onDone(netQuery.Response.userOption);
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
