using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserFavorite : Packet
	{
		private readonly Action<UserFavorite> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserFavorite(ulong userId, int kind, Action<UserFavorite> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserFavoriteRequestVO, UserFavoriteResponseVO> netQuery = new NetQuery<UserFavoriteRequestVO, UserFavoriteResponseVO>("GetUserFavoriteApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.itemKind = kind;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserFavoriteRequestVO, UserFavoriteResponseVO> netQuery = base.Query as NetQuery<UserFavoriteRequestVO, UserFavoriteResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userFavoriteData);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserFavoriteResponseVO src)
		{
			if (src.userFavoriteData.itemIdList == null)
			{
				src.userFavoriteData.itemIdList = Array.Empty<int>();
			}
		}
	}
}
