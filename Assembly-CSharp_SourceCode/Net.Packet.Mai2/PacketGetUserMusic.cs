using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserMusic : Packet
	{
		private readonly Action<UserMusicDetail[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly List<UserMusicResponseVO> _responseVOs;

		public PacketGetUserMusic(ulong userId, Action<UserMusicDetail[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_responseVOs = new List<UserMusicResponseVO>();
			Create(GetNetQuery(userId, 0));
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserMusicRequestVO, UserMusicResponseVO> netQuery = base.Query as NetQuery<UserMusicRequestVO, UserMusicResponseVO>;
				SafeNullMember(netQuery.Response);
				_responseVOs.Add(netQuery.Response);
				if (netQuery.Response.nextIndex != 0)
				{
					Create(GetNetQuery(netQuery.Response.userId, netQuery.Response.nextIndex));
					break;
				}
				_onDone(_responseVOs.SelectMany((UserMusicResponseVO i) => i.userMusicList.SelectMany((UserMusic j) => j.userMusicDetailList)).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private NetQuery<UserMusicRequestVO, UserMusicResponseVO> GetNetQuery(ulong userId, int nextIndex)
		{
			NetQuery<UserMusicRequestVO, UserMusicResponseVO> netQuery = new NetQuery<UserMusicRequestVO, UserMusicResponseVO>("GetUserMusicApi", userId);
			netQuery.Request.userId = userId;
			netQuery.Request.nextIndex = nextIndex;
			return netQuery;
		}

		private static void SafeNullMember(UserMusicResponseVO src)
		{
			if (src.userMusicList == null)
			{
				src.userMusicList = Array.Empty<UserMusic>();
			}
			for (int i = 0; i < src.userMusicList.Length; i++)
			{
				if (src.userMusicList[i].userMusicDetailList == null)
				{
					src.userMusicList[i].userMusicDetailList = Array.Empty<UserMusicDetail>();
				}
			}
		}
	}
}
