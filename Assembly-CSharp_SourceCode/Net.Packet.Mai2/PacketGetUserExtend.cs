using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserExtend : Packet
	{
		private readonly Action<UserExtend> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserExtend(ulong userId, Action<UserExtend> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserExtendRequestVO, UserExtendResponseVO> netQuery = new NetQuery<UserExtendRequestVO, UserExtendResponseVO>("GetUserExtendApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserExtendRequestVO, UserExtendResponseVO> netQuery = base.Query as NetQuery<UserExtendRequestVO, UserExtendResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userExtend);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserExtendResponseVO src)
		{
			if (src.userExtend.selectedCardList == null)
			{
				src.userExtend.selectedCardList = Array.Empty<int>();
			}
			if (src.userExtend.encountMapNpcList == null)
			{
				src.userExtend.encountMapNpcList = Array.Empty<MapEncountNpc>();
			}
		}
	}
}
