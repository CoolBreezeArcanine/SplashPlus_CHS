using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct GameSetting
	{
		public bool isMaintenance;

		public int requestInterval;

		public string rebootStartTime;

		public string rebootEndTime;

		public int movieUploadLimit;

		public int movieStatus;

		public string movieServerUri;

		public string deliverServerUri;

		public string oldServerUri;

		public string usbDlServerUri;

		public int rebootInterval;
	}
}
