using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserMapRequestVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public int maxCount = 20;
	}
}
