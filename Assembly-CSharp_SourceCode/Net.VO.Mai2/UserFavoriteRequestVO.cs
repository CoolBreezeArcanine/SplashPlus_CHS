using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserFavoriteRequestVO : VOSerializer
	{
		public ulong userId;

		public int itemKind;
	}
}
