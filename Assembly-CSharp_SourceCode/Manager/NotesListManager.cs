using System.Collections.Generic;
using MAI2.Util;
using MAI2System;
using Manager.MaiStudio;
using Manager.UserDatas;
using UnityEngine;
using Util;

namespace Manager
{
	public class NotesListManager : Singleton<NotesListManager>
	{
		private readonly SortedDictionary<int, NotesWrapper> _notesList = new SortedDictionary<int, NotesWrapper>();

		private readonly SortedDictionary<int, NotesWrapper> _strongScoreList = new SortedDictionary<int, NotesWrapper>();

		private int RankingMax = Singleton<MusicRankingManager>.Instance.GetRankingMaxNum();

		public SortedDictionary<int, NotesWrapper> GetNotesList()
		{
			return _notesList;
		}

		public SortedDictionary<int, NotesWrapper> GetStrongScoreList()
		{
			return _strongScoreList;
		}

		public void CreateScore()
		{
			CreateNormalNotes();
		}

		private void CreateNormalNotes()
		{
			CreateNormalNotesList();
		}

		private void CreateNormalNotesList()
		{
			Safe.ReadonlySortedDictionary<int, MusicData> musics = Singleton<DataManager>.Instance.GetMusics();
			List<MusicRankingManager.MusicRankingSt> rankings = Singleton<MusicRankingManager>.Instance.Rankings;
			_notesList.Clear();
			EventManager instance = Singleton<EventManager>.Instance;
			int[] array = new int[2]
			{
				GetTaskId(0),
				GetTaskId(1)
			};
			int[] array2 = new int[2]
			{
				GetChallengeId(0),
				GetChallengeId(1)
			};
			int[] array3 = new int[2] { -1, -1 };
			for (int i = 0; i < 2; i++)
			{
				if (array2[i] != -1)
				{
					ChallengeData challengeData = Singleton<DataManager>.Instance.GetChallengeData(array2[i]);
					if (challengeData != null && instance.IsOpenEvent(challengeData.EventName.id))
					{
						array3[i] = challengeData.Music.id;
					}
				}
			}
			foreach (KeyValuePair<int, MusicData> item in musics)
			{
				if (!Singleton<SystemConfig>.Instance.config.IsAllOpen && !instance.IsOpenEvent(item.Value.eventName.id))
				{
					continue;
				}
				NotesWrapper notesWrapper = new NotesWrapper
				{
					Name = item.Value.name,
					IsNewMusic = instance.IsNewEvent(item.Value.eventName.id)
				};
				notesWrapper.Ranking = -1;
				if (!GameManager.IsEventMode)
				{
					for (int j = 0; j < rankings.Count; j++)
					{
						if (rankings[j].MusicId == item.Value.name.id)
						{
							if (j < RankingMax)
							{
								notesWrapper.Ranking = j + 1;
							}
							break;
						}
					}
				}
				for (int k = 0; k < 2; k++)
				{
					if (!GameManager.IsFreedomMapSkip())
					{
						notesWrapper.IsMapBonus.Add(IsMapBonus(item.Value.name.id, k));
						notesWrapper.IsMapTaskMusic[k] = array[k] == item.Value.name.id;
						if (array3[k] == item.Value.name.id)
						{
							notesWrapper.ChallengeDetail[k] = ChallengeManager.GetChallengeDetail(array2[k]);
						}
					}
				}
				foreach (KeyValuePair<int, ScoreRankingForMusicSeq> scoreRanking in Singleton<ScoreRankingManager>.Instance.ScoreRankings)
				{
					foreach (ScoreRankingMusicInfo musicInfo in scoreRanking.Value.MusicInfoList)
					{
						if (musicInfo.MusicID == item.Value.name.id)
						{
							notesWrapper.ScoreRankings.Add(scoreRanking.Key);
							if (!musicInfo.IsLock)
							{
								notesWrapper.SpecialUnlock = true;
							}
						}
					}
				}
				if (item.Value.name.id >= 20000 && item.Value.name.id < 30000)
				{
					for (int l = 0; l < 6; l++)
					{
						notesWrapper.NotesList.Add(item.Value.notesData[l]);
						notesWrapper.EventName.Add(item.Value.eventName);
						notesWrapper.LockType.Add(item.Value.lockType);
						notesWrapper.IsEnable.Add(item.Value.notesData[l].isEnable);
						if (l == 5)
						{
							notesWrapper.IsUnlock.Add(IsUnlockStrong(item.Value.name.id, 0) || IsUnlockStrong(item.Value.name.id, 1));
							for (int m = 0; m < 2; m++)
							{
								notesWrapper.IsNeedUnlock[m].Add(!IsUnlockStrong(item.Value.name.id, m));
							}
						}
						else
						{
							notesWrapper.IsEnable[l] = false;
							notesWrapper.IsUnlock.Add(item: false);
							for (int n = 0; n < 2; n++)
							{
								notesWrapper.IsNeedUnlock[n].Add(item: false);
							}
						}
						for (int num = 0; num < 2; num++)
						{
							notesWrapper.IsRating[num].Add(IsRaging(item.Value.name.id, l, num));
							AddVsGhost(notesWrapper.VsGhost[num], item.Value.name.id, l, num);
							AddMapGhost(notesWrapper.MapGhost[num], item.Value.name.id, l, num);
							AddBossGhost(notesWrapper.BossGhost[num], item.Value.name.id, l, num);
						}
					}
				}
				else
				{
					for (int num2 = 0; num2 < 6; num2++)
					{
						notesWrapper.NotesList.Add(item.Value.notesData[num2]);
						notesWrapper.EventName.Add(item.Value.eventName);
						notesWrapper.LockType.Add(item.Value.lockType);
						notesWrapper.IsEnable.Add(item.Value.notesData[num2].isEnable);
						switch (num2)
						{
						case 3:
						{
							notesWrapper.IsUnlock.Add(IsUnlockMaster(item.Value.name.id, 0, notesWrapper.SpecialUnlock) || IsUnlockMaster(item.Value.name.id, 1, notesWrapper.SpecialUnlock));
							for (int num5 = 0; num5 < 2; num5++)
							{
								notesWrapper.IsNeedUnlock[num5].Add((!IsUnlockBase(item.Value.name.id, num5) && item.Value.lockType != 0) || !IsUnlockMaster(item.Value.name.id, num5, specialUnlock: false, checkCardOpen: false));
							}
							break;
						}
						case 4:
							if (item.Value.subLockType != 0)
							{
								notesWrapper.IsEnable[num2] = false;
								notesWrapper.IsUnlock.Add(item: false);
								for (int num7 = 0; num7 < 2; num7++)
								{
									notesWrapper.IsNeedUnlock[num7].Add(item: false);
								}
							}
							else if (!instance.IsOpenEvent(item.Value.subEventName.id))
							{
								notesWrapper.IsEnable[num2] = false;
								notesWrapper.IsUnlock.Add(item: false);
								for (int num8 = 0; num8 < 2; num8++)
								{
									notesWrapper.IsNeedUnlock[num8].Add(item: false);
								}
							}
							else
							{
								notesWrapper.IsUnlock.Add(IsUnlockReMaster(item.Value.name.id, 0, item.Value.subEventName.id, notesWrapper.SpecialUnlock) || IsUnlockReMaster(item.Value.name.id, 1, item.Value.subEventName.id, notesWrapper.SpecialUnlock));
								for (int num9 = 0; num9 < 2; num9++)
								{
									notesWrapper.IsNeedUnlock[num9].Add((!IsUnlockBase(item.Value.name.id, num9) && item.Value.lockType != 0) || !IsUnlockReMaster(item.Value.name.id, num9, item.Value.subEventName.id, specialUnlock: false, checkCardOpen: false));
								}
							}
							break;
						case 5:
						{
							notesWrapper.IsEnable[num2] = false;
							notesWrapper.IsUnlock.Add(item: false);
							for (int num6 = 0; num6 < 2; num6++)
							{
								notesWrapper.IsNeedUnlock[num6].Add(item: false);
							}
							break;
						}
						default:
						{
							if (item.Value.lockType == MusicLockType.Unlock)
							{
								notesWrapper.IsPlayable = true;
								notesWrapper.IsUnlock.Add(item: true);
								for (int num3 = 0; num3 < 2; num3++)
								{
									notesWrapper.IsNeedUnlock[num3].Add(item: false);
								}
								break;
							}
							notesWrapper.IsUnlock.Add(IsUnlockBase(item.Value.name.id, 0, notesWrapper.SpecialUnlock) || IsUnlockBase(item.Value.name.id, 1, notesWrapper.SpecialUnlock));
							for (int num4 = 0; num4 < 2; num4++)
							{
								notesWrapper.IsNeedUnlock[num4].Add(!IsUnlockBase(item.Value.name.id, num4));
							}
							if (num2 == 0)
							{
								notesWrapper.IsPlayable = notesWrapper.IsUnlock[0];
							}
							break;
						}
						}
						for (int num10 = 0; num10 < 2; num10++)
						{
							notesWrapper.IsRating[num10].Add(IsRaging(item.Value.name.id, num2, num10));
							AddVsGhost(notesWrapper.VsGhost[num10], item.Value.name.id, num2, num10);
							AddMapGhost(notesWrapper.MapGhost[num10], item.Value.name.id, num2, num10);
							AddBossGhost(notesWrapper.BossGhost[num10], item.Value.name.id, num2, num10);
						}
					}
				}
				_notesList.Add(notesWrapper.Name.id, notesWrapper);
			}
		}

