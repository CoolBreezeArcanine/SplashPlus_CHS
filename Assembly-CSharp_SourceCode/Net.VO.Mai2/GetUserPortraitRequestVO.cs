using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GetUserPortraitRequestVO : VOSerializer
	{
		public ulong userId;
	}
}
