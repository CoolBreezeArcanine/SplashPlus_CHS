using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserLoginResponseVO : VOSerializer
	{
		public int returnCode;

		public string lastLoginDate;

		public int loginCount;

		public int consecutiveLoginCount;

		public ulong loginId;
	}
}
