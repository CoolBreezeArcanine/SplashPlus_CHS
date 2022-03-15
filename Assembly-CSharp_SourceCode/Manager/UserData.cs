using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager.UserDatas;

namespace Manager
{
	public class UserData
	{
		public enum Collection
		{
			Chara,
			Icon,
			Plate,
			Title,
			Partner,
			Frame,
			Ticket,
			End
		}

		public enum NetMember
		{
			None,
			NoCharge,
			Standard,
			Premium
		}

		public enum MusicUnlock
		{
			Base,
			Master,
			ReMaster,
			Strong,
			End
		}

		public enum UserIDType
		{
			Guest,
			New,
			Inherit,
			Exist,
			Ghost,
			Npc,
			Boss
		}

		private const long RatingListMax = 30L;

		public bool IsEntry { get; set; }

		public bool IsSave { get; set; }

		public UserIDType UserType { get; set; }

		public AimeId AimeId
		{
			get
			{
				if (IsGuest())
				{
					return AimeId.Invalid;
				}
				return new AimeId((uint)(Detail.UserID & 0xFFFFFFFFu));
			}
		}

		public string OfflineId { get; set; }

		public int IsNetMember { get; set; }

		public UserDetail Detail { get; set; } = new UserDetail();


		public UserExtend Extend { get; set; } = new UserExtend();


		public UserOption Option { get; set; } = new UserOption();


		public UserSort SortFilter { get; set; } = new UserSort();


		public List<UserScore>[] ScoreList { get; set; } = new List<UserScore>[6];


		public List<UserCourse> CourseList { get; set; } = new List<UserCourse>();


		public List<UserCharge> ChargeList { get; set; } = new List<UserCharge>();


		public List<UserChara> CharaList { get; set; } = new List<UserChara>();


		public UserRating RatingList { get; set; } = new UserRating();


		public List<UserItem> IconList { get; set; } = new List<UserItem>();


		public List<UserItem> PlateList { get; set; } = new List<UserItem>();


		public List<UserItem> TitleList { get; set; } = new List<UserItem>();


		public List<UserItem> PartnerList { get; set; } = new List<UserItem>();


		public List<UserItem> PresentList { get; set; } = new List<UserItem>();


		public List<UserItem> FrameList { get; set; } = new List<UserItem>();


		public List<UserItem> TicketList { get; set; } = new List<UserItem>();


		public List<int> MusicUnlockList { get; set; } = new List<int>();


		public List<int> MusicUnlockMasterList { get; set; } = new List<int>();


		public List<int> MusicUnlockReMasterList { get; set; } = new List<int>();


		public List<int> MusicUnlockStrongList { get; set; } = new List<int>();


		public List<int> FavoriteIconList { get; set; } = new List<int>();


		public List<int> FavoriteTitleList { get; set; } = new List<int>();


		public List<int> FavoritePlateList { get; set; } = new List<int>();


		public List<int> FavoriteCharaList { get; set; } = new List<int>();


		public List<int> FavoriteFrameList { get; set; } = new List<int>();


		public UserActivity Activity { get; set; } = new UserActivity();


		public List<UserMapData> MapList { get; set; } = new List<UserMapData>();


		public List<UserLoginBonus> LoginBonusList { get; set; } = new List<UserLoginBonus>();


		public List<UserCard> CardList { get; set; } = new List<UserCard>();


		public UserGhost MyGhost { get; set; } = new UserGhost();


		public UserUpdate Update { get; set; } = new UserUpdate();


		public List<UserRegion> PlayRegionList { get; set; } = new List<UserRegion>();


		public List<UserScoreRanking> ScoreRankingList { get; set; } = new List<UserScoreRanking>();


		public List<int> NewIconList { get; set; } = new List<int>();


		public List<int> NewPlateList { get; set; } = new List<int>();


		public List<int> NewTitleList { get; set; } = new List<int>();


		public List<int> NewPartnerList { get; set; } = new List<int>();


		public List<int> NewCharaList { get; set; } = new List<int>();


		public List<int> NewFrameList { get; set; } = new List<int>();


		public List<int> NewTicketList { get; set; } = new List<int>();


