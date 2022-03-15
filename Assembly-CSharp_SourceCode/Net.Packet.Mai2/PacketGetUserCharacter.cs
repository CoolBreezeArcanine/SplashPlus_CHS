using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetUserCharacter : Packet
	{
		private readonly Action<UserCharacter[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetUserCharacter(ulong userId, Action<UserCharacter[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<UserCharacterRequestVO, UserCharacterResponseVO> netQuery = new NetQuery<UserCharacterRequestVO, UserCharacterResponseVO>("GetUserCharacterApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserCharacterRequestVO, UserCharacterResponseVO> netQuery = base.Query as NetQuery<UserCharacterRequestVO, UserCharacterResponseVO>;
				_onDone(netQuery.Response.userCharacterList ?? Array.Empty<UserCharacter>());
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}
	}
}
