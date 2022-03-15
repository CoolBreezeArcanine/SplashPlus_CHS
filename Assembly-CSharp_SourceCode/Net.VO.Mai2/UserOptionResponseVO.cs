using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserOptionResponseVO : VOSerializer
	{
		public ulong userId;

		public UserOption userOption;
	}
}