		public static bool IsGuest(UserIDType type)
		{
			return type == UserIDType.Guest;
		}

		public static bool IsNewUser(UserIDType type)
		{
			return type == UserIDType.New;
		}

		public static bool IsGhost(UserIDType type)
		{
			return type == UserIDType.Ghost;
		}

		public static bool IsNpc(UserIDType type)
		{
			return type == UserIDType.Npc;
		}

		public static bool IsBoss(UserIDType type)
		{
			return type == UserIDType.Boss;
		}

		public static bool IsHuman(UserIDType type)
		{
			if (!IsGhost(type) && !IsNpc(type))
			{
				return !IsBoss(type);
			}
			return false;
		}

		public bool IsGuest()
		{
			return IsGuest(UserType);
		}

		public bool IsGhost()
		{
			return IsGhost(UserType);
		}

		public bool IsNpc()
		{
			return IsNpc(UserType);
		}

		public bool IsBoss()
		{
			return IsBoss(UserType);
		}

		public bool IsHuman()
		{
			return IsHuman(UserType);
		}

		public UserData()
		{
			for (int i = 0; i < ScoreList.Length; i++)
			{
				ScoreList[i] = new List<UserScore>();
			}
			Initialize();
		}

		public void Initialize()
		{
			IsEntry = false;
			IsSave = false;
			UserType = UserIDType.Guest;
			OfflineId = "";
			IsNetMember = 0;
			Detail.Initialize();
			Extend.Initialize();
			Option.Initialize();
			SortFilter.Initialize();
			CharaList.Clear();
			RatingList.Clear();
			IconList.Clear();
			PlateList.Clear();
			TitleList.Clear();
			PartnerList.Clear();
			PresentList.Clear();
			FrameList.Clear();
			TicketList.Clear();
			NewIconList.Clear();
			NewPlateList.Clear();
			NewTitleList.Clear();
			NewPartnerList.Clear();
			NewFrameList.Clear();
			NewTicketList.Clear();
			NewCharaList.Clear();
			MusicUnlockList.Clear();
			MusicUnlockMasterList.Clear();
			MusicUnlockReMasterList.Clear();
			MusicUnlockStrongList.Clear();
			FavoriteIconList.Clear();
			FavoritePlateList.Clear();
			FavoriteTitleList.Clear();
			FavoriteCharaList.Clear();
			FavoriteFrameList.Clear();
			MapList.Clear();
			LoginBonusList.Clear();
			CardList.Clear();
			PlayRegionList.Clear();
			ScoreRankingList.Clear();
			CourseList.Clear();
			ChargeList.Clear();
			Activity.Clear();
			MyGhost.Clear();
			for (int i = 0; i < ScoreList.Length; i++)
			{
				ScoreList[i].Clear();
			}
			Detail.ContentBit.Clear();
			Extend.ExtendContendBit.Clear();
			Update.Clear();
		}

		public void CreateGhost(int index)
		{
			MyGhost.CreateGhost(Detail.UserID, UserGhost.GhostType.Player, index, Detail.UserName, Detail.EquipIconID, Detail.EquipPlateID, Detail.EquipTitleID, (int)Detail.Rating, RatingList.Udemae.ClassValue, Detail.CourseRank);
		}

		public void GhostEntry(UserGhost ghost)
		{
			Initialize();
			IsEntry = true;
			Detail.UserID = ghost.Id;
			Detail.UserName = ghost.Name;
			Detail.EquipIconID = ghost.IconId;
			Detail.EquipPlateID = ghost.PlateId;
			Detail.EquipTitleID = ghost.TitleId;
			Detail.Rating = (uint)ghost.Rate;
			Detail.CourseRank = ghost.CourseRank;
			Detail.EquipFrameID = 1;
			RatingList.Udemae.SetClassValue(ghost.ClassValue);
			switch (ghost.Type)
			{
			case UserGhost.GhostType.MapNpc:
				UserType = UserIDType.Npc;
				break;
			case UserGhost.GhostType.Player:
				UserType = UserIDType.Ghost;
				break;
			case UserGhost.GhostType.Boss:
				UserType = UserIDType.Boss;
				break;
			}
		}

