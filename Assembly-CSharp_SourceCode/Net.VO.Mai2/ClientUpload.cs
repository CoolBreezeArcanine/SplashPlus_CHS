using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct ClientUpload
	{
		public int orderId;

		public int divNumber;

		public int divLength;

		public string divData;

		public int placeId;

		public string clientId;

		public string uploadDate;

		public string fileName;
	}
}
