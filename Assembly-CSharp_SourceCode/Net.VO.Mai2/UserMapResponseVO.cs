using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserMapResponseVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public UserMap[] userMapList;
	}
}
