using System;
using System.Collections.Generic;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserCharge : Packet
	{
		private readonly Action<UserCharge[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserChargeResponseVO> _responseVOs;

		public PacketGetUserCharge(ulong userId, Action<UserCharge[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserChargeRequestVO, UserChargeResponseVO> netQuery = new NetQuery<UserChargeRequestVO, UserChargeResponseVO>("GetUserChargeApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserChargeRequestVO, UserChargeResponseVO> netQuery = base.Query as NetQuery<UserChargeRequestVO, UserChargeResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userChargeList);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserChargeResponseVO src)
		{
			if (src.userChargeList == null)
			{
				src.userChargeList = Array.Empty<UserCharge>();
			}
		}
	}
}
