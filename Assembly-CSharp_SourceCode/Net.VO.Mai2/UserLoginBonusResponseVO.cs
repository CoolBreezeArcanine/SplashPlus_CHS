using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserLoginBonusResponseVO : VOSerializer
	{
		public ulong userId;

		public long nextIndex;

		public UserLoginBonus[] userLoginBonusList;
	}
}
