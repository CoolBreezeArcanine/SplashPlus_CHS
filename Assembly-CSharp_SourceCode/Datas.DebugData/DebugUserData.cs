using System;
using System.Collections.Generic;
using DB;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Datas.DebugData
{
	[CreateAssetMenu(menuName = "Mai2Data/DebugDataCreate", fileName = "DebugUserData")]
	public class DebugUserData : ScriptableObject
	{
		[SerializeField]
		public bool IsEntry;

		[SerializeField]
		public UserData.NetMember IsNetMember;

		[SerializeField]
		public UserData.UserIDType UserIDType;

		[SerializeField]
		public ulong UserID;

		[SerializeField]
		public string UserName;

		[SerializeField]
		public string AccessCode;

		[SerializeField]
		public uint Rating;

		[SerializeField]
		public int ClassValue;

		[SerializeField]
		public int MaxClassValue;

		[SerializeField]
		public int PlayCount;

		[SerializeField]
		public int FirstFlags;

		[SerializeField]
		public int EquipIconID;

		[SerializeField]
		public int EquipTitleID;

		[SerializeField]
		public int EquipPlateID;

		[SerializeField]
		public int EquipPartnerID;

		[SerializeField]
		public int EquipFrameID;

		[SerializeField]
		public int SelectMapID = 1;

		[SerializeField]
		public DebugUserOption Option;

		[SerializeField]
		public DebugUserExtend Extend;

		[SerializeField]
		public DebugUserSort Sort;

		[SerializeField]
		public DebugUserScore ScoreList;

		[SerializeField]
		public List<DebugUserChara> CharaList = new List<DebugUserChara>();

		[SerializeField]
		public int[] CharaSlot = new int[5];

		[SerializeField]
		public List<int> iconList = new List<int>();

		[SerializeField]
		public List<int> plateList = new List<int>();

		[SerializeField]
		public List<int> titleList = new List<int>();

		[SerializeField]
		public List<int> partnerList = new List<int>();

		[SerializeField]
		public List<int> frameList = new List<int>();

		[SerializeField]
		public List<int> ticketList = new List<int>();

		[SerializeField]
		public int CardType;

		[SerializeField]
		public List<int> musicUnlockList = new List<int>();

		[SerializeField]
		public List<int> musicUnlockMasList = new List<int>();

		[SerializeField]
		public List<int> musicUnlockRemList = new List<int>();

		[SerializeField]
		public List<int> musicUnlockStrList = new List<int>();

		[SerializeField]
		public List<DebugUserMap> mapList = new List<DebugUserMap>();

		[SerializeField]
		public List<DebugUserLoginBonus> loginBonusList = new List<DebugUserLoginBonus>();

		[SerializeField]
		public List<UserCard> CardList = new List<UserCard>();

		[SerializeField]
		public List<UserAct> PlayList = new List<UserAct>();

		[SerializeField]
		public List<UserAct> MusicList = new List<UserAct>();

		[SerializeField]
		public List<UserCourse> CourseList = new List<UserCourse>();

		[SerializeField]
		public bool IsPhotoAgree;

		[SerializeField]
		public bool IsGotoCodeRead;

		[SerializeField]
		public bool IsGotoCharaSelect;

		[SerializeField]
		public bool IsGotoIconPhotoShoot;

		public UserData GetUserData()
		{
			UserData userData = new UserData();
			userData.IsEntry = IsEntry;
			userData.UserType = UserIDType;
			userData.Detail.UserID = UserID;
			userData.Detail.UserName = UserName;
			userData.Detail.AccessCode = AccessCode;
			userData.Detail.Rating = Rating;
			userData.RatingList.Udemae.SetClassValue(ClassValue);
			userData.RatingList.Udemae.SetMaxClassValue(MaxClassValue);
			userData.Detail.EquipIconID = EquipIconID;
			userData.Detail.EquipTitleID = EquipTitleID;
			userData.Detail.EquipPlateID = EquipPlateID;
			userData.Detail.EquipPartnerID = EquipPartnerID;
			userData.Detail.EquipFrameID = EquipFrameID;
			userData.Detail.SelectMapID = SelectMapID;
			userData.Detail.ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ClassValue);
			userData.Detail.IsNetMember = (int)IsNetMember;
			userData.Detail.PlayCount = PlayCount;
			userData.Activity.Clear();
			userData.Activity.PlayList = PlayList;
			userData.Activity.MusicList = MusicList;
			userData.Option = Option.GetUserOption();
			userData.Extend = Extend.GetUserExtend();
			foreach (MapEncountNpc encountMapNpc in Extend.EncountMapNpcList)
			{
				userData.Extend.AddEncountMapNpc(encountMapNpc.NpcId, encountMapNpc.MusicId);
			}
			userData.Extend.SelectedCardList = Extend.SelectedCardList;
			userData.Extend.SelectResultDetails = Extend.SelectResultDetails;
			userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, IsPhotoAgree);
			userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, IsGotoCodeRead);
			userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, IsGotoCharaSelect);
			userData.Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, IsGotoIconPhotoShoot);
			userData.SortFilter = new UserSort
			{
				SortMusic = Sort.sortMusic,
				SortTab = Sort.sortTab
			};
			ContentBitID[] array = (ContentBitID[])Enum.GetValues(typeof(ContentBitID));
			foreach (ContentBitID contentBitID in array)
			{
				if (contentBitID != ContentBitID.End && contentBitID != ContentBitID.Invalid)
				{
					bool flag = (FirstFlags & (1 << contentBitID.GetEnumValue())) > 0;
					userData.Detail.ContentBit.SetFlag(contentBitID, flag);
				}
			}
			userData.CharaList.Clear();
			userData.NewCharaList.Clear();
			foreach (DebugUserChara chara in CharaList)
			{
				UserChara userChara = new UserChara(chara.ID)
				{
					Awakening = chara.Awakening,
					Count = chara.Exp,
					Level = chara.Level,
					NextAwake = chara.NextAwake,
					NextAwakePercent = chara.NextAwakePercent
				};
				userChara.CalcLevelToAwake();
				userData.CharaList.Add(userChara);
			}
			userData.IconList.Clear();
			userData.NewIconList.Clear();
			foreach (int icon in iconList)
			{
				userData.IconList.Add(new UserItem(icon));
			}
			userData.PlateList.Clear();
			userData.NewPlateList.Clear();
			foreach (int plate in plateList)
			{
				userData.PlateList.Add(new UserItem(plate));
			}
			userData.TitleList.Clear();
			userData.NewTitleList.Clear();
			foreach (int title in titleList)
			{
				userData.TitleList.Add(new UserItem(title));
			}
			userData.PartnerList.Clear();
			userData.NewPartnerList.Clear();
			foreach (int partner in partnerList)
			{
				userData.PartnerList.Add(new UserItem(partner));
			}
			userData.FrameList.Clear();
			userData.NewFrameList.Clear();
			foreach (int frame in frameList)
			{
				userData.FrameList.Add(new UserItem(frame));
			}
			userData.TicketList.Clear();
			userData.NewTicketList.Clear();
			foreach (int ticket in ticketList)
			{
				userData.TicketList.Add(new UserItem(ticket));
			}
			userData.PresentList.Clear();
			userData.MusicUnlockList.Clear();
			foreach (int musicUnlock in musicUnlockList)
			{
				userData.MusicUnlockList.Add(musicUnlock);
			}
			userData.MusicUnlockMasterList.Clear();
			foreach (int musicUnlockMas in musicUnlockMasList)
			{
				userData.MusicUnlockMasterList.Add(musicUnlockMas);
			}
			userData.MusicUnlockReMasterList.Clear();
			foreach (int musicUnlockRem in musicUnlockRemList)
			{
				userData.MusicUnlockReMasterList.Add(musicUnlockRem);
			}
			userData.MusicUnlockStrongList.Clear();
			foreach (int musicUnlockStr in musicUnlockStrList)
			{
				userData.MusicUnlockStrongList.Add(musicUnlockStr);
			}
			userData.MapList.Clear();
			foreach (DebugUserMap map in mapList)
			{
				UserMapData item = new UserMapData(map.ID)
				{
					Distance = map.Distance,
					IsLock = map.IsLock,
					IsClear = map.IsClear,
					IsComplete = map.IsComplete
				};
				userData.MapList.Add(item);
			}
			userData.LoginBonusList.Clear();
			foreach (DebugUserLoginBonus loginBonus in loginBonusList)
			{
				UserLoginBonus item2 = new UserLoginBonus(loginBonus.ID)
				{
					Point = loginBonus.Point
				};
				userData.LoginBonusList.Add(item2);
			}
			userData.CardList = CardList;
			for (int j = 0; j < userData.ScoreList.Length; j++)
			{
				List<UserScore> list = j switch
				{
					0 => ScoreList.Basic, 
					1 => ScoreList.Advanced, 
					2 => ScoreList.Expart, 
					3 => ScoreList.Master, 
					4 => ScoreList.ReMaster, 
					_ => new List<UserScore>(), 
				};
				userData.ScoreList[j] = list;
			}
			for (int k = 0; k < userData.Detail.CharaSlot.Length; k++)
			{
				if (k < CharaSlot.Length)
				{
					userData.Detail.CharaSlot[k] = CharaSlot[k];
				}
			}
			userData.Detail.CardType = CardType;
			return userData;
		}
	}
}
