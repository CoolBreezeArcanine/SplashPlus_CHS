using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserItemRequestVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public int maxCount = 100;
	}
}
