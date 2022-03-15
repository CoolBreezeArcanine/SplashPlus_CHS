using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserPhoto
	{
		public int orderId;

		public ulong userId;

		public int divNumber;

		public int divLength;

		public string divData;

		public int placeId;

		public string clientId;

		public string uploadDate;

		public ulong playlogId;

		public int trackNo;
	}
}
