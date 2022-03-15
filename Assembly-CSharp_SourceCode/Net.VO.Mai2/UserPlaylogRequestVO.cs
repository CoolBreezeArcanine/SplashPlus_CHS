using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserPlaylogRequestVO : VOSerializer
	{
		public ulong userId;

		public UserPlaylog userPlaylog;
	}
}
