using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserFavoriteResponseVO : VOSerializer
	{
		public ulong userId;

		public UserFavorite userFavoriteData;
	}
}
