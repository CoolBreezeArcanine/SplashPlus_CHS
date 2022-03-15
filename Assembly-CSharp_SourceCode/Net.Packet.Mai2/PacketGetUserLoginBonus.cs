using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserLoginBonus : Packet
	{
		private readonly Action<Net.VO.Mai2.UserLoginBonus[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserLoginBonusResponseVO> _responseVOs;

		public PacketGetUserLoginBonus(ulong userId, Action<Net.VO.Mai2.UserLoginBonus[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserLoginBonusResponseVO>();
			Create(GetNetQuery(userId, 0L));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserLoginBonusRequestVO, UserLoginBonusResponseVO> netQuery = base.Query as NetQuery<UserLoginBonusRequestVO, UserLoginBonusResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0L)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserLoginBonusResponseVO i) => i.userLoginBonusList).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserLoginBonusRequestVO, UserLoginBonusResponseVO> GetNetQuery(ulong userId, long nextIndex)
		{
			NetQuery<UserLoginBonusRequestVO, UserLoginBonusResponseVO> netQuery = new NetQuery<UserLoginBonusRequestVO, UserLoginBonusResponseVO>("GetUserLoginBonusApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserLoginBonusResponseVO src)
		{
			if (src.userLoginBonusList == null)
			{
				src.userLoginBonusList = Array.Empty<Net.VO.Mai2.UserLoginBonus>();
			}
		}
	}
}
