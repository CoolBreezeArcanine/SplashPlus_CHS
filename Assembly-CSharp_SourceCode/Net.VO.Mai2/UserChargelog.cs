using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserChargelog
	{
		public int chargeId;

		public int price;

		public string purchaseDate;

		public int playCount;

		public int playerRating;

		public int placeId;

		public int regionId;

		public string clientId;
	}
}
