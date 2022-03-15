using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserMusicRequestVO : VOSerializer
	{
		public ulong userId;

		public int nextIndex;

		public int maxCount = 50;
	}
}
