using System.Collections.Generic;
using MAI2.Util;
using Manager.MaiStudio;
using Net.VO.Mai2;

namespace Manager
{
	public class MusicRankingManager : Singleton<MusicRankingManager>
	{
		public struct MusicRankingSt
		{
			public bool IsNew;

			public int MusicId;

			public MusicRankingSt(int musicId, bool isNew)
			{
				IsNew = isNew;
				MusicId = musicId;
			}
		}

		private const int RankingMaxNum = 50;

		private const int AdvertiseMaxNum = 20;

		private List<MusicRankingSt> enableRankings = new List<MusicRankingSt>();

		private int PlayingDemoIndex;

		public List<MusicRankingSt> Rankings => enableRankings;

		public int GetRankingMaxNum()
		{
			return 50;
		}

		public int GetPlayDemoIndex()
		{
			if (PlayingDemoIndex >= 20)
			{
				Singleton<MusicRankingManager>.Instance.PlayingDemoIndex = 0;
			}
			if (PlayingDemoIndex >= enableRankings.Count)
			{
				Singleton<MusicRankingManager>.Instance.PlayingDemoIndex = 0;
			}
			return PlayingDemoIndex;
		}

		public void AddDemoIndex()
		{
			PlayingDemoIndex++;
		}

		public void Initialize()
		{
			TimeManager.MarkGameStartTime();
			Singleton<EventManager>.Instance.UpdateEvent();
			GameRanking[] array = Singleton<OperationManager>.Instance.GetMusicRankingList();
			enableRankings.Clear();
			if (array == null || array.Length == 0 || array[0].id < 0)
			{
				array = Singleton<DataManager>.Instance.GetMusicOffRankings();
			}
			for (int i = 0; i < 50 && i < array.Length; i++)
			{
				if (array[i].id > 0)
				{
					MusicData music = Singleton<DataManager>.Instance.GetMusic((int)array[i].id);
					if (music != null && Singleton<EventManager>.Instance.IsOpenEvent(music.eventName.id))
					{
						enableRankings.Add(new MusicRankingSt(music.name.id, Singleton<EventManager>.Instance.IsNewEvent(music.eventName.id)));
					}
				}
			}
			if (enableRankings.Count != 0)
			{
				return;
			}
			array = Singleton<DataManager>.Instance.GetMusicOffRankings();
			for (int j = 0; j < 50 && j < array.Length; j++)
			{
				if (array[j].id > 0)
				{
					MusicData music2 = Singleton<DataManager>.Instance.GetMusic((int)array[j].id);
					if (music2 != null && Singleton<EventManager>.Instance.IsOpenEvent(music2.eventName.id))
					{
						enableRankings.Add(new MusicRankingSt(music2.name.id, Singleton<EventManager>.Instance.IsNewEvent(music2.eventName.id)));
					}
				}
			}
		}
	}
}
