using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserMusicResponseVO : VOSerializer
	{
		public ulong userId;

		public int nextIndex;

		public UserMusic[] userMusicList;
	}
}
