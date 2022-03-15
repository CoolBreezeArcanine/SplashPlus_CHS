using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserGhost : Packet
	{
		private readonly Action<UserGhost[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserGhost(ulong userId, Action<UserGhost[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserGhostRequestVO, UserGhostResponseVO> netQuery = new NetQuery<UserGhostRequestVO, UserGhostResponseVO>("GetUserGhostApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserGhostRequestVO, UserGhostResponseVO> netQuery = base.Query as NetQuery<UserGhostRequestVO, UserGhostResponseVO>;
				SafeNullMember(netQuery.Response);
				_onDone(netQuery.Response.userGhostList);
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private static void SafeNullMember(UserGhostResponseVO src)
		{
			if (src.userGhostList == null)
			{
				src.userGhostList = Array.Empty<UserGhost>();
				return;
			}
			for (int i = 0; i < src.userGhostList.Length; i++)
			{
				if (src.userGhostList[i].resultBitList == null)
				{
					src.userGhostList[i].resultBitList = Array.Empty<byte>();
				}
			}
		}
	}
}
