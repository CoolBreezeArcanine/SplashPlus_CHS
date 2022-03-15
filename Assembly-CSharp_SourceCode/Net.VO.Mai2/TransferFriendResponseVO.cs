using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class TransferFriendResponseVO : VOSerializer
	{
		public ulong userId;

		public TransferFriend[] transferFriendList;
	}
}
