using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserCard : Packet
	{
		private readonly Action<UserCard[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserCardResponseVO> _responseVOs;

		public PacketGetUserCard(ulong userId, Action<UserCard[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserCardResponseVO>();
			Create(GetNetQuery(userId, 0));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserCardRequestVO, UserCardResponseVO> netQuery = base.Query as NetQuery<UserCardRequestVO, UserCardResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserCardResponseVO i) => i.userCardList).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserCardRequestVO, UserCardResponseVO> GetNetQuery(ulong userId, int nextIndex)
		{
			NetQuery<UserCardRequestVO, UserCardResponseVO> netQuery = new NetQuery<UserCardRequestVO, UserCardResponseVO>("GetUserCardApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserCardResponseVO src)
		{
			if (src.userCardList == null)
			{
				src.userCardList = Array.Empty<UserCard>();
			}
		}
	}
}
