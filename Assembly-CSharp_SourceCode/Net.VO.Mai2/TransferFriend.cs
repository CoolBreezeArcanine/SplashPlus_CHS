using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct TransferFriend
	{
		public ulong playUserId;

		public string playUserName;

		public string playDate;

		public int friendPoint;

		public bool isFavorite;
	}
}
