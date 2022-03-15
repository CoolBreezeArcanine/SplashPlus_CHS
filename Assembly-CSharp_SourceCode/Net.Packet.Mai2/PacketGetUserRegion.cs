using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserRegion : Packet
	{
		private readonly Action<UserRegion[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserRegion(ulong userId, Action<UserRegion[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserRegionRequestVO, UserRegionResponseVO> netQuery = new NetQuery<UserRegionRequestVO, UserRegionResponseVO>("GetUserRegionApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserRegionRequestVO, UserRegionResponseVO> netQuery = base.Query as NetQuery<UserRegionRequestVO, UserRegionResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userRegionList);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserRegionResponseVO src)
		{
			if (src.userRegionList == null)
			{
				src.userRegionList = Array.Empty<UserRegion>();
			}
		}
	}
}
