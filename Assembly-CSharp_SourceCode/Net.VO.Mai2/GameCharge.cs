using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct GameCharge
	{
		public int orderId;

		public int chargeId;

		public int price;

		public string startDate;

		public string endDate;
	}
}
