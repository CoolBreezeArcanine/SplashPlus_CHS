namespace DB
{
	public enum WindowMessageID
	{
		EntryConfirmGuest = 0,
		EntryDisplayPleaseWait = 1,
		EntryConfirmNewAime = 2,
		EntryConfirmNewUser = 3,
		EntryConfirmExistingAime = 4,
		EntryConfirmInheritAime = 5,
		EntryConfirmNewAimeSp = 6,
		EntryConfirmExistingAimeEvent = 7,
		EntryConfirmExistingAimeOldUser = 8,
		EntryWaitPartner = 9,
		EntryNoticeFelicaRegistration = 10,
		EntryConfirmFelicaSite = 11,
		EntryConfirmAccessCode = 12,
		EntryErrorAimeUnknown = 13,
		EntryErrorAimeLogin = 14,
		EntryErrorAimeEventNew = 15,
		EntryErrorAimeEventInherit = 16,
		EntryErrorAimeVersion = 17,
		EntryErrorAimeVersionInherit = 18,
		EntryErrorAimeNetwork = 19,
		EntryErrorAimeFatal = 20,
		EntryErrorAimeOverlap = 21,
		EntryErrorOldServerDown = 22,
		EntryDoneEntryTwoPlayer = 23,
		EntryDoneEntryTwoPlayerNew = 24,
		EntryDoneEntryDailyBonus = 25,
		EntryDoneEntryWeekdayBonus = 26,
		EntryErrorAccessCodeRead = 27,
		EntryErrorAccessCodeRegistration = 28,
		PhotoNotRegistNet = 29,
		PhotoUploadConfirm = 30,
		PhotoUploadContract = 31,
		PhotoUploadDone = 32,
		NameEntryDescription = 33,
		NameEntryConfirm = 34,
		NameEntryNgwordInfo = 35,
		NameEntryNotEnteredInfo = 36,
		NameEntryWelcomeInfo = 37,
		NameEntryTimeupInfo01 = 38,
		NameEntryTimeUpInfo02 = 39,
		NameEntryTimeUpInfo03 = 40,
		PlayPreparationWait = 41,
		PlayPreparationCancel = 42,
		FreedomModeTimeUp = 43,
		CollectionAttentionMaxFavorite = 44,
		CollectionAttentionEmptyFavorite = 45,
		CollectionCategorySelectAnnounce = 46,
		CollectionSelectAnnounce = 47,
		CollectionGotoFavorite = 48,
		CollectionGotoCollectionCustom = 49,
		CollectionGetAnnounceInfo = 50,
		GhostDifficultyInfomation = 51,
		NetworkErrorToGuest = 52,
		TutorialSelectInfo = 53,
		TutorialEnter = 54,
		TutorialExit = 55,
		NextTrackTips01 = 56,
		NextTrackTips02 = 57,
		NextTrackTips03 = 58,
		NextTrackTips04 = 59,
		NextTrackTips05 = 60,
		NextTrackTips06 = 61,
		NextTrackTips07 = 62,
		NextTrackTips08 = 63,
		NextTrackTips09 = 64,
		NextTrackTips10 = 65,
		NextTrackTips11 = 66,
		NextTrackTips12 = 67,
		NextTrackTips13 = 68,
		NextTrackTips14 = 69,
		NextTrackTips15 = 70,
		NextTrackTips16 = 71,
		NextTrackTips17 = 72,
		NextTrackTips18 = 73,
		NextTrackTips19 = 74,
		NextTrackTips20 = 75,
		NextTrackTips21 = 76,
		NextTrackTips22 = 77,
		NextTrackTips23 = 78,
		NextTrackTips24 = 79,
		NextTrackTips25 = 80,
		NextTrackTips26 = 81,
		NextTrackTips27 = 82,
		NextTrackTips28 = 83,
		NextTrackTips29 = 84,
		NextTrackTips30 = 85,
		NextTrackTips31 = 86,
		NextTrackTips32 = 87,
		NextTrackTips33 = 88,
		NextTrackTips34 = 89,
		NextTrackTips35 = 90,
		NextTrackTips36 = 91,
		IconPhotoContract = 92,
		ClassicModeSelectMessage = 93,
		CharacterSelectAutoMessage = 94,
		CardReadFailedMessage = 95,
		CodeReadInsertCardMessage = 96,
		FreedomModeTerminationMessage = 97,
		DataSaveStart = 98,
		DataSaveError = 99,
		TakeOverInfo01 = 100,
		TakeOverInfo02 = 101,
		Warning = 102,
		CodeReadFirst = 103,
		CodeReadOutOfService = 104,
		CodeReadNotHave = 105,
		SimpleSettingFirst = 106,
		SinmpeSettingNewFrame = 107,
		MapSelectFirst = 108,
		CharaSelectFirst = 109,
		CharaSelectGoodBad = 110,
		CategorySelectFirst = 111,
		MusicSelectFirst = 112,
		PlayReadyFirst = 113,
		DxStandardFirst = 114,
		OtomodachiFirst = 115,
		RateCategoryFirst = 116,
		CharaAwakeFirst = 117,
		TotalResultFirst = 118,
		CollectionCustomFirst = 119,
		DxPassFirst = 120,
		FreedomFirst = 121,
		SortFirst = 122,
		AimeUseNotice = 123,
		MusicSelectCanceRecruit = 124,
		MusicSelectCancelConfirmHost = 125,
		MusicSelectCancelConfirmClient = 126,
		MusicSelectConfirmTrackStart = 127,
		MusicSelectForceTrackStart = 128,
		MusicSelectWaitToHost = 129,
		PhotoAgree = 130,
		NetworkErrorUpPlaylog = 131,
		NetworkError = 132,
		EntryError = 133,
		MapCounterStop = 134,
		BossAppearFirst = 135,
		BossStayHint = 136,
		TrackSkip3Second = 137,
		TrackSkip2Second = 138,
		TrackSkip1Second = 139,
		CharaLockInfo = 140,
		LoginBonusFirst01 = 141,
		LoginBonusFirst02 = 142,
		LoginBonusFirst03 = 143,
		CollectionPartnerFIrst = 144,
		TransferDx01 = 145,
		TransferDx02 = 146,
		TransferDx03 = 147,
		PassportCamera = 148,
		CharaSelectCut = 149,
		SpecialBossAppear = 150,
		PhotoShootCancel = 151,
		FBRFirst = 152,
		FBRClassPoint = 153,
		FBRLegend = 154,
		TicketSelectFirst = 155,
		ModeSelectSinRankAdd = 156,
		ModeSelectFirst = 157,
		TicketConnectServer = 158,
		TicketConnectFailed = 159,
		ModeSelectWhichOneTimeOut = 160,
		Begin = 0,
		End = 161,
		Invalid = -1
	}
}