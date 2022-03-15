using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserExtendResponseVO : VOSerializer
	{
		public ulong userId;

		public UserExtend userExtend;
	}
}
