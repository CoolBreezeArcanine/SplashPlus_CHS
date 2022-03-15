using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class TransferRequestVO : VOSerializer
	{
		public ulong userId;

		public UserDetail[] userData;

		public UserMusicDetail[] userMusicDetailList;

		public string isNewMusicDetailList;
	}
}