		public List<int> GetRandomMusic(int levelLower, int levelUpper, MusicDifficultyID diff, int num)
		{
			List<int> list = new List<int>();
			foreach (NotesWrapper value in _notesList.Values)
			{
				if (!value.IsPlayable)
				{
					continue;
				}
				for (int i = 0; i < value.NotesList.Count; i++)
				{
					if ((i == (int)diff || diff == MusicDifficultyID.Invalid) && (i != 4 || value.IsEnable[4]) && value.LockType[i] != MusicLockType.Challenge && value.LockType[i] != MusicLockType.Transmission)
					{
						int num2 = value.NotesList[i].level * 10 + value.NotesList[i].levelDecimal;
						if (levelLower <= num2 && num2 <= levelUpper)
						{
							int item = value.Name.id * 100 + i;
							list.Add(item);
						}
					}
				}
			}
			List<int> list2 = new List<int>();
			for (int j = 0; j < num; j++)
			{
				int index = Random.Range(0, list.Count);
				list2.Add(list[index]);
			}
			return list2;
		}

		private void SetVoidData(ref NotesWrapper notes)
		{
			notes.NotesList.Add(new Notes());
			notes.EventName.Add(new StringID());
			notes.LockType.Add(MusicLockType.Lock);
			notes.IsEnable.Add(item: false);
			notes.IsUnlock.Add(item: false);
			for (int i = 0; i < 2; i++)
			{
				notes.IsRating[i].Add(item: false);
				notes.IsNeedUnlock[i].Add(item: false);
			}
		}

