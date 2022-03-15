using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.UserDatas;
using UnityEngine;

namespace Manager
{
	public class GhostManager : Singleton<GhostManager>
	{
		public enum GhostTarget
		{
			BossGhost_1P = 0,
			BossGhost_2P = 1,
			MapGhost1_1P = 2,
			MapGhost2_1P = 3,
			MapGhost3_1P = 4,
			VsGhost1_1P = 5,
			VsGhost2_1P = 6,
			VsGhost3_1P = 7,
			VsGhost4_1P = 8,
			VsGhost5_1P = 9,
			VsGhost6_1P = 10,
			MapGhost1_2P = 11,
			MapGhost2_2P = 12,
			MapGhost3_2P = 13,
			VsGhost1_2P = 14,
			VsGhost2_2P = 15,
			VsGhost3_2P = 16,
			VsGhost4_2P = 17,
			VsGhost5_2P = 18,
			VsGhost6_2P = 19,
			End = 20,
			START_1P = 2,
			START_2P = 11,
			END_1P = 10,
			END_2P = 19
		}

		public enum GhostType
		{
			Global = 0,
			Map = 1,
			Boss = 2,
			End = 3,
			Invalid = -1
		}

		public const int VsGhostDLMax = 10;

		public const int VsGhostDispMax = 6;

		private readonly List<UserGhost>[] _vsGhostList = new List<UserGhost>[2]
		{
			new List<UserGhost>(),
			new List<UserGhost>()
		};

		private readonly List<UserGhost>[] _vsGhostRawList = new List<UserGhost>[2]
		{
			new List<UserGhost>(),
			new List<UserGhost>()
		};

		private const int MapGhostMax = 3;

		private readonly List<UserGhost>[] _mapGhostList = new List<UserGhost>[2]
		{
			new List<UserGhost>(),
			new List<UserGhost>()
		};

		private readonly UserGhost[] _bossGhost = new UserGhost[2]
		{
			new UserGhost(),
			new UserGhost()
		};

		public List<UserGhost> GetVsGhostList(int monitorIndex)
		{
			return _vsGhostList[monitorIndex];
		}

		public List<UserGhost> GetMapGhostList(int monitorIndex)
		{
			return _mapGhostList[monitorIndex];
		}

		public UserGhost GetBossGhostList(int monitorIndex)
		{
			return _bossGhost[monitorIndex];
		}

		public UserGhost GetGhostToEnum(GhostTarget target)
		{
			return target switch
			{
				GhostTarget.BossGhost_1P => _bossGhost[0], 
				GhostTarget.BossGhost_2P => _bossGhost[1], 
				GhostTarget.MapGhost1_1P => _mapGhostList[0][0], 
				GhostTarget.MapGhost2_1P => _mapGhostList[0][1], 
				GhostTarget.MapGhost3_1P => _mapGhostList[0][2], 
				GhostTarget.MapGhost1_2P => _mapGhostList[1][0], 
				GhostTarget.MapGhost2_2P => _mapGhostList[1][1], 
				GhostTarget.MapGhost3_2P => _mapGhostList[1][2], 
				GhostTarget.VsGhost1_1P => _vsGhostList[0][0], 
				GhostTarget.VsGhost2_1P => _vsGhostList[0][1], 
				GhostTarget.VsGhost3_1P => _vsGhostList[0][2], 
				GhostTarget.VsGhost4_1P => _vsGhostList[0][3], 
				GhostTarget.VsGhost5_1P => _vsGhostList[0][4], 
				GhostTarget.VsGhost6_1P => _vsGhostList[0][5], 
				GhostTarget.VsGhost1_2P => _vsGhostList[1][0], 
				GhostTarget.VsGhost2_2P => _vsGhostList[1][1], 
				GhostTarget.VsGhost3_2P => _vsGhostList[1][2], 
				GhostTarget.VsGhost4_2P => _vsGhostList[1][3], 
				GhostTarget.VsGhost5_2P => _vsGhostList[1][4], 
				GhostTarget.VsGhost6_2P => _vsGhostList[1][5], 
				_ => null, 
			};
		}

