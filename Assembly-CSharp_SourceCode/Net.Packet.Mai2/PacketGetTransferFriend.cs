using System;
using Net.VO;
using Net.VO.Mai2;

namespace Net.Packet.Mai2
{
	public class PacketGetTransferFriend : Packet
	{
		private readonly Action<TransferFriend[]> _onDone;

		private readonly Action<PacketStatus> _onError;

		public PacketGetTransferFriend(ulong userId, Action<TransferFriend[]> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			NetQuery<TransferFriendRequestVO, TransferFriendResponseVO> netQuery = new NetQuery<TransferFriendRequestVO, TransferFriendResponseVO>("GetTransferFriendApi", userId);
			netQuery.Request.userId = userId;
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<TransferFriendRequestVO, TransferFriendResponseVO> netQuery = base.Query as NetQuery<TransferFriendRequestVO, TransferFriendResponseVO>;
				_onDone(netQuery.Response.transferFriendList ?? Array.Empty<TransferFriend>());
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
