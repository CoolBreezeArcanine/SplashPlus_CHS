using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserPreviewRequestVO : VOSerializer
	{
		public ulong userId;

		public string segaIdAuthKey;
	}
}
