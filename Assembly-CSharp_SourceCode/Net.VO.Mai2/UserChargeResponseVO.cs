using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserChargeResponseVO : VOSerializer
	{
		public ulong userId;

		public long length;

		public UserCharge[] userChargeList;
	}
}
