using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserPortrait : Packet
	{
		private readonly Action<UserPortrait[], byte[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserPortrait(ulong userId, Action<UserPortrait[], byte[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<GetUserPortraitRequestVO, GetUserPortraitResponseVO> netQuery = new NetQuery<GetUserPortraitRequestVO, GetUserPortraitResponseVO>("GetUserPortraitApi", 0uL);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<GetUserPortraitRequestVO, GetUserPortraitResponseVO> netQuery = base.Query as NetQuery<GetUserPortraitRequestVO, GetUserPortraitResponseVO>;
				_onDone(netQuery.Response.userPortraitList.ToArray(), netQuery.Response.userPortraitList.SelectMany((UserPortrait i) => Base64ToBytes(i.divData)).ToArray());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static IEnumerable<byte> Base64ToBytes(string base64)
		{
			return Convert.FromBase64String(base64);
		}
	}
}
