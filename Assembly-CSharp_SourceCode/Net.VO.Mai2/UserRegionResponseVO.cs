using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserRegionResponseVO : VOSerializer
	{
		public ulong userId;

		public ulong length;

		public UserRegion[] userRegionList;
	}
}
