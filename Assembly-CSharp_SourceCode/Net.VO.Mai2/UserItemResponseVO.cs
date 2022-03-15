using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserItemResponseVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public int itemKind;

		public UserItem[] userItemList;
	}
}