		public static bool IsYourGhost(GhostTarget target, int index)
		{
			if (index == 0)
			{
				if (target == GhostTarget.BossGhost_1P || (uint)(target - 2) <= 8u)
				{
					return true;
				}
			}
			else if (1 == index && (target == GhostTarget.BossGhost_2P || (uint)(target - 11) <= 8u))
			{
				return true;
			}
			return false;
		}

		public void VsGhostUsed(GhostTarget target, int monitorIndex)
		{
			UserGhost ghostToEnum = GetGhostToEnum(target);
			_vsGhostRawList[monitorIndex].Remove(ghostToEnum);
		}

		public void UpdateGhost()
		{
			UpdateVsGhost();
			UpdateMapGhost();
			UpdateBossGhost();
		}

		public void ResetGhostServerData(int monitorIndex)
		{
			_vsGhostRawList[monitorIndex].Clear();
		}

		public void ResetGhostServerData()
		{
			for (int i = 0; i < _vsGhostRawList.Length; i++)
			{
				ResetGhostServerData(i);
			}
		}

		public void AddGhostData(int monitorIndex, UserGhost data)
		{
			_vsGhostRawList[monitorIndex].Add(data);
		}

		public int GetGhostCount(int monitorIndex)
		{
			return _vsGhostRawList[monitorIndex].Count;
		}

		public void GhostRandomSet()
		{
			for (int i = 0; i < _vsGhostRawList.Length; i++)
			{
				List<UserGhost> list = new List<UserGhost>();
				while (_vsGhostRawList[i].Count != 0)
				{
					int index = Random.Range(0, _vsGhostRawList[i].Count);
					UserGhost item = _vsGhostRawList[i][index];
					list.Add(item);
					_vsGhostRawList[i].RemoveAt(index);
				}
				_vsGhostRawList[i].Clear();
				foreach (UserGhost item2 in list)
				{
					_vsGhostRawList[i].Add(item2);
				}
			}
		}

