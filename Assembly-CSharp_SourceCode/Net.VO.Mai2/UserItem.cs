using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserItem
	{
		public int itemKind;

		public int itemId;

		public int stock;

		public bool isValid;
	}
}
