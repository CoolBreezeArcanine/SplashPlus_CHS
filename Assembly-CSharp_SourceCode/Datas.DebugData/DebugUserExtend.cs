using System;
using System.Collections.Generic;
using DB;
using Manager.UserDatas;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugUserExtend
	{
		public int CategoryIndex;

		public int MusicIndex;

		public int ExtraFlag;

		public bool SelectResultDetails;

		public SortTabID SortCategorySetting;

		public SortMusicID SortMusicSetting;

		public List<MapEncountNpc> EncountMapNpcList = new List<MapEncountNpc>();

		public List<int> SelectedCardList = new List<int>(4);

		public UserExtend GetUserExtend()
		{
			UserExtend userExtend = new UserExtend();
			userExtend.Initialize();
			return userExtend;
		}
	}
}