		private bool IsEntry(int index)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(index).IsEntry;
		}

		private bool IsGuest(int index)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(index).IsGuest();
		}

		private bool IsUnlockBase(int id, int index, bool specialUnlock = false)
		{
			if (IsEntry(index))
			{
				if (Singleton<SystemConfig>.Instance.config.IsAllOpen)
				{
					return true;
				}
				if (GameManager.IsCourseMode)
				{
					return true;
				}
				if (specialUnlock)
				{
					return true;
				}
				if (Singleton<UserDataManager>.Instance.GetUserData(index).MusicUnlockList.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsUnlockMaster(int id, int index, bool specialUnlock, bool checkCardOpen = true)
		{
			if (IsEntry(index))
			{
				if (Singleton<SystemConfig>.Instance.config.IsAllOpen)
				{
					return true;
				}
				if (id < 10000)
				{
					return true;
				}
				if (GameManager.IsEventMode)
				{
					return true;
				}
				if (GameManager.IsCourseMode)
				{
					return true;
				}
				if (specialUnlock)
				{
					return true;
				}
				if (checkCardOpen && GameManager.IsCardOpenMaster)
				{
					return true;
				}
				if (Singleton<UserDataManager>.Instance.GetUserData(index).MusicUnlockMasterList.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsUnlockReMaster(int id, int index, int subEventId, bool specialUnlock, bool checkCardOpen = true)
		{
			if (IsEntry(index))
			{
				EventManager instance = Singleton<EventManager>.Instance;
				if (Singleton<SystemConfig>.Instance.config.IsAllOpen)
				{
					return true;
				}
				if (!instance.IsOpenEvent(subEventId))
				{
					return false;
				}
				if (id < 10000)
				{
					return true;
				}
				if (GameManager.IsCourseMode)
				{
					return true;
				}
				if (specialUnlock)
				{
					return true;
				}
				if (GameManager.IsEventMode)
				{
					return true;
				}
				if (checkCardOpen && GameManager.IsCardOpenMaster)
				{
					return true;
				}
				if (Singleton<UserDataManager>.Instance.GetUserData(index).MusicUnlockReMasterList.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsUnlockStrong(int id, int index)
		{
			if (IsEntry(index))
			{
				if (Singleton<SystemConfig>.Instance.config.IsAllOpen)
				{
					return true;
				}
				if (GameManager.IsEventMode)
				{
					return true;
				}
				if (Singleton<UserDataManager>.Instance.GetUserData(index).MusicUnlockStrongList.Contains(id))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsRaging(int id, int difficulty, int index)
		{
			if (IsEntry(index) && !GameManager.IsEventMode && !IsGuest(index) && Singleton<UserDataManager>.Instance.GetUserData(index).RatingList.Contains(id, difficulty))
			{
				if (GameManager.IsCardOpenRating[index])
				{
					return true;
				}
				_ = Singleton<SystemConfig>.Instance.config.IsAllOpen;
				return true;
			}
			return false;
		}

		private bool IsMapBonus(int musicId, int index)
		{
			if (IsEntry(index) && !GameManager.IsEventMode && !IsGuest(index))
			{
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(Singleton<UserDataManager>.Instance.GetUserData(index).Detail.SelectMapID);
				if (mapData != null)
				{
					MapBonusMusicData mapBonusMusicData = Singleton<DataManager>.Instance.GetMapBonusMusicData(mapData.BonusMusicId.id);
					if (mapBonusMusicData != null)
					{
						foreach (StringID item in mapBonusMusicData.MusicIds.list)
						{
							if (item.id == musicId)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private int GetTaskId(int index)
		{
			if (IsEntry(index) && !GameManager.IsEventMode && !IsGuest(index))
			{
				int nowDistTreasureId = GetNowDistTreasureId(index);
				if (nowDistTreasureId != -1)
				{
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(nowDistTreasureId);
					if (mapTreasureData != null && mapTreasureData.TreasureType == MapTreasureType.MapTaskMusic)
					{
						return mapTreasureData.MusicId.id;
					}
				}
			}
			return -1;
		}

		private int GetChallengeId(int index)
		{
			if (IsEntry(index) && !GameManager.IsEventMode && !IsGuest(index))
			{
				int nowDistTreasureId = GetNowDistTreasureId(index);
				if (nowDistTreasureId != -1)
				{
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(nowDistTreasureId);
					if (mapTreasureData != null && mapTreasureData.TreasureType == MapTreasureType.Challenge)
					{
						return mapTreasureData.Challenge.id;
					}
				}
			}
			return -1;
		}

		private int GetNowDistTreasureId(int index)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(userData.Detail.SelectMapID);
			if (mapData != null)
			{
				UserMapData userMapData = userData.MapList.Find((UserMapData m) => m.ID == userData.Detail.SelectMapID);
				if (userMapData != null)
				{
					uint distance = userMapData.Distance;
					foreach (MapTreasureExData treasureExData in mapData.TreasureExDatas)
					{
						if (treasureExData.Distance == distance)
						{
							return treasureExData.TreasureId.id;
						}
					}
				}
			}
			return -1;
		}

		private void AddVsGhost(List<GhostMatchData> list, int musicId, int difficulty, int index)
		{
			if (!IsEntry(index) || GameManager.IsEventMode)
			{
				return;
			}
			int num = 0;
			foreach (UserGhost vsGhost in Singleton<GhostManager>.Instance.GetVsGhostList(index))
			{
				if (vsGhost.Difficulty == difficulty && vsGhost.MusicId == musicId)
				{
					list.Add(new GhostMatchData
					{
						Difficulty = (MusicDifficultyID)difficulty,
						Target = (GhostManager.GhostTarget)((index == 0) ? (5 + num) : (14 + num))
					});
				}
				num++;
			}
		}

		private void AddMapGhost(List<GhostMatchData> list, int musicId, int difficulty, int index)
		{
			if (!IsEntry(index) || GameManager.IsEventMode)
			{
				return;
			}
			int num = 0;
			foreach (UserGhost mapGhost in Singleton<GhostManager>.Instance.GetMapGhostList(index))
			{
				if (mapGhost.Difficulty == difficulty && mapGhost.MusicId == musicId)
				{
					list.Add(new GhostMatchData
					{
						Difficulty = (MusicDifficultyID)difficulty,
						Target = (GhostManager.GhostTarget)((index == 0) ? (2 + num) : (11 + num))
					});
				}
				num++;
			}
		}

		private void AddBossGhost(List<GhostMatchData> list, int musicId, int difficulty, int index)
		{
			if (IsEntry(index) && !GameManager.IsEventMode)
			{
				UserGhost bossGhostList = Singleton<GhostManager>.Instance.GetBossGhostList(index);
				if (bossGhostList.Difficulty == difficulty && bossGhostList.MusicId == musicId)
				{
					list.Add(new GhostMatchData
					{
						Difficulty = (MusicDifficultyID)difficulty,
						Target = ((index != 0) ? GhostManager.GhostTarget.BossGhost_2P : GhostManager.GhostTarget.BossGhost_1P)
					});
				}
			}
		}
	}
}
