using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserActivity : Packet
	{
		private readonly Action<UserActivity> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserActivity(ulong userId, Action<UserActivity> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserActivityRequestVO, UserActivityResponseVO> netQuery = new NetQuery<UserActivityRequestVO, UserActivityResponseVO>("GetUserActivityApi", 0uL);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserActivityRequestVO, UserActivityResponseVO> netQuery = base.Query as NetQuery<UserActivityRequestVO, UserActivityResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userActivity);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserActivityResponseVO src)
		{
			if (src.userActivity.playList == null)
			{
				src.userActivity.playList = Array.Empty<UserAct>();
			}
			if (src.userActivity.musicList == null)
			{
				src.userActivity.musicList = Array.Empty<UserAct>();
			}
		}
	}
}
