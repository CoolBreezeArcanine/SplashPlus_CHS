namespace DB
{
	public enum CommonMessageID
	{
		Reset = 0,
		Music_SetActive = 1,
		Music_SetData = 2,
		Music_SetGameData = 3,
		User_SetActive = 4,
		User_SetData = 5,
		User_SetTeamData = 6,
		Chara_SetActive = 7,
		Chara_SetSlot = 8,
		Chara_SetTeamSlot = 9,
		Track_SetActive = 10,
		Track_SetTrackNum = 11,
		Classic_SET = 12,
		GameMode_Set = 13,
		CreditMain = 14,
		CreditSub = 15,
		ReplaceUserIcon = 16,
		Entry_InfoAime = 17,
		Entry_InfoCoin = 18,
		Entry_InfoButton = 19,
		Entry_InfoNotes = 20,
		Entry_InfoMoreCoin = 21,
		Entry_OnePlayer = 22,
		Entry_TwoPlayer = 23,
		Entry_Freedom = 24,
		Entry_TimeSkip = 25,
		Tutorial_Long_Intro_01 = 26,
		Turotial_Tap_01 = 27,
		Turotial_Tap_02 = 28,
		Turotial_Tap_03 = 29,
		Turotial_Tap_04 = 30,
		Tutorial_LetsPlay = 31,
		Turotial_Each_01 = 32,
		Turotial_Each_02 = 33,
		Turotial_Hold_01 = 34,
		Turotial_Hold_02 = 35,
		Turotial_Slide_01 = 36,
		Turotial_Slide_02 = 37,
		Turotial_Slide_03 = 38,
		Turotial_Slide_04 = 39,
		Turotial_TouchTap_01 = 40,
		Turotial_TouchTap_02 = 41,
		Turotial_TouchHold_01 = 42,
		Turotial_TouchHold_02 = 43,
		Tutorial_End_01 = 44,
		Tutorial_End_02 = 45,
		Tutorial_Short_Intro_01 = 46,
		Tutorial_Ex_01 = 47,
		Tutorial_NewHold_02 = 48,
		NewUserName = 49,
		GuestUserName = 50,
		DefaultUserName = 51,
		Scroll_Map_Select = 52,
		Scroll_Character_Select = 53,
		Scroll_Category_Select = 54,
		Scroll_Music_Select = 55,
		Scroll_Category_Sort_Setting = 56,
		Scroll_Level_Select_Normal = 57,
		Scroll_Level_Select_Otomodachi = 58,
		Scroll_Play_Setting = 59,
		Scroll_Option = 60,
		Scroll_Collection_Top = 61,
		Scroll_Collection_Icon = 62,
		Scroll_Collection_Nameplate = 63,
		Scroll_Collection_Title = 64,
		Scroll_Collection_Partner = 65,
		Scroll_Collection_Frame = 66,
		UnderServerMaintenance = 67,
		AimeOffline = 68,
		MapSelect_DecisionSerif = 69,
		MapSelect_CloseMap = 70,
		MapSelect_CloseReplaceName = 71,
		MapSelect_UnknownMapName = 72,
		MapSelect_To = 73,
		MusicSelectConnectNG_Freedom = 74,
		MusicSelectConnectNG_Ghost = 75,
		MusicSelectConnectNG_Single = 76,
		MusicSelectVsUpperDifficulty = 77,
		MusicSelectVsLowerDifficulty = 78,
		CodeReadBoostDate = 79,
		CodeReadBoostDateAt = 80,
		CodeReadBoostOutOfDate = 81,
		CodeReadNotUsed = 82,
		CodeReadThisCardUse = 83,
		CodeReadPromoCodeUnmatch = 84,
		CodeReadNotBuy = 85,
		CodeReadAllreadyRelease = 86,
		ResultKumaMessage01 = 87,
		ResultKumaMessage02 = 88,
		ResultKumaMessage03 = 89,
		ResultKumaMessage04 = 90,
		ResultKumaMessage05 = 91,
		CollectionNum = 92,
		CollectionTotalNum = 93,
		Entry_OldSrvDisconnect = 94,
		MusicSelectGhostPlayTime = 95,
		MusicSelectGhostPllayDefaultTime = 96,
		MusicSelectGhostInfo = 97,
		MusicSelectOptionMenuSpeed = 98,
		MusicSelectOptionMenuMirror = 99,
		MusicSelectOptionMenuTrackSkip = 100,
		CommonCredits = 101,
		CommonFreePlay = 102,
		SystemGood = 103,
		SystemBad = 104,
		SystemCheck = 105,
		SystemNa = 106,
		SystemWarn = 107,
		SystemDupli = 108,
		PlInfomationLeft = 109,
		PlInfomationRight = 110,
		GetWindowMusic = 111,
		GetWindowMusicUnlock = 112,
		GetWindowMusicTrans = 113,
		GetWindowChara = 114,
		GetWindowGhost = 115,
		GetWindowGhostTitle = 116,
		GetWindowIsland = 117,
		GetWindowCollection = 118,
		CharaSetNormal = 119,
		CharaSetNewCommer = 120,
		Entry_QRMessage = 121,
		Entry_AccessCodeMessage = 122,
		Entry_AccessCodeMessage1 = 123,
		ErrorIDTitle = 124,
		ErrorMessageTitle = 125,
		ErrorDateTitle = 126,
		MusicSelectEnableMatch = 127,
		MusicSelectDisableMatch = 128,
		MusicSelectMatchEntry = 129,
		MusicSelectMatchSetup = 130,
		MusicSelectMatchSet = 131,
		MusicSelectOptionBasic = 132,
		MusicSelectOptionAdvanced = 133,
		MusicSelectOptionExpert = 134,
		MusicSelectOptionCustom = 135,
		MusicSelectOptionBasicInfo = 136,
		MusicSelectOptionAdvancedInfo = 137,
		MusicSelectOptionExpertInfo = 138,
		MusicSelectOptionCustomInfo = 139,
		MusicSelectOptionVol = 140,
		PhotoEditTouchZoom = 141,
		SimpleSettingStartThisData = 142,
		SimpleSettingHeadphoneVol = 143,
		SimpleSettingCheckPreview = 144,
		SimpleSettingCheckCamera = 145,
		TutorialSelectNew = 146,
		TutorialSelectBasic = 147,
		LoginBonusInfo = 148,
		TournamentInfo_1 = 149,
		TournamentInfo_2 = 150,
		TournamentInfo_3 = 151,
		ChallengeInfoAssignmentTitle = 152,
		ChallengeInfoAssignmentMessage01 = 153,
		ChallengeInfoAssignmentMessage02 = 154,
		ChallengeInfoPerfectTitle = 155,
		ChallengeInfoPerfectMessage01 = 156,
		ChallengeInfoPerfectMessage02 = 157,
		MapResultInfoChallenge01 = 158,
		MapResultInfoChallenge02 = 159,
		MapResultInfoChallenge02Replace = 160,
		MapResultInfoStageFailed = 161,
		MapResultInfoTask01 = 162,
		MapResultInfoTask02 = 163,
		TransmissionMusic = 164,
		RegionalSelectCountMusic = 165,
		RegionalSelectCountCollection = 166,
		RegionalSelectTotalDistancce = 167,
		RegionalSelectEvent = 168,
		ExtraInfoBoss = 169,
		ExtraInfoSpecialBoss = 170,
		GhostJumpText = 171,
		GhostReturnText = 172,
		UnlockMusicTransmission = 173,
		UnlockMusicScoreRanking = 174,
		UnlockMusicCollection = 175,
		LoginBonusStmp = 176,
		LoginBonusCharacterSelect = 177,
		MusicSelectConnectNG_Challenge = 178,
		MusicSelectConnectNG_Course = 179,
		Begin = 0,
		End = 180,
		Invalid = -1
	}
}
