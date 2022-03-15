using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using AMDaemon;
using AMDaemon.Allnet;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using UnityEngine;

namespace Net.VO.Mai2
{
	public static class VOExtensions
	{
		public static void Export(this DailyLog src, ref ClientBookkeeping dst)
		{
			dst.placeId = (int)Auth.LocationId;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			dst.updateDate = BackUpTimeUtil.toString(src.dateTime);
			dst.creditSetting0 = (int)AMDaemon.Credit.Players[0].GameCosts[0];
			dst.creditSetting1 = (int)AMDaemon.Credit.Players[0].GameCosts[1];
			dst.creditSetting2 = (int)AMDaemon.Credit.Players[0].GameCosts[2];
			dst.regionId = Auth.RegionCode;
			dst.playCount = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.localParameter.playCount;
			dst.creditCoin = (int)src.coinCredit;
			dst.creditService = (int)src.serviceCredit;
			dst.creditEmoney = (int)src.emoneyCredit;
			dst.credits1P = (int)src.credits1P;
			dst.credits2P = (int)src.credits2P;
			dst.creditsFreedom = (int)src.creditsFreedom;
			dst.creditsTicket = (int)src.reserved[0];
			dst.timeTotal = (int)src.totalRunningTime.TotalSeconds;
			dst.timeTotalPlay = (int)src.totalPlayTime.TotalSeconds;
			dst.timeLongest1P = 0;
			dst.timeShortest1P = 0;
			dst.timeLongest2P = 0;
			dst.timeLongestFreedom = 0;
			dst.timeShortest2P = 0;
			dst.timeShortestFreedom = 0;
			dst.aimeLoginNum = (int)src.aimeLoginNum;
			dst.guestLoginNum = (int)src.guestLoginNum;
			dst.play1PNum = (int)src.play1PNum;
			dst.play2PNum = (int)src.play2PNum;
			dst.playFreedomNum = (int)src.playFreedomNum;
			dst.newFreeUserNum = (int)src.newCardFreePlayNum;
			dst.tutorialPlay = (int)src.tutorialNum;
		}

		public static void ExportClientSetting(ref ClientSetting dst)
		{
			dst.bordId = AMDaemon.System.BoardId.ShortValue;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			dst.placeId = (int)Auth.LocationId;
			dst.placeName = Auth.LocationName;
			dst.regionId = Auth.RegionCode;
			dst.regionName = Auth.RegionNames[0];
			dst.romVersion = 100;
			dst.isAou = true;
			dst.isDevelop = true;
		}

		public static void ExportClientTestmode(ref ClientTestmode dst)
		{
			dst.placeId = (int)Auth.LocationId;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			Manager.Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			if (backup != null)
			{
				dst.trackSingle = (backup.gameSetting.IsStandardSettingMachine ? 1 : 0);
				dst.trackMulti = (int)backup.gameSetting.MachineGroupID;
				dst.trackEvent = (int)backup.gameSetting.AdvVol;
				dst.totalMachine = backup.systemSetting.touchSens1P;
				dst.satelliteId = backup.systemSetting.touchSens2P;
				dst.cameraPosition = (backup.gameSetting.IsContinue ? 1 : 0);
			}
		}

		public static void Export(ref ClientUpload dst, string fname)
		{
			dst.orderId = 0;
			dst.divNumber = 0;
			dst.divLength = 0;
			dst.divData = null;
			dst.placeId = (int)Auth.LocationId;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			dst.uploadDate = TimeManager.GetNowDateString();
			dst.fileName = fname;
		}

		public static void ExportPhotoData(ref UserPhoto dst, ulong userId, ulong playlogId, int trackNo)
		{
			dst.orderId = 0;
			dst.userId = userId;
			dst.divNumber = 0;
			dst.divLength = 0;
			dst.divData = null;
			dst.placeId = (int)Auth.LocationId;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			dst.uploadDate = TimeManager.GetNowDateString();
			dst.playlogId = playlogId;
			dst.trackNo = trackNo;
		}

		public static Manager.UserDatas.UserActivity Export(this UserActivity src)
		{
			return new Manager.UserDatas.UserActivity
			{
				PlayList = src.playList.Select((UserAct r) => r.Convert()).ToList(),
				MusicList = src.musicList.Select((UserAct r) => r.Convert()).ToList()
			};
		}

		public static UserActivity Export(this Manager.UserDatas.UserActivity src)
		{
			UserActivity result = default(UserActivity);
			result.playList = src.PlayList.Select((Manager.UserDatas.UserAct r) => r.Convert()).ToArray();
			result.musicList = src.MusicList.Select((Manager.UserDatas.UserAct r) => r.Convert()).ToArray();
			return result;
		}

		private static Manager.UserDatas.UserAct Convert(this UserAct src)
		{
			return new Manager.UserDatas.UserAct(src.kind, src.id, src.sortNumber, src.param1, src.param2, src.param3, src.param4);
		}

		private static UserAct Convert(this Manager.UserDatas.UserAct src)
		{
			UserAct dst = default(UserAct);
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		public static UserActivity Clone(this UserActivity src)
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, src);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return (UserActivity)binaryFormatter.Deserialize(memoryStream);
		}

