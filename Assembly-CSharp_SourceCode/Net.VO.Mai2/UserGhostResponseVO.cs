using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserGhostResponseVO : VOSerializer
	{
		public ulong userId;

		public UserGhost[] userGhostList;
	}
}
