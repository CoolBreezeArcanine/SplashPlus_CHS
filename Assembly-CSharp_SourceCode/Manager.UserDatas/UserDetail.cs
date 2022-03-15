using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2.Util;
using Manager.MaiStudio;
using Net;

namespace Manager.UserDatas
{
	public class UserDetail
	{
		public string LastGameId;

		public string LastRomVersion;

		public string LastDataVersion;

		public string LastLoginDate;

		public string LastPlayDate;

		public int LastPlaceId;

		public string LastPlaceName;

		public int LastAllNetId;

		public int LastRegionId;

		public string LastRegionName;

		public string LastClientId;

		public string LastCountryCode;

		public string FirstGameId;

		public string FirstRomVersion;

		public string FirstDataVersion;

		public string FirstPlayDate;

		public string CompatibleCmVersion;

		public int LastPlayCredit;

		public int LastPlayMode;

		public int LastSelectEMoney;

		public int LastSelectTicket;

		public int LastSelectCourse;

		public int LastCountCourse;

		public string DailyBonusDate;

		public string DailyCourseBonusDate;

		public int PlayVsCount;

		public int PlaySyncCount;

		public int WinCount;

		public int HelpCount;

		public int ComboCount;

		public long TotalDeluxscore;

		public int TotalSync;

		public long TotalAchievement;

		public ulong UserID { get; set; }

		public string UserName { get; set; }

		public string AccessCode { get; set; }

		public string AuthKey { get; set; }

		public int EquipIconID { get; set; }

		public int EquipTitleID { get; set; }

		public int EquipPlateID { get; set; }

		public int EquipPartnerID { get; set; }

		public int EquipFrameID { get; set; }

		public int SelectMapID { get; set; }

		public UserContentBit ContentBit { get; set; } = new UserContentBit();


		public int PlayCount { get; set; }

		public int TotalAwake { get; set; }

		public int IsNetMember { get; set; }

		public string EventWatchedDate { get; set; }

		public int[] CharaSlot { get; set; } = new int[5];


		public int[] CharaLockSlot { get; set; } = new int[5];


		public uint MusicRating { get; set; }

		public uint GradeRating { get; set; }

		public uint Rating { get; set; }

		public uint HighestRating { get; set; }

		public uint GradeRank { get; set; }

		public uint ClassRank { get; set; }

		public uint CourseRank { get; set; }

		public uint ContinueLoginCount { get; set; }

		public uint TotalLoginCount { get; set; }

		public long[] TotalDeluxScores { get; set; } = new long[6];


		public int[] TotalSyncs { get; set; } = new int[6];


		public long[] TotalAchieves { get; set; } = new long[6];


		public int CardType { get; set; }

		public string PreLastLoginDate { get; set; }

		public bool FirstPlayOnDay { get; set; }

		public UserDetail()
		{
			Initialize();
		}

		public void Initialize()
		{
			UserID = Net.UserID.GuestID();
			UserName = CommonMessageID.GuestUserName.GetName();
			AccessCode = "";
			AuthKey = "";
			PlayCount = 0;
			TotalAwake = 0;
			EquipIconID = 1;
			EquipTitleID = 1;
			EquipPlateID = 1;
			EquipPartnerID = 17;
			EquipFrameID = 1;
			SelectMapID = 0;
			MusicRating = 0u;
			GradeRating = 0u;
			Rating = 0u;
			HighestRating = 0u;
			ContinueLoginCount = 0u;
			TotalLoginCount = 0u;
			CardType = 0;
			PreLastLoginDate = "";
			FirstPlayOnDay = false;
			ContentBit.Clear();
			GradeRank = 0u;
			ClassRank = 0u;
			CourseRank = 0u;
			IsNetMember = 0;
			for (int i = 0; i < CharaSlot.Length; i++)
			{
				CharaSlot[i] = 0;
				CharaLockSlot[i] = 0;
			}
			PlayVsCount = 0;
			PlaySyncCount = 0;
			WinCount = 0;
			HelpCount = 0;
			ComboCount = 0;
			EventWatchedDate = "";
			LastGameId = "";
			LastRomVersion = "";
			LastDataVersion = "";
			LastLoginDate = "";
			LastPlayDate = "";
			LastPlaceId = 0;
			LastPlaceName = "";
			LastAllNetId = 0;
			LastRegionId = 0;
			LastRegionName = "";
			LastClientId = "";
			LastCountryCode = "";
			FirstGameId = "";
			FirstRomVersion = "";
			FirstDataVersion = "";
			FirstPlayDate = "";
			CompatibleCmVersion = "";
			LastPlayCredit = 0;
			LastPlayMode = 0;
			LastSelectEMoney = 0;
			LastSelectTicket = 0;
			LastSelectCourse = 0;
			LastCountCourse = 0;
			DailyBonusDate = "";
			DailyCourseBonusDate = "";
			TotalDeluxscore = 0L;
			TotalSync = 0;
			TotalAchievement = 0L;
			for (int j = 0; j < TotalDeluxScores.Length; j++)
			{
				TotalDeluxScores[j] = 0L;
			}
			for (int k = 0; k < TotalSyncs.Length; k++)
			{
				TotalSyncs[k] = 0;
			}
			for (int l = 0; l < TotalAchieves.Length; l++)
			{
				TotalAchieves[l] = 0L;
			}
		}

