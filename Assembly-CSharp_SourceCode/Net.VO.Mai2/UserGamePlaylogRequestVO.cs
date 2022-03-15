using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserGamePlaylogRequestVO : VOSerializer
	{
		public ulong userId;

		public UserGamePlaylog userChargelog;
	}
}