		public void PartyEntry(ulong userId, string name, int icon, int rating, int classVal, int maxClassVal)
		{
			Initialize();
			IsEntry = true;
			Detail.UserID = userId;
			Detail.UserName = name;
			Detail.EquipIconID = icon;
			UserType = UserIDType.Exist;
			Detail.Rating = (uint)rating;
			RatingList.Udemae.SetClassValue(classVal);
			RatingList.Udemae.SetMaxClassValue(maxClassVal);
		}

		public bool AddCollections(Collection type, int id, bool addNewGet = false)
		{
			if (type == Collection.Chara)
			{
				if (CharaList.FirstOrDefault((UserChara c) => c.ID == id) == null)
				{
					CharaList.Add(new UserChara(id));
					NewCharaList.Add(id);
					return true;
				}
				return false;
			}
			List<UserItem> list = null;
			List<int> list2 = null;
			switch (type)
			{
			case Collection.Icon:
				list = IconList;
				list2 = NewIconList;
				break;
			case Collection.Plate:
				list = PlateList;
				list2 = NewPlateList;
				break;
			case Collection.Title:
				list = TitleList;
				list2 = NewTitleList;
				break;
			case Collection.Partner:
				list = PartnerList;
				list2 = NewPartnerList;
				break;
			case Collection.Frame:
				list = FrameList;
				list2 = NewFrameList;
				break;
			case Collection.Ticket:
				list = TicketList;
				list2 = NewTicketList;
				break;
			}
			if (list == null || list2 == null)
			{
				return false;
			}
			if (list.Select((UserItem s) => s).All((UserItem x) => x.itemId != id))
			{
				list.Add(new UserItem(id));
				if (addNewGet && !list2.Contains(id))
				{
					list2.Add(id);
				}
				return true;
			}
			if (type == Collection.Ticket)
			{
				list.Find((UserItem x) => x.itemId == id).stock++;
				if (addNewGet && !list2.Contains(id))
				{
					list2.Add(id);
				}
			}
			return false;
		}

		private List<int> GetMusicUnlockList(MusicUnlock type)
		{
			return type switch
			{
				MusicUnlock.Base => MusicUnlockList, 
				MusicUnlock.Master => MusicUnlockMasterList, 
				MusicUnlock.ReMaster => MusicUnlockReMasterList, 
				MusicUnlock.Strong => MusicUnlockStrongList, 
				_ => null, 
			};
		}

		public bool IsUnlockMusic(MusicUnlock type, int id)
		{
			return GetMusicUnlockList(type)?.Contains(id) ?? false;
		}

		public bool AddUnlockMusic(MusicUnlock type, int id)
		{
			List<int> musicUnlockList = GetMusicUnlockList(type);
			if (musicUnlockList == null)
			{
				return false;
			}
			if (!musicUnlockList.Contains(id))
			{
				musicUnlockList.Add(id);
				if (type == MusicUnlock.Base && !GameManager.IsFreedomMode)
				{
					Extend.SelectMusicID = id;
					GameManager.IsForceChangeMusic = true;
				}
				return true;
			}
			return false;
		}

		public void UpdateScore(int musicid, int difficulty, uint achive, uint romVersion)
		{
			RatingList.UpdateScore(musicid, difficulty, achive, romVersion);
			UpdateUserRate();
		}

		private List<int> GetFavoriteList(Collection fav)
		{
			return fav switch
			{
				Collection.Chara => FavoriteCharaList, 
				Collection.Icon => FavoriteIconList, 
				Collection.Plate => FavoritePlateList, 
				Collection.Title => FavoriteTitleList, 
				Collection.Frame => FavoriteFrameList, 
				_ => null, 
			};
		}

		public bool IsFavorite(Collection fav, int id)
		{
			return GetFavoriteList(fav)?.Contains(id) ?? false;
		}

		public bool SetFavorite(Collection fav, int id, bool flg)
		{
			List<int> favoriteList = GetFavoriteList(fav);
			if (favoriteList == null)
			{
				return false;
			}
			if (flg)
			{
				if (favoriteList.Contains(id))
				{
					return true;
				}
				favoriteList.Add(id);
			}
			else
			{
				favoriteList.RemoveAll((int x) => x == id);
			}
			if (favoriteList.Count > 20)
			{
				favoriteList.RemoveRange(20, favoriteList.Count - 20);
			}
			return favoriteList?.Contains(id) ?? false;
		}

