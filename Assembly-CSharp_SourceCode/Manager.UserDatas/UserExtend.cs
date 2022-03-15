using System.Collections.Generic;
using System.Linq;
using DB;
using Manager.MaiStudio.CardTypeName;

namespace Manager.UserDatas
{
	public class UserExtend
	{
		public bool SelectResultDetails;

		public int SelectMusicID { get; set; }

		public int SelectDifficultyID { get; set; }

		public int CategoryIndex { get; set; }

		public int MusicIndex { get; set; }

		public int ExtraFlag { get; set; }

		public int SelectScoreType { get; set; }

		public UserExtendContentBit ExtendContendBit { get; set; } = new UserExtendContentBit();


		public SortTabID SortCategorySetting { get; set; }

		public SortMusicID SortMusicSetting { get; set; }

		public List<int> SelectedCardList { get; set; } = new List<int>();


		public List<MapEncountNpc> EncountMapNpcList { get; set; } = new List<MapEncountNpc>();


		public UserExtend()
		{
			Initialize();
		}

		public void Initialize()
		{
			SelectMusicID = 0;
			SelectDifficultyID = 0;
			CategoryIndex = 0;
			MusicIndex = 0;
			ExtraFlag = 0;
			SelectScoreType = 1;
			SelectResultDetails = false;
			SortCategorySetting = SortTabID.Genre;
			SortMusicSetting = SortMusicID.ID;
			SelectedCardList.Clear();
			EncountMapNpcList.Clear();
			SelectedCardList.Clear();
			ExtendContendBit.Clear();
		}

		public void TakeOverReset()
		{
			SelectMusicID = 0;
			SelectDifficultyID = 0;
			CategoryIndex = 0;
			MusicIndex = 0;
			ExtraFlag = 0;
			SelectScoreType = 1;
			SelectResultDetails = false;
			SortCategorySetting = SortTabID.Genre;
			SortMusicSetting = SortMusicID.ID;
			EncountMapNpcList.Clear();
			SelectedCardList.Clear();
		}

		public void AddEncountMapNpc(int mapNpcId, int mapNpcMusic)
		{
			MapEncountNpc mapEncountNpc = EncountMapNpcList.FirstOrDefault((MapEncountNpc a) => a.NpcId == mapNpcId);
			MapEncountNpc item = new MapEncountNpc(mapNpcId, mapNpcMusic);
			if (mapEncountNpc != null)
			{
				ClearEncountMapNpc(mapNpcId);
			}
			EncountMapNpcList.Insert(0, item);
			while (EncountMapNpcList.Count > 3)
			{
				EncountMapNpcList.RemoveAt(EncountMapNpcList.Count - 1);
			}
		}

		public void ClearEncountMapNpc(int mapNpcId)
		{
			EncountMapNpcList.RemoveAll((MapEncountNpc x) => x.NpcId == mapNpcId);
		}

		public void UpsertCardLog(int cardType)
		{
			SelectedCardList.RemoveAll((int x) => x == cardType);
			Table table = (Table)cardType;
			if ((uint)(table - 2) <= 2u || table == Table.FreedomPass)
			{
				SelectedCardList.Insert(0, cardType);
			}
			while (SelectedCardList.Count > 5)
			{
				SelectedCardList.RemoveAt(SelectedCardList.Count - 1);
			}
		}
	}
}
