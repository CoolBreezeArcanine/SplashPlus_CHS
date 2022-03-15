using System.Collections.Generic;

namespace Manager.UserDatas
{
	public class UserUpdate
	{
		public List<UserScore>[] UpdateScoreList { get; set; } = new List<UserScore>[6];


		public List<UserScore> UpdateScoreStrongList { get; set; } = new List<UserScore>();


		public List<UserScore> UpdateClassicScoreList { get; set; } = new List<UserScore>();


		public List<int> UpdateIconList { get; set; } = new List<int>();


		public List<int> UpdatePlateList { get; set; } = new List<int>();


		public List<int> UpdateTitleList { get; set; } = new List<int>();


		public List<int> UpdateMusicUnlockList { get; set; } = new List<int>();


		public List<int> UpdateMusicUnlockMasterList { get; set; } = new List<int>();


		public List<int> UpdateMusicUnlockReMasterList { get; set; } = new List<int>();


		public List<int> MusicUnlockStrongList { get; set; } = new List<int>();


		public List<UserMapData> UpdateMapList { get; set; } = new List<UserMapData>();


		public List<UserChara> UpdateCharaList { get; set; } = new List<UserChara>();


		public UserUpdate()
		{
			for (int i = 0; i < UpdateScoreList.Length; i++)
			{
				UpdateScoreList[i] = new List<UserScore>();
			}
		}

		public void Clear()
		{
			UpdateMusicUnlockList.Clear();
			UpdateMusicUnlockMasterList.Clear();
			UpdateMusicUnlockReMasterList.Clear();
			MusicUnlockStrongList.Clear();
			UpdateIconList.Clear();
			UpdatePlateList.Clear();
			UpdateTitleList.Clear();
			UpdateMapList.Clear();
			UpdateCharaList.Clear();
			for (int i = 0; i < UpdateScoreList.Length; i++)
			{
				UpdateScoreList[i].Clear();
			}
			UpdateScoreStrongList.Clear();
			UpdateClassicScoreList.Clear();
		}
	}
}