		public static void ExportUserAll(this UserData src, ref UserAll dst)
		{
			int num = ((Singleton<UserDataManager>.Instance.GetUserData(0L).Detail.UserID != src.Detail.UserID) ? ((Singleton<UserDataManager>.Instance.GetUserData(1L).Detail.UserID == src.Detail.UserID) ? 1 : (-1)) : 0);
			if (num != -1)
			{
				NetUserData netUserData = Singleton<NetDataManager>.Instance.GetNetUserData(num);
				dst.userData = new UserDetail[1] { src.Detail.Export() };
				dst.userExtend = new UserExtend[1] { src.Extend.Export() };
				dst.userOption = new UserOption[1] { src.Option.Export() };
				dst.userGhost = ((!src.MyGhost.NeedSave) ? null : new UserGhost[1] { src.MyGhost.Export() });
				dst.userRatingList = new UserRating[1] { src.RatingList.Export() };
				dst.userChargeList = src.ExportUserChargeList();
				UserCharacter[] characterList = netUserData.CharacterList;
				UserCharacter[] nsrc = src.CharaList.Export();
				dst.userCharacterList = BuildListData(out dst.isNewCharacterList, nsrc, characterList, (UserCharacter a, UserCharacter b) => a.characterId == b.characterId);
				UserMap[] mapList = netUserData.MapList;
				UserMap[] nsrc2 = src.MapList.Export();
				dst.userMapList = BuildListData(out dst.isNewMapList, nsrc2, mapList, (UserMap a, UserMap b) => a.mapId == b.mapId);
				UserLoginBonus[] loginBonusList = netUserData.LoginBonusList;
				UserLoginBonus[] nsrc3 = src.LoginBonusList.Export();
				dst.userLoginBonusList = BuildListData(out dst.isNewLoginBonusList, nsrc3, loginBonusList, (UserLoginBonus a, UserLoginBonus b) => a.bonusId == b.bonusId);
				UserItem[] osrc = netUserData.ExportUserItems();
				UserItem[] nsrc4 = src.ExportUserItems();
				dst.userItemList = BuildListData(out dst.isNewItemList, nsrc4, osrc, (UserItem a, UserItem b) => a.itemId == b.itemId && a.itemKind == b.itemKind);
				UserMusicDetail[] musicDetailList = netUserData.MusicDetailList;
				UserMusicDetail[] nsrc5 = src.ExportScoreDetailList();
				dst.userMusicDetailList = BuildListData(out dst.isNewMusicDetailList, nsrc5, musicDetailList, (UserMusicDetail a, UserMusicDetail b) => a.musicId == b.musicId && a.level == b.level);
				UserCourse[] courseList = netUserData.CourseList;
				UserCourse[] nsrc6 = src.CourseList.Export();
				dst.userCourseList = BuildListData(out dst.isNewCourseList, nsrc6, courseList, (UserCourse a, UserCourse b) => a.courseId == b.courseId);
				dst.userFavoriteList = Array.Empty<UserFavorite>();
				dst.isNewFavoriteList = "";
				dst.userGamePlaylogList = new UserGamePlaylog[1] { src.ExportUserGamePlaylog(num) };
				dst.userActivityList = new UserActivity[1] { src.Activity.Export() };
			}
		}

		public static T[] BuildListData<T>(out string flags, T[] nsrc, T[] osrc, Func<T, T, bool> compare)
		{
			T[] except = nsrc.Except(osrc).ToArray();
			string[] array = Enumerable.Repeat("1", except.Length).ToArray();
			T val = default(T);
			int i;
			for (i = 0; i < except.Length; i++)
			{
				if (!osrc.FirstOrDefault((T n) => compare(n, except[i])).Equals(val))
				{
					array[i] = "0";
				}
			}
			flags = string.Join("", array);
			return except;
		}

		public static T[] BuildListData2<T>(out string flags, T[] nsrc, T[] osrc, Func<T, T, bool> compare)
		{
			IEnumerable<string> first = nsrc.Select((T i) => JsonUtility.ToJson(i));
			IEnumerable<string> second = osrc.Select((T i) => JsonUtility.ToJson(i));
			T[] except = first.Except(second).Select(JsonUtility.FromJson<T>).ToArray();
			string[] array = Enumerable.Repeat("1", except.Length).ToArray();
			T val = default(T);
			int j;
			for (j = 0; j < except.Length; j++)
			{
				if (!osrc.FirstOrDefault((T n) => compare(n, except[j])).Equals(val))
				{
					array[j] = "0";
				}
			}
			flags = string.Join("", array);
			return except;
		}

		public static List<Manager.UserDatas.UserCard> Export(this UserCard[] src)
		{
			return src.Select((UserCard c) => c.Convert()).ToList();
		}

		private static Manager.UserDatas.UserCard Convert(this UserCard src)
		{
			return new Manager.UserDatas.UserCard
			{
				cardId = src.cardId,
				cardTypeId = src.cardTypeId,
				charaId = src.charaId,
				mapId = src.mapId,
				startDate = TimeManager.GetUnixTime(src.startDate),
				endDataDate = TimeManager.GetUnixTime(src.endDate)
			};
		}

		public static UserCharacter[] Export(this List<UserChara> src)
		{
			return src.Select((UserChara c) => c.Convert()).ToArray();
		}

		private static UserCharacter Convert(this UserChara src)
		{
			UserCharacter result = default(UserCharacter);
			result.characterId = src.ID;
			result.level = src.Level;
			result.awakening = src.Awakening;
			result.useCount = src.Count;
			return result;
		}

