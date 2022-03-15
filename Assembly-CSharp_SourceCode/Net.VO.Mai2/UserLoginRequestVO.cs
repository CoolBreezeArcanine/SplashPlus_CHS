using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserLoginRequestVO : VOSerializer
	{
		public ulong userId;

		public string accessCode;

		public int regionId;

		public int placeId;

		public string clientId;

		public long dateTime;
	}
}
