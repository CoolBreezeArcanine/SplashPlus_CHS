using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserAllRequestVO : VOSerializer
	{
		public ulong userId;

		public ulong playlogId;

		public bool isEventMode;

		public bool isFreePlay;

		public UserAll upsertUserAll;
	}
}
