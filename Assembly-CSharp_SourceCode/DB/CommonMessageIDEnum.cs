namespace DB
{
	public static class CommonMessageIDEnum
	{
		private static readonly CommonMessageTableRecord[] records;

		public static bool IsActive(this CommonMessageID self)
		{
			if (self >= CommonMessageID.Reset && self < CommonMessageID.End)
			{
				return self != CommonMessageID.Reset;
			}
			return false;
		}

		public static bool IsValid(this CommonMessageID self)
		{
			if (self >= CommonMessageID.Reset)
			{
				return self < CommonMessageID.End;
			}
			return false;
		}

		public static void Clamp(this CommonMessageID self)
		{
			if (self < CommonMessageID.Reset)
			{
				self = CommonMessageID.Reset;
			}
			else if ((int)self >= GetEnd())
			{
				self = (CommonMessageID)GetEnd();
			}
		}

		public static int GetEnd(this CommonMessageID self)
		{
			return GetEnd();
		}

		public static CommonMessageID FindID(string enumName)
		{
			for (CommonMessageID commonMessageID = CommonMessageID.Reset; commonMessageID < CommonMessageID.End; commonMessageID++)
			{
				if (commonMessageID.GetEnumName() == enumName)
				{
					return commonMessageID;
				}
			}
			return CommonMessageID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this CommonMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this CommonMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this CommonMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this CommonMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		static CommonMessageIDEnum()
		{
			records = new CommonMessageTableRecord[180]
			{
				new CommonMessageTableRecord(0, "Reset", "上画面情報すべてをリセットし、非表示にする", ""),
				new CommonMessageTableRecord(1, "Music_SetActive", "楽曲情報の表示設定", ""),
				new CommonMessageTableRecord(2, "Music_SetData", "楽曲情報の設定", ""),
				new CommonMessageTableRecord(3, "Music_SetGameData", "ゲームスコアを設定する", ""),
				new CommonMessageTableRecord(4, "User_SetActive", "ユーザー情報の表示設定", ""),
				new CommonMessageTableRecord(5, "User_SetData", "ユーザー情報を設定する", ""),
				new CommonMessageTableRecord(6, "User_SetTeamData", "ユーザーチーム情報の設定", ""),
				new CommonMessageTableRecord(7, "Chara_SetActive", "キャラクターセットの表示設定", ""),
				new CommonMessageTableRecord(8, "Chara_SetSlot", "キャラクターセットのスロットに情報を設定する", ""),
				new CommonMessageTableRecord(9, "Chara_SetTeamSlot", "キャラクターセットのチームスロットに情報を設定する", ""),
				new CommonMessageTableRecord(10, "Track_SetActive", "トラック数表示設定", ""),
				new CommonMessageTableRecord(11, "Track_SetTrackNum", "トラック数の情報を設定する", ""),
				new CommonMessageTableRecord(12, "Classic_SET", "クラシックモードの表示設定", ""),
				new CommonMessageTableRecord(13, "GameMode_Set", "ゲームモード開始終了の通知", ""),
				new CommonMessageTableRecord(14, "CreditMain", "クレジット情報をメインモニターに表示", ""),
				new CommonMessageTableRecord(15, "CreditSub", "クレジット情報をサブモニターに表示", ""),
				new CommonMessageTableRecord(16, "ReplaceUserIcon", "ユーザーアイコンを適応する", ""),
				new CommonMessageTableRecord(17, "Entry_InfoAime", "请使用兼容Aime的卡或手机\n触碰读卡器", ""),
				new CommonMessageTableRecord(18, "Entry_InfoCoin", "请投币", ""),
				new CommonMessageTableRecord(19, "Entry_InfoButton", "请按按键", ""),
				new CommonMessageTableRecord(20, "Entry_InfoNotes", "请在游戏画面上操作", ""),
				new CommonMessageTableRecord(21, "Entry_InfoMoreCoin", "请继续投币", ""),
				new CommonMessageTableRecord(22, "Entry_OnePlayer", "开始单人模式", ""),
				new CommonMessageTableRecord(23, "Entry_TwoPlayer", "开始双人模式", ""),
				new CommonMessageTableRecord(24, "Entry_Freedom", "开始自由模式", ""),
				new CommonMessageTableRecord(25, "Entry_TimeSkip", "长按跳过倒计时键结束报名", ""),
				new CommonMessageTableRecord(26, "Tutorial_Long_Intro_01", "你～好～我是“迪拉熊”\r\n我来说明舞萌的游戏操作哦♪", ""),
				new CommonMessageTableRecord(27, "Turotial_Tap_01", "首先是<size=30><color=#FF5668FF>TAP音符！</color></size>", string.Empty),
				new CommonMessageTableRecord(28, "Turotial_Tap_02", "与判定线重合时要\r\n<size=30><color=#FF5668FF>轻拍按键或屏幕</color></size>！", string.Empty),
				new CommonMessageTableRecord(29, "Turotial_Tap_03", "有各种各样的TAP音符呢…", string.Empty),
				new CommonMessageTableRecord(30, "Turotial_Tap_04", "只要看准时机全都拍到就行啦！", string.Empty),
				new CommonMessageTableRecord(31, "Tutorial_LetsPlay", "<size=30>试试看吧～！</size>", string.Empty),
				new CommonMessageTableRecord(32, "Turotial_Each_01", "对了对了，\r\n有时会同时出现２个音符呢", string.Empty),
				new CommonMessageTableRecord(33, "Turotial_Each_02", "这时可以试着用双手\n同时拍击它们哟！", string.Empty),
				new CommonMessageTableRecord(34, "Turotial_Hold_01", "接下来是<size=30><color=#FF5668FF>HOLD音符！</color></size>", string.Empty),
				new CommonMessageTableRecord(35, "Turotial_Hold_02", "当这个又细又长的音符同判定线重合开始，\n<color=#FF5668FF><size=30>就要一直按着</color></size>，直到它消失为止！\r\n", string.Empty),
				new CommonMessageTableRecord(36, "Turotial_Slide_01", "接下来是<size=30><color=#FF5668FF>SLIDE音符！</color></size>", string.Empty),
				new CommonMessageTableRecord(37, "Turotial_Slide_02", "轻拍操作\r\n<size=30><color=#FF5668FF>和TAP音符是一样的</color></size>哟", string.Empty),
				new CommonMessageTableRecord(38, "Turotial_Slide_03", "等待一拍后跟随\r\n<size=30><color=blue>☆</color><color=#FF5668FF>的移动轨迹在屏幕上滑动</color></size>！", string.Empty),
				new CommonMessageTableRecord(39, "Turotial_Slide_04", "记得要先拍击<size=30><color=blue>☆</color><color=#FF5668FF>后再滑动</color></size>哦♪", string.Empty),
				new CommonMessageTableRecord(40, "Turotial_TouchTap_01", "接着是<size=30><color=#FF5668FF>TOUCH音符！</color></size>", string.Empty),
				new CommonMessageTableRecord(41, "Turotial_TouchTap_02", "在快门闭合的瞬间\r\n<size=30><color=#FF5668FF>触摸屏幕！</color></size>", string.Empty),
				new CommonMessageTableRecord(42, "Turotial_TouchHold_01", "最后！\n<size=30><color=#FF5668FF>TOUCH HOLD音符！</color></size>", string.Empty),
				new CommonMessageTableRecord(43, "Turotial_TouchHold_02", "在音符<size=30><color=#FF5668FF>消失前</color>\r\n<color=#FF5668FF>要一直按住不放</color></size>哦！", string.Empty),
				new CommonMessageTableRecord(44, "Tutorial_End_01", "音符的说明就到这里！\n<size=30>辛苦啦♪</size>", string.Empty),
				new CommonMessageTableRecord(45, "Tutorial_End_02", "那么，接下来就以通关为目标加油吧！\r\n<size=30>一路顺风♪</size>", string.Empty),
				new CommonMessageTableRecord(46, "Tutorial_Short_Intro_01", "你～好～我是“迪拉熊”\r\n我来告诉你舞萌的新游戏要素哦♪", string.Empty),
				new CommonMessageTableRecord(47, "Tutorial_Ex_01", "<size=30><color=#FF5668FF>增加了EX音符！</color></size>\r\n可以让GOOD判定也变成“完美”哦♪", string.Empty),
				new CommonMessageTableRecord(48, "Tutorial_NewHold_02", "<size=26><color=#FF5668FF>音符结束后一直按着也是没关系</color></size>哒！\r\n即便中途松开也可以重新按♪", string.Empty),
				new CommonMessageTableRecord(49, "NewUserName", "欢迎！", ""),
				new CommonMessageTableRecord(50, "GuestUserName", "游客", ""),
				new CommonMessageTableRecord(51, "DefaultUserName", "舞萌", ""),
				new CommonMessageTableRecord(52, "Scroll_Map_Select", "请选择要前往的区域   各区域都有独特的旅行伙伴、收藏品以及乐曲！试着来收集吧！", ""),
				new CommonMessageTableRecord(53, "Scroll_Character_Select", "选择组队的旅行伙伴   同“擅长”水准的旅行伙伴组队容易走得更远哦", ""),
				new CommonMessageTableRecord(54, "Scroll_Category_Select", "请选择想要搜索的乐曲种类\u3000\u3000\u3000↑上方的乐曲也包含在内哟\u3000\u3000\u3000通过按<color=#51bcf3><sprite name=left_arrow_w></color> <color=#ff4100><sprite name=right_arrow_w></color>键可以更改种类・排序的设定哟", ""),
				new CommonMessageTableRecord(55, "Scroll_Music_Select", "请选择想要游玩的乐曲\u3000\u3000\u3000通过按<color=#51bcf3><sprite name=left_arrow_w></color> <color=#ff4100><sprite name=right_arrow_w></color>键可以更改种类･排序的设定哟\u3000\u3000\u3000同时按下左右偏上位置的按钮的话就可以在“DX乐谱”和“标准乐谱”之间切换哦", ""),
				new CommonMessageTableRecord(56, "Scroll_Category_Sort_Setting", "试着按照想搜索的乐曲来设定下种类和排序吧", ""),
				new CommonMessageTableRecord(57, "Scroll_Level_Select_Normal", "请选择难度   想更换乐曲时，按返回键就可以哦", ""),
				new CommonMessageTableRecord(58, "Scroll_Level_Select_Otomodachi", "请选择难度   选择比友人对战的对手更低的难度时，将无法进行友人对战哦", ""),
				new CommonMessageTableRecord(59, "Scroll_Play_Setting", "选项设定好了吗？   耳机音量也可以调整哦   都准备好的话按下“乐曲开始”按键吧", ""),
				new CommonMessageTableRecord(60, "Scroll_Option", "首先确认速度！   可以边看演示边设定各种选项", ""),
				new CommonMessageTableRecord(61, "Scroll_Collection_Top", "请选择想要设置的收藏品   本次新获得的收藏品上会标有“NEW”哟", ""),
				new CommonMessageTableRecord(62, "Scroll_Collection_Icon", "请选择想要设置的头像   设置后可以在上方画面进行确认！", ""),
				new CommonMessageTableRecord(63, "Scroll_Collection_Nameplate", "请选择想要设置的姓名框   设置后可以在上方画面进行确认！", ""),
				new CommonMessageTableRecord(64, "Scroll_Collection_Title", "请选择想要设置的称号   设置后可以在上方画面进行确认！", ""),
				new CommonMessageTableRecord(65, "Scroll_Collection_Partner", "请选择想要设置的伙伴", ""),
				new CommonMessageTableRecord(66, "Scroll_Collection_Frame", "请选择想要设置的边框    设置后可以在上方画面进行确认！", ""),
				new CommonMessageTableRecord(67, "UnderServerMaintenance", "因系统维护，无法进行游戏", ""),
				new CommonMessageTableRecord(68, "AimeOffline", "网络不佳，目前无法进行会员登陆\r\n只能通过游客模式参加游戏", ""),
				new CommonMessageTableRecord(69, "MapSelect_DecisionSerif", "就这么决定喽", ""),
				new CommonMessageTableRecord(70, "MapSelect_CloseMap", "只要点进[ちほー名]\r\n就可以了哟！", ""),
				new CommonMessageTableRecord(71, "MapSelect_CloseReplaceName", "[ちほー名]", ""),
				new CommonMessageTableRecord(72, "MapSelect_UnknownMapName", "？？？？区域", ""),
				new CommonMessageTableRecord(73, "MapSelect_To", "  まで", ""),
				new CommonMessageTableRecord(74, "MusicSelectConnectNG_Freedom", "自由模式乐曲\r\n无法进行店内招募", ""),
				new CommonMessageTableRecord(75, "MusicSelectConnectNG_Ghost", "友人对战乐曲\r\n无法进行店内招募", ""),
				new CommonMessageTableRecord(76, "MusicSelectConnectNG_Single", "根据该乐曲设定\r\n无法进行店内招募", ""),
				new CommonMessageTableRecord(77, "MusicSelectVsUpperDifficulty", "这个难度对你来说有点难哦", ""),
				new CommonMessageTableRecord(78, "MusicSelectVsLowerDifficulty", "这个难度对你来说太简单，无法体现你的技术水准", ""),
				new CommonMessageTableRecord(79, "CodeReadBoostDate", "ブースト期限", ""),
				new CommonMessageTableRecord(80, "CodeReadBoostDateAt", "まで", ""),
				new CommonMessageTableRecord(81, "CodeReadBoostOutOfDate", "这张卡已过期", ""),
				new CommonMessageTableRecord(82, "CodeReadNotUsed", "这张卡无法使用", ""),
				new CommonMessageTableRecord(83, "CodeReadThisCardUse", "使用这张卡吗？", ""),
				new CommonMessageTableRecord(84, "CodeReadPromoCodeUnmatch", "促销码不匹配", ""),
				new CommonMessageTableRecord(85, "CodeReadNotBuy", "这张卡尚未购买", ""),
				new CommonMessageTableRecord(86, "CodeReadAllreadyRelease", "すでに解放されています", ""),
				new CommonMessageTableRecord(87, "ResultKumaMessage01", "恭喜通关！", ""),
				new CommonMessageTableRecord(88, "ResultKumaMessage02", "真厉害啊！", ""),
				new CommonMessageTableRecord(89, "ResultKumaMessage03", "太棒啦！", ""),
				new CommonMessageTableRecord(90, "ResultKumaMessage04", "恭喜你～♪", ""),
				new CommonMessageTableRecord(91, "ResultKumaMessage05", "干得不错哦♪", ""),
				new CommonMessageTableRecord(92, "CollectionNum", "获得数", ""),
				new CommonMessageTableRecord(93, "CollectionTotalNum", "总获得数", ""),
				new CommonMessageTableRecord(94, "Entry_OldSrvDisconnect", "目前无法引继或注册Aime", ""),
				new CommonMessageTableRecord(95, "MusicSelectGhostPlayTime", "游戏日期", ""),
				new CommonMessageTableRecord(96, "MusicSelectGhostPllayDefaultTime", "----/--/-- --:--", ""),
				new CommonMessageTableRecord(97, "MusicSelectGhostInfo", "与全国各地的朋友对战！", ""),
				new CommonMessageTableRecord(98, "MusicSelectOptionMenuSpeed", "速度：", ""),
				new CommonMessageTableRecord(99, "MusicSelectOptionMenuMirror", "镜像：", ""),
				new CommonMessageTableRecord(100, "MusicSelectOptionMenuTrackSkip", "跳过乐曲：", ""),
				new CommonMessageTableRecord(101, "CommonCredits", "CREDIT(S)  ", ""),
				new CommonMessageTableRecord(102, "CommonFreePlay", "免费游玩", ""),
				new CommonMessageTableRecord(103, "SystemGood", "良", ""),
				new CommonMessageTableRecord(104, "SystemBad", "坏", ""),
				new CommonMessageTableRecord(105, "SystemCheck", "检查", ""),
				new CommonMessageTableRecord(106, "SystemNa", "不适用", ""),
				new CommonMessageTableRecord(107, "SystemWarn", "警告", ""),
				new CommonMessageTableRecord(108, "SystemDupli", "(重复)", ""),
				new CommonMessageTableRecord(109, "PlInfomationLeft", "不要拍得太用力哦", ""),
				new CommonMessageTableRecord(110, "PlInfomationRight", "不要滑得太用力哦", ""),
				new CommonMessageTableRecord(111, "GetWindowMusic", "您现在可以游玩新的乐曲！", ""),
				new CommonMessageTableRecord(112, "GetWindowMusicUnlock", "解开新的乐曲！", ""),
				new CommonMessageTableRecord(113, "GetWindowMusicTrans", "楽曲伝導！遊べるようになりました！", ""),
				new CommonMessageTableRecord(114, "GetWindowChara", "有新的旅行伙伴加入！", ""),
				new CommonMessageTableRecord(115, "GetWindowGhost", "※它可能会在一段时间后消失", ""),
				new CommonMessageTableRecord(116, "GetWindowGhostTitle", "一緒に遊ぶとちほーがたくさん進むよ！", ""),
				new CommonMessageTableRecord(117, "GetWindowIsland", "有新的区域可到达！请稍后查看哦！", ""),
				new CommonMessageTableRecord(118, "GetWindowCollection", "获得了新的收藏品！", ""),
				new CommonMessageTableRecord(119, "CharaSetNormal", "触摸设置要替换的旅行伙伴", ""),
				new CommonMessageTableRecord(120, "CharaSetNewCommer", "触摸设置新成员的位置", ""),
				new CommonMessageTableRecord(121, "Entry_QRMessage", "请通过二维码在Aime官网上注册", ""),
				new CommonMessageTableRecord(122, "Entry_AccessCodeMessage", "请通过二维码在Aime官网上注册", ""),
				new CommonMessageTableRecord(123, "Entry_AccessCodeMessage1", "请注意不要将二维码告诉他人", ""),
				new CommonMessageTableRecord(124, "ErrorIDTitle", "1.错误编号", ""),
				new CommonMessageTableRecord(125, "ErrorMessageTitle", "2.错误名称", ""),
				new CommonMessageTableRecord(126, "ErrorDateTitle", "3.发生日期", ""),
				new CommonMessageTableRecord(127, "MusicSelectEnableMatch", "可以招募!", ""),
				new CommonMessageTableRecord(128, "MusicSelectDisableMatch", "根据个人设定\r\n无法进行店内招募", ""),
				new CommonMessageTableRecord(129, "MusicSelectMatchEntry", "招募中", ""),
				new CommonMessageTableRecord(130, "MusicSelectMatchSetup", "准备中", ""),
				new CommonMessageTableRecord(131, "MusicSelectMatchSet", "准备好了!", ""),
				new CommonMessageTableRecord(132, "MusicSelectOptionBasic", "适合初级玩家的设定", ""),
				new CommonMessageTableRecord(133, "MusicSelectOptionAdvanced", "适合中级玩家的设定", ""),
				new CommonMessageTableRecord(134, "MusicSelectOptionExpert", "适合高级玩家的设定", ""),
				new CommonMessageTableRecord(135, "MusicSelectOptionCustom", "详细设定", ""),
				new CommonMessageTableRecord(136, "MusicSelectOptionBasicInfo", "适合BASIC难度的设定", ""),
				new CommonMessageTableRecord(137, "MusicSelectOptionAdvancedInfo", "适合ADVANCED难度的设定", ""),
				new CommonMessageTableRecord(138, "MusicSelectOptionExpertInfo", "适合EXPERT难度的设定", ""),
				new CommonMessageTableRecord(139, "MusicSelectOptionCustomInfo", "详细设置每个选项", ""),
				new CommonMessageTableRecord(140, "MusicSelectOptionVol", "请使用「按钮＋」「按钮-」进行调整", ""),
				new CommonMessageTableRecord(141, "PhotoEditTouchZoom", "触摸屏幕退出放大", ""),
				new CommonMessageTableRecord(142, "SimpleSettingStartThisData", "从这个数据开始", ""),
				new CommonMessageTableRecord(143, "SimpleSettingHeadphoneVol", "耳机音量设定", ""),
				new CommonMessageTableRecord(144, "SimpleSettingCheckPreview", "在预览中检查画面的位置！ 如果按下拍摄按钮，将在3秒内拍摄。", ""),
				new CommonMessageTableRecord(145, "SimpleSettingCheckCamera", "请看着摄像机并摆好姿势！", ""),
				new CommonMessageTableRecord(146, "TutorialSelectNew", "适合第一次玩的人！", ""),
				new CommonMessageTableRecord(147, "TutorialSelectBasic", "适合玩过的人！", ""),
				new CommonMessageTableRecord(148, "LoginBonusInfo", "パートナーをゲットしました\nコレクションカスタムでセットできるよ！", ""),
				new CommonMessageTableRecord(149, "TournamentInfo_1", "线上排位赛正在举行！！\r", ""),
				new CommonMessageTableRecord(150, "TournamentInfo_2", "地区锦标赛正在举行！！\r", ""),
				new CommonMessageTableRecord(151, "TournamentInfo_3", "顶级决战正在打响！！\r", ""),
				new CommonMessageTableRecord(152, "ChallengeInfoAssignmentTitle", "課題曲出現！", ""),
				new CommonMessageTableRecord(153, "ChallengeInfoAssignmentMessage01", "課題曲に挑戦して楽曲をGETしよう！", ""),
				new CommonMessageTableRecord(154, "ChallengeInfoAssignmentMessage02", "課題曲をクリアしないとちほーの進行はできません", ""),
				new CommonMessageTableRecord(155, "ChallengeInfoPerfectTitle", "パーフェクトチャレンジ出現", ""),
				new CommonMessageTableRecord(156, "ChallengeInfoPerfectMessage01", "パーフェクトチャレンジに挑戦して楽曲をGETしよう！", ""),
				new CommonMessageTableRecord(157, "ChallengeInfoPerfectMessage02", "パーフェクトチャレンジ楽曲はライフが設定され\r\nプレイの結果によっては楽曲が途中で終了します", ""),
				new CommonMessageTableRecord(158, "MapResultInfoChallenge01", "パーフェクトチャレンジをクリアしよう！", ""),
				new CommonMessageTableRecord(159, "MapResultInfoChallenge02", "ルールがやさしくなるまで\u3000あと[残り]日", ""),
				new CommonMessageTableRecord(160, "MapResultInfoChallenge02Replace", "[残り]", ""),
				new CommonMessageTableRecord(161, "MapResultInfoStageFailed", "ちほーを進むにはパーフェクトチャレンジをクリアする必要があります", ""),
				new CommonMessageTableRecord(162, "MapResultInfoTask01", "課題曲をクリアしよう！", ""),
				new CommonMessageTableRecord(163, "MapResultInfoTask02", "ちほーを進むには課題曲をクリアする必要があります", ""),
				new CommonMessageTableRecord(164, "TransmissionMusic", "一緒に伝導楽曲を遊んで楽曲を伝導しよう！", ""),
				new CommonMessageTableRecord(165, "RegionalSelectCountMusic", "楽曲獲得まであと", ""),
				new CommonMessageTableRecord(166, "RegionalSelectCountCollection", "コレクション獲得まであと", ""),
				new CommonMessageTableRecord(167, "RegionalSelectTotalDistancce", "ちほー累計距離", ""),
				new CommonMessageTableRecord(168, "RegionalSelectEvent", "イベント開催中！", ""),
				new CommonMessageTableRecord(169, "ExtraInfoBoss", "譜面ボーイズと勝負！", ""),
				new CommonMessageTableRecord(170, "ExtraInfoSpecialBoss", "ボスに勝利して伝導楽曲をゲット！", ""),
				new CommonMessageTableRecord(171, "GhostJumpText", "オトモダチ対戦\r\n対象楽曲", ""),
				new CommonMessageTableRecord(172, "GhostReturnText", "通常楽曲選択に\r\n戻るよ！", ""),
				new CommonMessageTableRecord(173, "UnlockMusicTransmission", "楽曲伝導！遊べるようになりました！", ""),
				new CommonMessageTableRecord(174, "UnlockMusicScoreRanking", "大会専用楽曲を獲得しました！", ""),
				new CommonMessageTableRecord(175, "UnlockMusicCollection", "楽曲を獲得しました！", ""),
				new CommonMessageTableRecord(176, "LoginBonusStmp", "1日1回スタンプを押すよ\r\n10個集めてパートナーGET！", ""),
				new CommonMessageTableRecord(177, "LoginBonusCharacterSelect", "ゲットしたいパートナーの [すたんぷカード] を選ぼう！\r\n<color=#ff705d><size=14>※選んだ [すたんぷカード] は10個集まるまで変えられません</size></color>", ""),
				new CommonMessageTableRecord(178, "MusicSelectConnectNG_Challenge", "パーフェクトチャレンジ楽曲のため店内マッチングはできません。", ""),
				new CommonMessageTableRecord(179, "MusicSelectConnectNG_Course", "段位認定のため店内マッチングはできません。", "")
			};
		}
	}
}