		public static UserChargelog ExportUserChargelog(this UserData src, int ticketID)
		{
			UserChargelog result = default(UserChargelog);
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketID);
			result.chargeId = ticketID;
			if (ticket != null)
			{
				result.price = ticket.creditNum;
			}
			result.purchaseDate = TimeManager.GetNowDateString();
			result.playCount = src.Detail.PlayCount;
			result.playerRating = (int)src.Detail.Rating;
			result.placeId = (int)Auth.LocationId;
			result.regionId = Auth.RegionCode;
			result.clientId = AMDaemon.System.KeychipId.ShortValue;
			return result;
		}

		public static UserCharge[] ExportUserChargeList(this UserData src)
		{
			UserCharge[] array = new UserCharge[src.ChargeList.Count];
			int num = 0;
			foreach (Manager.UserDatas.UserCharge charge in src.ChargeList)
			{
				array[num].chargeId = charge.chargeId;
				array[num].stock = charge.stock;
				array[num].purchaseDate = charge.purchaseDate;
				array[num].validDate = charge.validDate;
				num++;
			}
			return array;
		}

		public static UserCharge ExportEnableUserCharge(this UserData src, int ticketId)
		{
			UserCharge result = default(UserCharge);
			foreach (Manager.UserDatas.UserCharge charge in src.ChargeList)
			{
				if (charge.stock > 0 && charge.chargeId == ticketId)
				{
					result.chargeId = charge.chargeId;
					result.stock = charge.stock;
					result.purchaseDate = charge.purchaseDate;
					result.validDate = charge.validDate;
					return result;
				}
			}
			return result;
		}

		public static UserCourse[] Export(this List<Manager.UserDatas.UserCourse> src)
		{
			return src.Select((Manager.UserDatas.UserCourse c) => c.Convert()).ToArray();
		}

		private static UserCourse Convert(this Manager.UserDatas.UserCourse src)
		{
			UserCourse result = default(UserCourse);
			result.courseId = src.courseId;
			result.isLastClear = src.isLastClear;
			result.totalRestlife = src.totalRestlife;
			result.totalAchievement = src.totalAchievement;
			result.totalDeluxscore = src.totalDeluxscore;
			result.playCount = src.playCount;
			result.clearDate = src.clearDate;
			result.lastPlayDate = src.lastPlayDate;
			result.bestAchievement = src.bestAchievement;
			result.bestAchievementDate = src.bestAchievementDate;
			result.bestDeluxscore = src.bestDeluxscore;
			result.bestDeluxscoreDate = src.bestDeluxscoreDate;
			return result;
		}

		public static Manager.UserDatas.UserDetail Export(this UserDetail src, ulong userId)
		{
			return new Manager.UserDatas.UserDetail
			{
				UserID = userId,
				AccessCode = src.accessCode,
				UserName = src.userName,
				IsNetMember = src.isNetMember,
				EquipIconID = src.iconId,
				EquipPlateID = src.plateId,
				EquipTitleID = src.titleId,
				EquipPartnerID = src.partnerId,
				EquipFrameID = src.frameId,
				SelectMapID = src.selectMapId,
				TotalAwake = src.totalAwake,
				GradeRating = (uint)src.gradeRating,
				MusicRating = (uint)src.musicRating,
				Rating = (uint)src.playerRating,
				HighestRating = (uint)src.highestRating,
				GradeRank = (uint)src.gradeRank,
				ClassRank = (uint)src.classRank,
				CourseRank = (uint)src.courseRank,
				CharaSlot = (int[])src.charaSlot.Clone(),
				CharaLockSlot = (int[])src.charaLockSlot.Clone(),
				ContentBit = ToContentBit(src.contentBit),
				PlayCount = src.playCount,
				EventWatchedDate = src.eventWatchedDate,
				FirstGameId = src.firstGameId,
				FirstRomVersion = src.firstRomVersion,
				FirstDataVersion = src.firstDataVersion,
				FirstPlayDate = src.firstPlayDate,
				LastGameId = src.lastGameId,
				LastRomVersion = src.lastRomVersion,
				LastDataVersion = src.lastDataVersion,
				LastLoginDate = src.lastLoginDate,
				LastPlayDate = src.lastPlayDate,
				LastPlayCredit = 0,
				LastPlayMode = 0,
				LastPlaceId = src.lastPlaceId,
				LastPlaceName = src.lastPlaceName,
				LastAllNetId = src.lastAllNetId,
				LastRegionId = src.lastRegionId,
				LastRegionName = src.lastRegionName,
				LastClientId = src.lastClientId,
				LastCountryCode = src.lastCountryCode,
				LastSelectEMoney = src.lastSelectEMoney,
				LastSelectTicket = src.lastSelectTicket,
				LastSelectCourse = src.lastSelectCourse,
				LastCountCourse = src.lastCountCourse,
				DailyBonusDate = src.dailyBonusDate,
				DailyCourseBonusDate = src.dailyCourseBonusDate,
				CompatibleCmVersion = src.compatibleCmVersion,
				PlayVsCount = src.playVsCount,
				PlaySyncCount = src.playSyncCount,
				WinCount = src.winCount,
				HelpCount = src.helpCount,
				ComboCount = src.comboCount
			};
		}

		public static UserDetail Export(this Manager.UserDatas.UserDetail src)
		{
			UserDetail result = default(UserDetail);
			result.accessCode = src.AccessCode;
			result.userName = src.UserName;
			result.isNetMember = src.IsNetMember;
			result.iconId = src.EquipIconID;
			result.plateId = src.EquipPlateID;
			result.titleId = src.EquipTitleID;
			result.partnerId = src.EquipPartnerID;
			result.frameId = src.EquipFrameID;
			result.selectMapId = src.SelectMapID;
			result.totalAwake = src.TotalAwake;
			result.gradeRating = (int)src.GradeRating;
			result.musicRating = (int)src.MusicRating;
			result.playerRating = (int)src.Rating;
			result.highestRating = (int)src.HighestRating;
			result.gradeRank = (int)src.GradeRank;
			result.classRank = (int)src.ClassRank;
			result.courseRank = (int)src.CourseRank;
			result.charaSlot = (int[])src.CharaSlot.Clone();
			result.charaLockSlot = (int[])src.CharaLockSlot.Clone();
			result.contentBit = FromContentBit(src.ContentBit);
			result.playCount = src.PlayCount;
			result.eventWatchedDate = src.EventWatchedDate;
			result.lastGameId = AMDaemon.System.GameId;
			result.lastRomVersion = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionString;
			result.lastDataVersion = Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionString;
			result.lastLoginDate = src.LastLoginDate;
			result.lastPlayDate = TimeManager.GetNowDateString();
			result.lastPlayCredit = src.LastPlayCredit;
			result.lastPlayMode = src.LastPlayMode;
			result.lastPlaceId = (int)Auth.LocationId;
			result.lastPlaceName = Auth.LocationName;
			result.lastRegionId = Auth.RegionCode;
			result.lastRegionName = Auth.RegionNames[0];
			result.lastClientId = AMDaemon.System.KeychipId.ShortValue;
			result.lastCountryCode = Auth.CountryCode;
			result.lastSelectEMoney = src.LastSelectEMoney;
			result.lastSelectTicket = src.LastSelectTicket;
			result.lastSelectCourse = src.LastSelectCourse;
			result.lastCountCourse = src.LastCountCourse;
			result.lastAllNetId = 0;
			result.firstPlayDate = (string.IsNullOrEmpty(src.FirstPlayDate) ? TimeManager.GetDateString(TimeManager.PlayBaseTime) : src.FirstPlayDate);
			result.firstGameId = (string.IsNullOrEmpty(src.FirstGameId) ? AMDaemon.System.GameId : src.FirstGameId);
			result.firstRomVersion = (string.IsNullOrEmpty(src.FirstRomVersion) ? Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionString : src.FirstRomVersion);
			result.firstDataVersion = (string.IsNullOrEmpty(src.FirstDataVersion) ? Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionString : src.FirstDataVersion);
			result.compatibleCmVersion = Singleton<SystemConfig>.Instance.config.cardMakerVersionInfo.versionNo.versionString;
			result.dailyBonusDate = (string.IsNullOrEmpty(src.DailyBonusDate) ? TimeManager.GetDateString(0L) : src.DailyBonusDate);
			result.dailyCourseBonusDate = (string.IsNullOrEmpty(src.DailyCourseBonusDate) ? TimeManager.GetDateString(0L) : src.DailyCourseBonusDate);
			result.playVsCount = src.PlayVsCount;
			result.playSyncCount = src.PlaySyncCount;
			result.winCount = src.WinCount;
			result.helpCount = src.HelpCount;
			result.comboCount = src.ComboCount;
			result.totalDeluxscore = src.TotalDeluxscore;
			result.totalBasicDeluxscore = src.TotalDeluxScores[0];
			result.totalAdvancedDeluxscore = src.TotalDeluxScores[1];
			result.totalExpertDeluxscore = src.TotalDeluxScores[2];
			result.totalMasterDeluxscore = src.TotalDeluxScores[3];
			result.totalReMasterDeluxscore = src.TotalDeluxScores[4];
			result.totalSync = src.TotalSync;
			result.totalBasicSync = src.TotalSyncs[0];
			result.totalAdvancedSync = src.TotalSyncs[1];
			result.totalExpertSync = src.TotalSyncs[2];
			result.totalMasterSync = src.TotalSyncs[3];
			result.totalReMasterSync = src.TotalSyncs[4];
			result.totalAchievement = src.TotalAchievement;
			result.totalBasicAchievement = src.TotalAchieves[0];
			result.totalAdvancedAchievement = src.TotalAchieves[1];
			result.totalExpertAchievement = src.TotalAchieves[2];
			result.totalMasterAchievement = src.TotalAchieves[3];
			result.totalReMasterAchievement = src.TotalAchieves[4];
			result.dateTime = new DateTimeOffset(Auth.AuthTime).ToUnixTimeSeconds();
			return result;
		}

		private static UserContentBit ToContentBit(ulong src)
		{
			UserContentBit userContentBit = new UserContentBit();
			for (ContentBitID contentBitID = ContentBitID.FirstPlay; contentBitID < ContentBitID.End; contentBitID++)
			{
				ulong num = (ulong)(1L << (int)contentBitID);
				if ((src & num) != 0L)
				{
					userContentBit.SetFlag(contentBitID, flag: true);
				}
			}
			return userContentBit;
		}

		private static ulong FromContentBit(UserContentBit src)
		{
			ulong num = 0uL;
			for (ContentBitID contentBitID = ContentBitID.FirstPlay; contentBitID < ContentBitID.End; contentBitID++)
			{
				if (src.IsFlagOn(contentBitID))
				{
					num |= (ulong)(1L << (int)contentBitID);
				}
			}
			return num;
		}

		public static Manager.UserDatas.UserExtend Export(this UserExtend src)
		{
			return new Manager.UserDatas.UserExtend
			{
				SelectMusicID = src.selectMusicId,
				SelectDifficultyID = src.selectDifficultyId,
				CategoryIndex = src.categoryIndex,
				MusicIndex = src.musicIndex,
				ExtraFlag = src.extraFlag,
				ExtendContendBit = ToExtendContentBit(src.extendContentBit),
				SelectScoreType = src.selectScoreType,
				SelectResultDetails = src.selectResultDetails,
				SortCategorySetting = src.sortCategorySetting,
				SortMusicSetting = src.sortMusicSetting,
				SelectedCardList = src.selectedCardList.ToList(),
				EncountMapNpcList = src.encountMapNpcList.Select((MapEncountNpc r) => r.Convert()).ToList()
			};
		}

		public static UserExtend Export(this Manager.UserDatas.UserExtend src)
		{
			UserExtend result = default(UserExtend);
			result.selectMusicId = src.SelectMusicID;
			result.selectDifficultyId = src.SelectDifficultyID;
			result.categoryIndex = src.CategoryIndex;
			result.musicIndex = src.MusicIndex;
			result.extraFlag = src.ExtraFlag;
			result.extendContentBit = FromExtendContentBit(src.ExtendContendBit);
			result.selectScoreType = src.SelectScoreType;
			result.selectResultDetails = src.SelectResultDetails;
			result.sortCategorySetting = src.SortCategorySetting;
			result.sortMusicSetting = src.SortMusicSetting;
			result.selectedCardList = src.SelectedCardList.ToArray();
			result.encountMapNpcList = src.EncountMapNpcList.Select((Manager.UserDatas.MapEncountNpc r) => r.Convert()).ToArray();
			return result;
		}

		private static Manager.UserDatas.MapEncountNpc Convert(this MapEncountNpc src)
		{
			return new Manager.UserDatas.MapEncountNpc(src.npcId, src.musicId);
		}

		private static MapEncountNpc Convert(this Manager.UserDatas.MapEncountNpc src)
		{
			MapEncountNpc dst = default(MapEncountNpc);
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		private static UserExtendContentBit ToExtendContentBit(ulong src)
		{
			UserExtendContentBit userExtendContentBit = new UserExtendContentBit();
			for (ExtendContentBitID extendContentBitID = ExtendContentBitID.PhotoAgree; extendContentBitID < ExtendContentBitID.End; extendContentBitID++)
			{
				ulong num = (ulong)(1L << (int)extendContentBitID);
				if ((src & num) != 0L)
				{
					userExtendContentBit.SetFlag(extendContentBitID, flag: true);
				}
			}
			return userExtendContentBit;
		}

		private static ulong FromExtendContentBit(UserExtendContentBit src)
		{
			ulong num = 0uL;
			for (ExtendContentBitID extendContentBitID = ExtendContentBitID.PhotoAgree; extendContentBitID < ExtendContentBitID.End; extendContentBitID++)
			{
				if (src.IsFlagOn(extendContentBitID))
				{
					num |= (ulong)(1L << (int)extendContentBitID);
				}
			}
			return num;
		}

		public static List<int> Export(this UserFavorite src)
		{
			return src.itemIdList.ToList();
		}

		public static UserFavorite[] ExportUserFavorites(this UserData src)
		{
			return new UserFavorite[5]
			{
				new UserFavorite
				{
					userId = src.Detail.UserID,
					itemKind = 1,
					itemIdList = src.FavoriteIconList.ToArray()
				},
				new UserFavorite
				{
					userId = src.Detail.UserID,
					itemKind = 2,
					itemIdList = src.FavoritePlateList.ToArray()
				},
				new UserFavorite
				{
					userId = src.Detail.UserID,
					itemKind = 3,
					itemIdList = src.FavoriteTitleList.ToArray()
				},
				new UserFavorite
				{
					userId = src.Detail.UserID,
					itemKind = 4,
					itemIdList = src.FavoriteCharaList.ToArray()
				},
				new UserFavorite
				{
					userId = src.Detail.UserID,
					itemKind = 5,
					itemIdList = src.FavoriteFrameList.ToArray()
				}
			};
		}

		public static UserGamePlaylog ExportUserGamePlaylog(this UserData src, int playerId)
		{
			UserGamePlaylog result = default(UserGamePlaylog);
			result.playlogId = Singleton<NetDataManager>.Instance.GetLoginVO(playerId)?.loginId ?? 0;
			int num = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughPlay();
			if (GameManager.IsFreedomMode)
			{
				num = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughFreedom();
			}
			int useTicketId = Singleton<TicketManager>.Instance.GetUseTicketId(playerId);
			int ticketCredit = Singleton<TicketManager>.Instance.GetTicketCredit(useTicketId);
			int playCredit = num + ticketCredit;
			int max = 32051994;
			int num2 = UnityEngine.Random.Range(1, max) * 67;
			num2 = (int)(num2 + GameManager.SpecialKindNum);
			int num3 = 0;
			for (int i = 0; i < 32; i++)
			{
				num3 <<= 1;
				num3 += num2 % 2;
				num2 >>= 1;
			}
			result.version = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionString;
			result.playDate = TimeManager.GetNowDateString();
			result.playMode = 0;
			if (GameManager.IsFreedomMode)
			{
				result.playMode = 1;
			}
			if (GameManager.IsCourseMode)
			{
				result.playMode = 2;
			}
			result.useTicketId = useTicketId;
			result.playCredit = playCredit;
			result.playTrack = Singleton<GamePlayManager>.Instance.GetScoreListCount();
			result.clientId = AMDaemon.System.KeychipId.ShortValue;
			result.isPlayTutorial = GameManager.TutorialPlayed != GameManager.TutorialEnum.NotPlay;
			result.isEventMode = GameManager.IsEventMode;
			result.isNewFree = src.UserType == UserData.UserIDType.New;
			result.playCount = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.localParameter.playCount;
			result.playSpecial = num3;
			return result;
		}

		public static List<Manager.UserDatas.UserGhost> Export(this UserGhost[] src)
		{
			return src.Select((UserGhost g) => g.Export()).ToList();
		}

		private static Manager.UserDatas.UserGhost Export(this UserGhost src)
		{
			Manager.UserDatas.UserGhost dst = new Manager.UserDatas.UserGhost();
			NetPacketUtil.CopyMember(ref dst, src);
			dst.UnixTime = TimeManager.GetUnixTime(src.playDatetime);
			dst.ResultBitList = src.resultBitList.ToList();
			return dst;
		}

		public static UserGhost Export(this Manager.UserDatas.UserGhost src)
		{
			UserGhost dst = default(UserGhost);
			NetPacketUtil.CopyMember(ref dst, src);
			dst.playDatetime = TimeManager.GetDateString(src.UnixTime);
			dst.resultBitList = src.ResultBitList.ToArray();
			dst.version = (int)Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionCode;
			return dst;
		}

		public static List<Manager.UserDatas.UserItem> Export(this UserItem[] src)
		{
			return src.Select((UserItem s) => s.Convert()).ToList();
		}

		public static List<int> ExportToIntList(this UserItem[] src)
		{
			return src.Select((UserItem i) => i.itemId).ToList();
		}

		public static UserItem[] ExportUserItems(this UserData src)
		{
			return new List<IEnumerable<UserItem>>
			{
				src.PlateList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result11 = default(UserItem);
					result11.itemKind = 1;
					result11.itemId = i.itemId;
					result11.stock = i.stock;
					result11.isValid = i.isValid;
					return result11;
				}),
				src.TitleList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result10 = default(UserItem);
					result10.itemKind = 2;
					result10.itemId = i.itemId;
					result10.stock = i.stock;
					result10.isValid = i.isValid;
					return result10;
				}),
				src.IconList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result9 = default(UserItem);
					result9.itemKind = 3;
					result9.itemId = i.itemId;
					result9.stock = i.stock;
					result9.isValid = i.isValid;
					return result9;
				}),
				src.PartnerList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result8 = default(UserItem);
					result8.itemKind = 10;
					result8.itemId = i.itemId;
					result8.stock = i.stock;
					result8.isValid = i.isValid;
					return result8;
				}),
				src.PresentList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result7 = default(UserItem);
					result7.itemKind = 4;
					result7.itemId = i.itemId;
					result7.stock = i.stock;
					result7.isValid = i.isValid;
					return result7;
				}),
				src.FrameList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result6 = default(UserItem);
					result6.itemKind = 11;
					result6.itemId = i.itemId;
					result6.stock = i.stock;
					result6.isValid = i.isValid;
					return result6;
				}),
				src.TicketList.Select(delegate(Manager.UserDatas.UserItem i)
				{
					UserItem result5 = default(UserItem);
					result5.itemKind = 12;
					result5.itemId = i.itemId;
					result5.stock = i.stock;
					result5.isValid = i.isValid;
					return result5;
				}),
				src.MusicUnlockList.Select(delegate(int i)
				{
					UserItem result4 = default(UserItem);
					result4.itemKind = 5;
					result4.itemId = i;
					result4.stock = 1;
					result4.isValid = true;
					return result4;
				}),
				src.MusicUnlockMasterList.Select(delegate(int i)
				{
					UserItem result3 = default(UserItem);
					result3.itemKind = 6;
					result3.itemId = i;
					result3.stock = 1;
					result3.isValid = true;
					return result3;
				}),
				src.MusicUnlockReMasterList.Select(delegate(int i)
				{
					UserItem result2 = default(UserItem);
					result2.itemKind = 7;
					result2.itemId = i;
					result2.stock = 1;
					result2.isValid = true;
					return result2;
				}),
				src.MusicUnlockStrongList.Select(delegate(int i)
				{
					UserItem result = default(UserItem);
					result.itemKind = 8;
					result.itemId = i;
					result.stock = 1;
					result.isValid = true;
					return result;
				})
			}.SelectMany((IEnumerable<UserItem> i) => i).ToArray();
		}

		private static Manager.UserDatas.UserItem Convert(this UserItem src)
		{
			return new Manager.UserDatas.UserItem
			{
				itemId = src.itemId,
				isValid = src.isValid,
				stock = src.stock
			};
		}

		public static UserLoginBonus[] Export(this List<global::UserLoginBonus> src)
		{
			return src.Select((global::UserLoginBonus m) => m.Convert()).ToArray();
		}

		private static UserLoginBonus Convert(this global::UserLoginBonus src)
		{
			UserLoginBonus result = default(UserLoginBonus);
			result.bonusId = src.ID;
			result.point = src.Point;
			result.isCurrent = src.IsCurrent;
			result.isComplete = src.IsComplete;
			return result;
		}

		public static UserMap[] Export(this List<UserMapData> src)
		{
			return src.Select((UserMapData m) => m.Convert()).ToArray();
		}

		private static UserMap Convert(this UserMapData src)
		{
			UserMap result = default(UserMap);
			result.mapId = src.ID;
			result.distance = src.Distance;
			result.isLock = src.IsLock;
			result.isClear = src.IsClear;
			result.isComplete = src.IsComplete;
			return result;
		}

		public static List<UserScore>[] ExportScoreList(this UserMusicDetail[] src)
		{
			List<UserScore>[] array = new List<UserScore>[6];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<UserScore>();
			}
			for (int j = 0; j < src.Length; j++)
			{
				UserMusicDetail src2 = src[j];
				array[(int)src2.level].Add(src2.Convert());
			}
			return array;
		}

		public static UserMusicDetail[] ExportScoreDetailList(this UserData src)
		{
			return src.ScoreList.SelectMany((List<UserScore> i, int idx) => i.Select((UserScore j) => j.Convert((MusicDifficultyID)(0 + idx)))).ToArray();
		}

		private static UserScore Convert(this UserMusicDetail src)
		{
			return new UserScore(src.musicId)
			{
				playcount = src.playCount,
				achivement = src.achievement,
				combo = src.comboStatus,
				sync = src.syncStatus,
				deluxscore = src.deluxscoreMax,
				scoreRank = src.scoreRank
			};
		}

		private static UserMusicDetail Convert(this UserScore src, MusicDifficultyID kind)
		{
			UserMusicDetail result = default(UserMusicDetail);
			result.musicId = src.id;
			result.level = kind;
			result.playCount = src.playcount;
			result.achievement = src.achivement;
			result.comboStatus = src.combo;
			result.syncStatus = src.sync;
			result.deluxscoreMax = src.deluxscore;
			result.scoreRank = src.scoreRank;
			return result;
		}

		public static Manager.UserDatas.UserOption Export(this UserOption src)
		{
			Manager.UserDatas.UserOption dst = new Manager.UserDatas.UserOption();
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		public static UserOption Export(this Manager.UserDatas.UserOption src)
		{
			UserOption dst = default(UserOption);
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		public static UserPlaylog ExportUserPlaylog(this UserData src, int index, int trackNo)
		{
			UserPlaylog result = default(UserPlaylog);
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(index, trackNo);
			int playerIgnoreNpcNum = Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum(trackNo);
			GameScoreList ghostScore = Singleton<GamePlayManager>.Instance.GetGhostScore(trackNo);
			VersionNo versionNo = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo;
			result.userId = 0uL;
			result.loginDate = TimeManager.GetUnixTime(src.Detail.LastLoginDate);
			result.playDate = TimeManager.GetLogDateString(gameScore.UnixTime);
			result.userPlayDate = TimeManager.GetDateString(gameScore.UnixTime);
			result.version = (int)versionNo.versionCode;
			result.playlogId = Singleton<NetDataManager>.Instance.GetLoginVO(index)?.loginId ?? 0;
			result.placeId = (int)Auth.LocationId;
			result.placeName = Auth.LocationName;
			result.type = gameScore.SessionInfo.notesData.notesType;
			result.musicId = gameScore.SessionInfo.musicId;
			result.level = gameScore.SessionInfo.difficulty;
			result.trackNo = gameScore.TrackNo;
			result.playerNum = playerIgnoreNpcNum;
			result.vsRank = (int)((playerIgnoreNpcNum != 1) ? gameScore.VsRank : 0);
			if (ghostScore == null || (!ghostScore.IsGhost() && !ghostScore.IsNpc() && !ghostScore.IsBoss()))
			{
				result.vsMode = 0;
				result.vsUserName = "";
				result.vsStatus = 0;
				result.vsUserRating = 0;
				result.vsUserAchievement = 0;
				result.vsUserGradeRank = 0;
			}
			else
			{
				if (ghostScore.IsGhost())
				{
					result.vsMode = 1;
					result.vsStatus = (Singleton<GamePlayManager>.Instance.GetVsGhostWin(index, trackNo) ? 1 : 2);
				}
				else if (ghostScore.IsNpc())
				{
					result.vsMode = 2;
					result.vsStatus = (Singleton<GamePlayManager>.Instance.IsMapNpcWin(index, trackNo) ? 1 : 2);
				}
				else
				{
					result.vsMode = 3;
					result.vsStatus = (Singleton<GamePlayManager>.Instance.GetVsGhostWin(index, trackNo) ? 1 : 2);
				}
				result.vsUserName = ghostScore.UserName;
				result.vsUserRating = (int)ghostScore.Rating();
				result.vsUserAchievement = (int)Singleton<GamePlayManager>.Instance.GetGhostAchivement(trackNo);
				result.vsUserGradeRank = (int)Manager.UserDatas.UserUdemae.GetRateToUdemaeID((int)ghostScore.ClassValue);
			}
			if (gameScore.Rival.Count >= 1)
			{
				result.playedUserId1 = (ulong)gameScore.Rival[0].RivalID;
				result.playedUserName1 = gameScore.Rival[0].RivalName;
				result.playedMusicLevel1 = gameScore.Rival[0].Difficuty;
			}
			if (gameScore.Rival.Count >= 2)
			{
				result.playedUserId2 = (ulong)gameScore.Rival[1].RivalID;
				result.playedUserName2 = gameScore.Rival[1].RivalName;
				result.playedMusicLevel2 = gameScore.Rival[1].Difficuty;
			}
			if (gameScore.Rival.Count >= 3)
			{
				result.playedUserId3 = (ulong)gameScore.Rival[2].RivalID;
				result.playedUserName3 = gameScore.Rival[2].RivalName;
				result.playedMusicLevel3 = gameScore.Rival[2].Difficuty;
			}
			result.characterId1 = gameScore.CharaSlot[0];
			result.characterId2 = gameScore.CharaSlot[1];
			result.characterId3 = gameScore.CharaSlot[2];
			result.characterId4 = gameScore.CharaSlot[3];
			result.characterId5 = gameScore.CharaSlot[4];
			result.characterLevel1 = (int)gameScore.CharaLevel[0];
			result.characterLevel2 = (int)gameScore.CharaLevel[1];
			result.characterLevel3 = (int)gameScore.CharaLevel[2];
			result.characterLevel4 = (int)gameScore.CharaLevel[3];
			result.characterLevel5 = (int)gameScore.CharaLevel[4];
			result.characterAwakening1 = (int)gameScore.CharaAwake[0];
			result.characterAwakening2 = (int)gameScore.CharaAwake[1];
			result.characterAwakening3 = (int)gameScore.CharaAwake[2];
			result.characterAwakening4 = (int)gameScore.CharaAwake[3];
			result.characterAwakening5 = (int)gameScore.CharaAwake[4];
			result.achievement = GameManager.ConvAchiveDecimalToInt(gameScore.GetAchivement());
			result.deluxscore = (int)gameScore.GetDeluxeScoreAll();
			result.scoreRank = (int)GameManager.GetClearRank(result.achievement);
			result.maxCombo = (int)gameScore.MaxCombo;
			result.totalCombo = (int)gameScore.TheoryCombo;
			result.maxSync = (int)gameScore.MaxChain;
			result.totalSync = (int)((playerIgnoreNpcNum != 1) ? gameScore.TheoryChain : 0);
			result.tapCriticalPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Critical);
			result.tapPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Perfect);
			result.tapGreat = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Great);
			result.tapGood = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Good);
			result.tapMiss = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Tap, NoteJudge.JudgeBox.Miss);
			result.holdCriticalPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Critical);
			result.holdPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Perfect);
			result.holdGreat = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Great);
			result.holdGood = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Good);
			result.holdMiss = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Hold, NoteJudge.JudgeBox.Miss);
			result.slideCriticalPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Critical);
			result.slidePerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Perfect);
			result.slideGreat = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Great);
			result.slideGood = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Good);
			result.slideMiss = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Slide, NoteJudge.JudgeBox.Miss);
			result.touchCriticalPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Critical);
			result.touchPerfect = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Perfect);
			result.touchGreat = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Great);
			result.touchGood = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Good);
			result.touchMiss = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Touch, NoteJudge.JudgeBox.Miss);
			result.breakCriticalPerfect = (int)gameScore.GetJudgeBreakNum(NoteJudge.JudgeBox.Critical);
			result.breakPerfect = (int)gameScore.GetJudgeBreakNum(NoteJudge.JudgeBox.Perfect);
			result.breakGreat = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Break, NoteJudge.JudgeBox.Great);
			result.breakGood = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Break, NoteJudge.JudgeBox.Good);
			result.breakMiss = (int)gameScore.GetJudgeNum(NoteScore.EScoreType.Break, NoteJudge.JudgeBox.Miss);
			result.isTap = result.tapPerfect + result.tapGreat + result.tapGood + result.tapMiss + result.tapCriticalPerfect != 0;
			result.isHold = result.holdPerfect + result.holdGreat + result.holdGood + result.holdMiss + result.holdCriticalPerfect != 0;
			result.isSlide = result.slidePerfect + result.slideGreat + result.slideGood + result.slideMiss + result.slideCriticalPerfect != 0;
			result.isTouch = result.touchPerfect + result.touchGreat + result.touchGood + result.touchMiss + result.touchCriticalPerfect != 0;
			result.isBreak = result.breakPerfect + result.breakGreat + result.breakGood + result.breakMiss + result.breakCriticalPerfect != 0;
			result.isCriticalDisp = gameScore.UserOption.DispJudge.IsCritical();
			result.isFastLateDisp = true;
			result.fastCount = (int)gameScore.Fast;
			result.lateCount = (int)gameScore.Late;
			result.isAchieveNewRecord = gameScore.PreAchivement < (decimal)GameManager.ConvAchiveDecimalToInt(gameScore.Achivement);
			result.isDeluxscoreNewRecord = gameScore.PreDxScore < gameScore.DxScore;
			result.comboStatus = (int)gameScore.ComboType;
			result.syncStatus = (int)gameScore.SyncType;
			result.isClear = gameScore.IsClear;
			result.beforeRating = (int)gameScore.PreMusicRate;
			result.afterRating = (int)gameScore.MusicRate;
			result.beforeGrade = (int)gameScore.PreDanRate;
			result.afterGrade = (int)gameScore.DanRate;
			result.afterGradeRank = (int)gameScore.Dan;
			result.beforeDeluxRating = (int)gameScore.PreRating();
			result.afterDeluxRating = (int)gameScore.Rating();
			result.isPlayTutorial = GameManager.TutorialPlayed != GameManager.TutorialEnum.NotPlay;
			result.isEventMode = GameManager.IsEventMode;
			result.isFreedomMode = GameManager.IsFreedomMode;
			result.playMode = 0;
			if (GameManager.IsFreedomMode)
			{
				result.playMode = 1;
			}
			if (GameManager.IsCourseMode)
			{
				result.playMode = 2;
			}
			result.isNewFree = src.UserType == UserData.UserIDType.New;
			result.extNum1 = (int)(gameScore.StartLife * 10000 + gameScore.Life);
			result.extNum2 = Singleton<CourseManager>.Instance.GetCourseData()?.GetID() ?? 0;
			return result;
		}

		public static void ExportIconData(ref UserPortrait dst, ulong userId, string fname)
		{
			dst.userId = userId;
			dst.divNumber = 0;
			dst.divLength = 0;
			dst.divData = null;
			dst.placeId = (int)Auth.LocationId;
			dst.clientId = AMDaemon.System.KeychipId.ShortValue;
			dst.uploadDate = TimeManager.GetNowDateString();
			dst.fileName = fname;
		}

		public static Manager.UserDatas.UserRating Export(this UserRating src)
		{
			return new Manager.UserDatas.UserRating
			{
				Rating = src.rating,
				RatingList = src.ratingList.Select((UserRate r) => r.Convert()).ToList(),
				NewRatingList = src.newRatingList.Select((UserRate r) => r.Convert()).ToList(),
				NextRatingList = src.nextRatingList.Select((UserRate r) => r.Convert()).ToList(),
				NextNewRatingList = src.nextNewRatingList.Select((UserRate r) => r.Convert()).ToList(),
				Udemae = src.udemae.Export()
			};
		}

		public static UserRating Export(this Manager.UserDatas.UserRating src)
		{
			UserRating result = default(UserRating);
			result.rating = src.Rating;
			result.ratingList = src.RatingList.Select((Manager.UserDatas.UserRate r) => r.Convert()).ToArray();
			result.newRatingList = src.NewRatingList.Select((Manager.UserDatas.UserRate r) => r.Convert()).ToArray();
			result.nextRatingList = src.NextRatingList.Select((Manager.UserDatas.UserRate r) => r.Convert()).ToArray();
			result.nextNewRatingList = src.NextNewRatingList.Select((Manager.UserDatas.UserRate r) => r.Convert()).ToArray();
			result.udemae = src.Udemae.Export();
			return result;
		}

		private static Manager.UserDatas.UserRate Convert(this UserRate src)
		{
			return new Manager.UserDatas.UserRate(src.musicId, src.level, src.achievement, src.romVersion);
		}

		private static UserRate Convert(this Manager.UserDatas.UserRate src)
		{
			UserRate dst = default(UserRate);
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		public static UserRating Clone(this UserRating src)
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, src);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return (UserRating)binaryFormatter.Deserialize(memoryStream);
		}

		public static List<Manager.UserDatas.UserRegion> Export(this UserRegion[] src)
		{
			return src.Select((UserRegion g) => g.Export()).ToList();
		}

		private static Manager.UserDatas.UserRegion Export(this UserRegion src)
		{
			Manager.UserDatas.UserRegion dst = new Manager.UserDatas.UserRegion();
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		public static UserRegion Export(this Manager.UserDatas.UserRegion src)
		{
			UserRegion dst = default(UserRegion);
			NetPacketUtil.CopyMember(ref dst, src);
			return dst;
		}

		private static Manager.UserDatas.UserScoreRanking Convert(this UserScoreRanking src)
		{
			return new Manager.UserDatas.UserScoreRanking
			{
				tournamentId = src.tournamentId,
				totalScore = src.totalScore,
				ranking = src.ranking,
				rankingDate = src.rankingDate
			};
		}

		public static Manager.UserDatas.UserUdemae Export(this UserUdemae src)
		{
			Manager.UserDatas.UserUdemae dst = new Manager.UserDatas.UserUdemae();
			NetPacketUtil.CopyMember(ref dst, src);
			dst.SetClassValue(src.classValue);
			dst.SetMaxClassValue(src.maxClassValue);
			return dst;
		}

		public static UserUdemae Export(this Manager.UserDatas.UserUdemae src)
		{
			UserUdemae dst = default(UserUdemae);
			NetPacketUtil.CopyMember(ref dst, src);
			dst.classValue = src.ClassValue;
			dst.maxClassValue = src.MaxClassValue;
			return dst;
		}
	}
}
