using System;
using DB;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserExtend
	{
		public int selectMusicId;

		public int selectDifficultyId;

		public int categoryIndex;

		public int musicIndex;

		public int extraFlag;

		public int selectScoreType;

		public ulong extendContentBit;

		public bool isPhotoAgree;

		public bool isGotoCodeRead;

		public bool selectResultDetails;

		public SortTabID sortCategorySetting;

		public SortMusicID sortMusicSetting;

		public int[] selectedCardList;

		public MapEncountNpc[] encountMapNpcList;
	}
}
