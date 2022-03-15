using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameChargeResponseVO : VOSerializer
	{
		public long length;

		public GameCharge[] gameChargeList;
	}
}
