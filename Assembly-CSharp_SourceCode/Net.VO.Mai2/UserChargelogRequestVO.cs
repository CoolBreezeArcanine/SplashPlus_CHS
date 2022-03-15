using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserChargelogRequestVO : VOSerializer
	{
		public ulong userId;

		public UserChargelog userChargelog;

		public UserCharge userCharge;
	}
}
