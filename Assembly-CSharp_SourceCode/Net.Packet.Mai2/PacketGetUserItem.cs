using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserItem : Packet
	{
		private readonly Action<UserItem[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserItemResponseVO> _responseVOs;

		public PacketGetUserItem(ulong userId, long itemId, Action<UserItem[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserItemResponseVO>();
			Create(GetNetQuery(userId, itemId * 10000000000L));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserItemRequestVO, UserItemResponseVO> netQuery = base.Query as NetQuery<UserItemRequestVO, UserItemResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0L)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserItemResponseVO i) => i.userItemList).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserItemRequestVO, UserItemResponseVO> GetNetQuery(ulong userId, long nextIndex)
		{
			NetQuery<UserItemRequestVO, UserItemResponseVO> netQuery = new NetQuery<UserItemRequestVO, UserItemResponseVO>("GetUserItemApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserItemResponseVO src)
		{
			if (src.userItemList == null)
			{
				src.userItemList = Array.Empty<UserItem>();
			}
		}
	}
}
