using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserMap : Packet
	{
		private readonly Action<UserMap[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserMapResponseVO> _responseVOs;

		public PacketGetUserMap(ulong userId, Action<UserMap[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserMapResponseVO>();
			Create(GetNetQuery(userId, 0L));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserMapRequestVO, UserMapResponseVO> netQuery = base.Query as NetQuery<UserMapRequestVO, UserMapResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0L)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserMapResponseVO i) => i.userMapList).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserMapRequestVO, UserMapResponseVO> GetNetQuery(ulong userId, long nextIndex)
		{
			NetQuery<UserMapRequestVO, UserMapResponseVO> netQuery = new NetQuery<UserMapRequestVO, UserMapResponseVO>("GetUserMapApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserMapResponseVO src)
		{
			if (src.userMapList == null)
			{
				src.userMapList = Array.Empty<UserMap>();
			}
		}
	}
}