		private void UpdateVsGhost()
		{
			for (int i = 0; i < 2; i++)
			{
				List<UserGhost> list = _vsGhostList[i];
				list.Clear();
				if (!Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry || Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
				{
					continue;
				}
				UserGhost[] array = _vsGhostRawList[i].Where((UserGhost x) => !x.IsUsed).ToArray();
				int num = ((array.Length < 10) ? array.Length : 10);
				for (int j = 0; j < array.Length; j++)
				{
					list.Add(array[(j + GameManager.MusicTrackNumber - 1) % num]);
					if (list.Count >= 6)
					{
						break;
					}
				}
			}
		}

		private void UpdateMapGhost()
		{
			for (int i = 0; i < 2; i++)
			{
				List<UserGhost> list = _mapGhostList[i];
				list.Clear();
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				UserExtend extend = userData.Extend;
				if (!userData.IsEntry || Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
				{
					continue;
				}
				foreach (MapEncountNpc encountMapNpc in extend.EncountMapNpcList)
				{
					MapOtomodachiData mapOtomodachiData = Singleton<DataManager>.Instance.GetMapOtomodachiData(encountMapNpc.NpcId);
					if (mapOtomodachiData != null)
					{
						int npcId = encountMapNpc.NpcId;
						int musicId = encountMapNpc.MusicId;
						int classValue = ((mapOtomodachiData.Rate < 0) ? userData.RatingList.Udemae.ClassValue : mapOtomodachiData.Rate);
						int npcParamBoss = UserUdemae.GetRateToUdemaeID(classValue).GetNpcParamBoss();
						UdemaeBossData udemaeBoss = Singleton<DataManager>.Instance.GetUdemaeBoss(npcParamBoss);
						int id = udemaeBoss.difficulty.id;
						int achieve = udemaeBoss.achieve;
						int achieveUnder = udemaeBoss.achieveUnder;
						int max = achieve;
						int min = achieveUnder;
						MusicDifficultyID musicDifficultyID = (MusicDifficultyID)id;
						if (musicDifficultyID == MusicDifficultyID.ReMaster)
						{
							MusicData music = Singleton<DataManager>.Instance.GetMusic(musicId);
							if (music == null || !music.notesData[4].isEnable || !Singleton<EventManager>.Instance.IsOpenEvent(music.subEventName.id))
							{
								musicDifficultyID = MusicDifficultyID.Master;
							}
						}
						int num = Random.Range(min, max);
						list.Add(new UserGhost
						{
							Id = (ulong)npcId,
							Type = UserGhost.GhostType.MapNpc,
							Name = mapOtomodachiData.name.str,
							IconId = mapOtomodachiData.Silhouette.id,
							TitleId = mapOtomodachiData.Title.id,
							PlateId = 1,
							Rate = 0,
							MusicId = musicId,
							Difficulty = (int)musicDifficultyID,
							Achievement = num,
							Rank = GetRank(num),
							CourseRank = (uint)udemaeBoss.daniId.id,
							ClassValue = classValue,
							ClassRank = (uint)UserUdemae.GetRateToUdemaeID(classValue)
						});
					}
					if (list.Count >= 3)
					{
						break;
					}
				}
			}
		}

		private void UpdateBossGhost()
		{
			for (int i = 0; i < 2; i++)
			{
				UserGhost userGhost = _bossGhost[i];
				userGhost.Clear();
				if (!Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry || Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
				{
					continue;
				}
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				bool flag = userData.RatingList.Udemae.IsBossBattleEnable();
				int classValue = userData.RatingList.Udemae.ClassValue;
				if (flag)
				{
					UdemaeBossData bossData = userData.RatingList.Udemae.GetBossData();
					if (bossData != null)
					{
						userGhost.Id = (ulong)bossData.GetID();
						userGhost.Type = UserGhost.GhostType.Boss;
						userGhost.Name = bossData.notesDesigner.str;
						userGhost.IconId = bossData.silhouette.id;
						userGhost.TitleId = 11;
						userGhost.PlateId = 11;
						userGhost.Rate = bossData.rating;
						userGhost.CourseRank = (uint)bossData.daniId.id;
						userGhost.MusicId = bossData.music.id;
						userGhost.Difficulty = bossData.difficulty.id;
						int achieveUnder = bossData.achieveUnder;
						int achieve = bossData.achieve;
						userGhost.Rank = GetRank(userGhost.Achievement = Random.Range(achieveUnder, achieve));
						userGhost.ClassValue = classValue;
						userGhost.ClassRank = (uint)UserUdemae.GetRateToUdemaeID(classValue);
					}
				}
			}
		}

		public static MusicClearrankID GetRank(int achieve)
		{
			int num = Random.Range(0, 1);
			if (achieve < 800000)
			{
				return MusicClearrankID.Rank_A;
			}
			if (achieve >= 800000 && achieve < 900000)
			{
				return (MusicClearrankID)(5 + num);
			}
			if (achieve >= 900000 && achieve < 940000)
			{
				return (MusicClearrankID)(6 + num);
			}
			if (achieve >= 940000 && achieve < 970000)
			{
				return (MusicClearrankID)(7 + num);
			}
			if (achieve >= 970000 && achieve < 990000)
			{
				return (MusicClearrankID)(8 + num * 2);
			}
			if (achieve >= 990000 && achieve < 1000000)
			{
				return (MusicClearrankID)(10 + num * 2);
			}
			if (achieve >= 1000000 && achieve < 1005000)
			{
				return (MusicClearrankID)(12 + num);
			}
			if (achieve >= 1005000 && achieve < 1010000)
			{
				return (MusicClearrankID)(13 + num);
			}
			return MusicClearrankID.End;
		}
	}
}