		public UserChara[] AutoComposeCharacterSlot(int playerIndex)
		{
			return AutoComposeCharacterSlot(playerIndex, null);
		}

		public UserChara[] AutoComposeCharacterSlot(int playerIndex, List<int> removeList, int leaderID = -1)
		{
			List<UserChara> list = new List<UserChara>(5);
			List<UserChara> list2 = new List<UserChara>();
			List<UserChara> list3 = new List<UserChara>();
			List<UserChara> charaList = Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CharaList;
			foreach (UserChara item2 in charaList)
			{
				if (removeList != null)
				{
					bool flag = false;
					foreach (int remove in removeList)
					{
						if (item2.ID == remove)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				if (IsMatchColor(playerIndex, item2.ID))
				{
					list2.Add(item2);
				}
				else
				{
					list3.Add(item2);
				}
			}
			list2.Sort(delegate(UserChara a, UserChara b)
			{
				int num = (int)(b.GetMovementParam(IsMatchColor(playerIndex, b.ID)) - a.GetMovementParam(IsMatchColor(playerIndex, a.ID)));
				if (num == 0)
				{
					num = (int)(a.Level - b.Level);
					if (num == 0)
					{
						return b.ID - a.ID;
					}
				}
				return num;
			});
			for (int i = 0; i < 5 && i < list2.Count; i++)
			{
				list.Add(list2[i]);
			}
			UserChara[] resultArray = new UserChara[5];
			if (list.Count >= 5)
			{
				for (int j = 0; j < resultArray.Length && j < list.Count; j++)
				{
					resultArray[j] = list[j];
				}
				InsertLockChara(charaList, ref resultArray, leaderID);
				return resultArray;
			}
			List<UserChara> list4 = new List<UserChara>();
			list3.Sort((UserChara a, UserChara b) => (int)(a.Level - b.Level));
			for (int k = 0; k < 5 && k < list3.Count; k++)
			{
				UserChara item = list3[k];
				list.Add(item);
				list4.Add(item);
			}
			if (list.Count >= 5)
			{
				for (int l = 0; l < resultArray.Length && l < list.Count; l++)
				{
					resultArray[l] = list[l];
				}
				InsertLockChara(charaList, ref resultArray, leaderID);
				return resultArray;
			}
			foreach (UserChara item3 in list)
			{
				if (list3.Contains(item3))
				{
					list3.Remove(item3);
				}
			}
			list4.Clear();
			list3.Sort((UserChara a, UserChara b) => a.ID - b.ID);
			for (int m = 0; m < 5 && m < list3.Count; m++)
			{
				list.Add(list3[m]);
			}
			for (int n = 0; n < resultArray.Length && n < list.Count; n++)
			{
				resultArray[n] = list[n];
			}
			InsertLockChara(charaList, ref resultArray, leaderID);
			return resultArray;
		}

		public bool IsMatchColor(int playerIndex, int ID)
		{
			StringID colorId = Singleton<DataManager>.Instance.GetMapData(Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.SelectMapID).ColorId;
			StringID color = Singleton<DataManager>.Instance.GetChara(ID).color;
			StringID colorGroupId = Singleton<DataManager>.Instance.GetMapColorData(colorId.id).ColorGroupId;
			StringID colorGroupId2 = Singleton<DataManager>.Instance.GetMapColorData(color.id).ColorGroupId;
			if (3 == colorGroupId.id)
			{
				return true;
			}
			return colorGroupId.id == colorGroupId2.id;
		}

		private void OldInsertLockChara(List<UserChara> userCharaList, ref UserChara[] resultArray)
		{
			for (int i = 0; i < CharaLockSlot.Length; i++)
			{
				if (CharaLockSlot[i] <= 0)
				{
					continue;
				}
				bool flag = true;
				for (int j = 0; j < resultArray.Length; j++)
				{
					if (CharaLockSlot[i] != resultArray[j].ID)
					{
						continue;
					}
					if (j < i)
					{
						UserChara userChara = resultArray[j];
						for (int k = j; k < i; k++)
						{
							resultArray[k] = resultArray[k + 1];
						}
						resultArray[i] = userChara;
					}
					else
					{
						UserChara userChara2 = resultArray[j];
						for (int num = j; num > i; num--)
						{
							resultArray[num] = resultArray[num - 1];
						}
						resultArray[i] = userChara2;
					}
					flag = false;
					break;
				}
				if (!flag)
				{
					continue;
				}
				for (int num2 = resultArray.Length - 1; num2 > i; num2--)
				{
					resultArray[num2] = resultArray[num2 - 1];
				}
				UserChara userChara3 = null;
				foreach (UserChara userChara4 in userCharaList)
				{
					if (userChara4.ID == CharaLockSlot[i])
					{
						userChara3 = userChara4;
						break;
					}
				}
				if (userChara3 != null)
				{
					resultArray[i] = userChara3;
				}
			}
		}

		private void InsertLockChara(List<UserChara> userCharaList, ref UserChara[] resultArray, int leaderID)
		{
			UserChara[] array = new UserChara[5];
			Queue<int> queue = new Queue<int>(5);
			Queue<int> queue2 = new Queue<int>(5);
			Queue<UserChara> queue3 = new Queue<UserChara>(5);
			if (userCharaList.Count <= 0)
			{
				return;
			}
			int i = 0;
			if (0 <= leaderID)
			{
				array[0] = userCharaList.First((UserChara c) => c.ID == leaderID);
				i = 1;
			}
			for (; i < CharaLockSlot.Length; i++)
			{
				if (0 < CharaLockSlot[i])
				{
					queue2.Enqueue(i);
				}
				else
				{
					queue.Enqueue(i);
				}
			}
			foreach (int index in queue2)
			{
				array[index] = userCharaList.First((UserChara c) => c.ID == CharaLockSlot[index]);
			}
			for (int j = 0; j < 5; j++)
			{
				bool flag = true;
				foreach (int item in queue2)
				{
					if (resultArray[j]?.ID == CharaLockSlot[item])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					queue3.Enqueue(resultArray[j]);
				}
			}
			foreach (int item2 in queue)
			{
				UserChara userChara = (array[item2] = queue3.Dequeue());
			}
			resultArray = array;
		}

		public void SetFirstOnDay(int playerIndex)
		{
			FirstPlayOnDay = false;
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
			if (userData.IsEntry)
			{
				UserData.UserIDType userType = userData.UserType;
				if ((uint)userType <= 2u)
				{
					FirstPlayOnDay = true;
				}
			}
			if (PreLastLoginDate != null && PreLastLoginDate.Length > 0 && LastLoginDate != null && LastLoginDate.Length > 0)
			{
				DateTime dateTime = DateTime.Parse(PreLastLoginDate);
				DateTime dateTime2 = DateTime.Parse(LastLoginDate);
				DateTime dateTime3 = DateTime.Parse(dateTime.ToShortDateString() + " 7:00:00");
				TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
				int hours = dateTime.TimeOfDay.Hours;
				if (hours >= 0 && hours < 7)
				{
					dateTime3 -= timeSpan;
				}
				DateTime dateTime4 = dateTime3;
				dateTime4 += timeSpan;
				if (dateTime2 >= dateTime4)
				{
					FirstPlayOnDay = true;
				}
			}
		}
	}
}
