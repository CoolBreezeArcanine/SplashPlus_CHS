using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserCharacterResponseVO : VOSerializer
	{
		public ulong userId;

		public UserCharacter[] userCharacterList;
	}
}
