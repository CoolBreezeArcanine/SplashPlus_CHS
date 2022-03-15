using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserGamePlaylog
	{
		public ulong playlogId;

		public string version;

		public string playDate;

		public int playMode;

		public int useTicketId;

		public int playCredit;

		public int playTrack;

		public string clientId;

		public bool isPlayTutorial;

		public bool isEventMode;

		public bool isNewFree;

		public int playCount;

		public int playSpecial;
	}
}
