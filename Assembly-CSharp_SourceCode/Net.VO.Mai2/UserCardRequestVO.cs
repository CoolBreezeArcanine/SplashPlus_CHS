using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserCardRequestVO : VOSerializer
	{
		public ulong userId;

		public int nextIndex;

		public int maxCount = 20;
	}
}
