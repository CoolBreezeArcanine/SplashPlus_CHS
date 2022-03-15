using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct ClientSetting
	{
		public int placeId;

		public string clientId;

		public string placeName;

		public int regionId;

		public string regionName;

		public string bordId;

		public int romVersion;

		public bool isDevelop;

		public bool isAou;
	}
}
