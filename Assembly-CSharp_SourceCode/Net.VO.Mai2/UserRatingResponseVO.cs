using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserRatingResponseVO : VOSerializer
	{
		public ulong userId;

		public UserRating userRating;
	}
}
