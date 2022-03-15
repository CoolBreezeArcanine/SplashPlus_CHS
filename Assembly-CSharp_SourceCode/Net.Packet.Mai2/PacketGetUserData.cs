using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserData : Packet
	{
		private readonly Action<UserDetail> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserData(ulong userId, Action<UserDetail> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserDetailRequestVO, UserDetailResponseVO> netQuery = new NetQuery<UserDetailRequestVO, UserDetailResponseVO>("GetUserDataApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserDetailRequestVO, UserDetailResponseVO> netQuery = base.Query as NetQuery<UserDetailRequestVO, UserDetailResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userData);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserDetailResponseVO src)
		{
			if (src.userData.charaSlot == null)
			{
				src.userData.charaSlot = new int[5];
			}
		}
	}
}
