using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserDetailResponseVO : VOSerializer
	{
		public ulong userId;

		public UserDetail userData;
	}
}