		public bool FlipFavorite(Collection fav, int id)
		{
			List<int> favoriteList = GetFavoriteList(fav);
			if (favoriteList == null)
			{
				return false;
			}
			return SetFavorite(fav, id, favoriteList.Contains(id));
		}

		public uint UpdateUserRate()
		{
			UserRating ratingList = RatingList;
			Detail.MusicRating = 0u;
			Detail.GradeRating = 0u;
			foreach (UserRate rating in ratingList.RatingList)
			{
				Detail.MusicRating += rating.SingleRate;
			}
			foreach (UserRate newRating in ratingList.NewRatingList)
			{
				Detail.MusicRating += newRating.SingleRate;
			}
			Detail.Rating = Detail.MusicRating;
			if (Detail.Rating > Detail.HighestRating)
			{
				Detail.HighestRating = Detail.Rating;
			}
			Detail.ClassRank = (uint)ratingList.Udemae.ClassID;
			return Detail.Rating;
		}

		public void AddPlayCount()
		{
			Detail.PlayCount++;
		}

		public void UpdateTotalAwake()
		{
			Detail.TotalAwake = CharaList.Select((UserChara x) => (int)x.Awakening).Sum();
		}

		public void CheckUserData()
		{
			Extend.EncountMapNpcList.RemoveAll((MapEncountNpc s) => s.NpcId == 0 || s.MusicId == 0);
			Option.NoteSize = Option.NoteSize.GetDefault();
			Option.TouchSize = Option.TouchSize.GetDefault();
			Option.SlideSize = Option.SlideSize.GetDefault();
			Option.SubmonitorAnimation = Option.SubmonitorAnimation.GetDefault();
			CalcTotalValue();
		}

		public void CalcTotalValue()
		{
			List<UserScore>[] scoreList = ScoreList;
			Detail.TotalAchievement = 0L;
			Detail.TotalDeluxscore = 0L;
			Detail.TotalSync = 0;
			for (int i = 0; i < Detail.TotalAchieves.Length; i++)
			{
				Detail.TotalAchieves[i] = 0L;
			}
			for (int j = 0; j < Detail.TotalDeluxScores.Length; j++)
			{
				Detail.TotalDeluxScores[j] = 0L;
			}
			for (int k = 0; k < Detail.TotalSyncs.Length; k++)
			{
				Detail.TotalSyncs[k] = 0;
			}
			if (scoreList != null)
			{
				for (int l = 0; l < 6; l++)
				{
					List<UserScore> list = scoreList[l];
					if (list != null)
					{
						foreach (UserScore item in list)
						{
							if (Singleton<DataManager>.Instance.GetMusics().ContainsKey(item.id))
							{
								Detail.TotalAchieves[l] += item.achivement;
								Detail.TotalDeluxScores[l] += item.deluxscore;
								Detail.TotalSyncs[l] += item.sync.GetPoint();
							}
						}
					}
					Detail.TotalAchievement += Detail.TotalAchieves[l];
					Detail.TotalDeluxscore += Detail.TotalDeluxScores[l];
					Detail.TotalSync += Detail.TotalSyncs[l];
				}
			}
			UpdateTotalAwake();
		}

		public string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("IsEntry:\t" + IsEntry);
			stringBuilder.AppendLine("IsGuest:\t" + IsGuest());
			stringBuilder.AppendLine("UserID:\t" + Detail.UserID);
			stringBuilder.AppendLine("UserName:\t" + Detail.UserName);
			stringBuilder.AppendLine("Rating:\t" + Detail.Rating);
			stringBuilder.AppendLine("SelectMapID:\t" + Detail.SelectMapID);
			stringBuilder.AppendLine("LastLoginDate:\t" + Detail.LastLoginDate);
			stringBuilder.AppendLine($"CardList\t{CardList.Count}");
			stringBuilder.AppendLine($"MapList\t{MapList.Count}");
			stringBuilder.AppendLine($"CharaList\t{CharaList.Count}");
			return stringBuilder.ToString();
		}
	}
}
