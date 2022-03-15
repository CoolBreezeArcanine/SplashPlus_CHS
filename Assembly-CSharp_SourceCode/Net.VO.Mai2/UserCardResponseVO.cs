using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserCardResponseVO : VOSerializer
	{
		public ulong userId;

		public int nextIndex;

		public UserCard[] userCardList;
	}
}
