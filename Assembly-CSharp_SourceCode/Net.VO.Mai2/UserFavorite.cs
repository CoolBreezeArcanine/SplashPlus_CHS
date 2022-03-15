using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserFavorite
	{
		public ulong userId;

		public int itemKind;

		public int[] itemIdList;
	}
}
