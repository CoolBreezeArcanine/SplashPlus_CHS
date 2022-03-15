namespace DB
{
	public static class WindowMessageIDEnum
	{
		private static readonly WindowMessageTableRecord[] records;

		public static bool IsActive(this WindowMessageID self)
		{
			if (self >= WindowMessageID.EntryConfirmGuest && self < WindowMessageID.End)
			{
				return self != WindowMessageID.EntryConfirmGuest;
			}
			return false;
		}

		public static bool IsValid(this WindowMessageID self)
		{
			if (self >= WindowMessageID.EntryConfirmGuest)
			{
				return self < WindowMessageID.End;
			}
			return false;
		}

		public static void Clamp(this WindowMessageID self)
		{
			if (self < WindowMessageID.EntryConfirmGuest)
			{
				self = WindowMessageID.EntryConfirmGuest;
			}
			else if ((int)self >= GetEnd())
			{
				self = (WindowMessageID)GetEnd();
			}
		}

		public static int GetEnd(this WindowMessageID self)
		{
			return GetEnd();
		}

		public static WindowMessageID FindID(string enumName)
		{
			for (WindowMessageID windowMessageID = WindowMessageID.EntryConfirmGuest; windowMessageID < WindowMessageID.End; windowMessageID++)
			{
				if (windowMessageID.GetEnumName() == enumName)
				{
					return windowMessageID;
				}
			}
			return WindowMessageID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static WindowKindID GetKind(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Kind;
			}
			return WindowKindID.Invalid;
		}

		public static WindowPositionID GetPosition(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Position;
			}
			return WindowPositionID.Invalid;
		}

		public static WindowSizeID GetSize(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Size;
			}
			return WindowSizeID.Invalid;
		}

		public static string GetTitle(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Title;
			}
			return "";
		}

		public static string GetTitleEx(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].TitleEx;
			}
			return "";
		}

		public static string GetName(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static string GetNameEx(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].NameEx;
			}
			return "";
		}

		public static int GetLifetime(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Lifetime;
			}
			return 0;
		}

		public static string GetFileName(this WindowMessageID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].FileName;
			}
			return "";
		}

		static WindowMessageIDEnum()
		{
			records = new WindowMessageTableRecord[161]
			{
				new WindowMessageTableRecord(0, "EntryConfirmGuest", 0, 1, 3, "通知信息", "", "不使用账号进行游戏！可以吗？\r\n\r\n游戏的通关进度等信息都将无法保存", "", -1, ""),
				new WindowMessageTableRecord(1, "EntryDisplayPleaseWait", 0, 1, 4, "请稍候", "", "相邻玩家正在操作", "", -1, ""),
				new WindowMessageTableRecord(2, "EntryConfirmNewAime", 0, 1, 0, "注册Aime吗？", "", "要以Aime游玩，需要在此卡或手机上注册Aime（免费）\r\n※无法将已使用的Aime卡或电话号码的数据转移至该卡。", "", -1, ""),
				new WindowMessageTableRecord(3, "EntryConfirmNewUser", 0, 1, 3, "使用新账号进行游戏吗？", "", "可以观看教程并免费玩１首曲子哦！\n而且<color=#FF4000>双人游戏</color>的话<color=#FF4000>可以免费玩４首曲子哦</color>哦！", "", -1, ""),
				new WindowMessageTableRecord(4, "EntryConfirmExistingAime", 0, 0, 4, "通知信息", "", "用这张Aime卡开始游戏吗？", "", -1, ""),
				new WindowMessageTableRecord(5, "EntryConfirmInheritAime", 0, 0, 3, "用这个账号开始游戏吗？", "", "将当前的数据引继至maimai DX中\n\n※如果你在引继后继续玩maimai FiNALE，更新的数据将不会反映在maimai DX中", "", -1, ""),
				new WindowMessageTableRecord(6, "EntryConfirmNewAimeSp", 0, 1, 3, "用这个账号开始游戏吗？", "", "此账号在上次游戏后没有正常登出\r\n要使用游戏点数开始新的游戏吗？", "", -1, ""),
				new WindowMessageTableRecord(7, "EntryConfirmExistingAimeEvent", 0, 0, 4, "用这个账号开始游戏吗？", "", "由于目前处在大会模式，无法保存游戏结果", "", -1, ""),
				new WindowMessageTableRecord(8, "EntryConfirmExistingAimeOldUser", 1, 0, 4, "用这个账号开始游戏吗？", "", "由于版本不同，现在将数据继承至新版本\r\n※数据继承完成后，将无法在之前的版本游戏", "", -1, ""),
				new WindowMessageTableRecord(9, "EntryWaitPartner", 0, 1, 4, "正在等待其他玩家", "", "长按跳过倒计时键结束报名", "", -1, ""),
				new WindowMessageTableRecord(10, "EntryNoticeFelicaRegistration", 0, 1, 4, "Aime注册完成", "", "这台电话可以当做Aime使用\u3000（翻訳抜け）", "", -1, ""),
				new WindowMessageTableRecord(11, "EntryConfirmFelicaSite", 0, 1, 3, "要在Aime官网注册吗？", "", "通过在Aime官网上注册，您可以接管数据，\n以防出现型号更改、丢失或故障等问题。", "", -1, ""),
				new WindowMessageTableRecord(12, "EntryConfirmAccessCode", 0, 1, 4, "查询卡号", "", "将兼容Aime的卡或手机与Aime读卡器接触", "", -1, ""),
				new WindowMessageTableRecord(13, "EntryErrorAimeUnknown", 0, 1, 4, "读取失败", "", "请重新读取", "", -1, ""),
				new WindowMessageTableRecord(14, "EntryErrorAimeLogin", 0, 1, 4, "该账号没有登出", "", "当前使用的账号没有正常登出\r\n请过15分钟后再试", "", -1, ""),
				new WindowMessageTableRecord(15, "EntryErrorAimeEventNew", 0, 1, 4, "需要登录账号", "", "由于目前处在大会模式，无法使用新账号登录", "", -1, ""),
				new WindowMessageTableRecord(16, "EntryErrorAimeEventInherit", 0, 1, 4, "需要进行数据继承的账号", "", "由于目前处在大会模式，无法进行数据继承", "", -1, ""),
				new WindowMessageTableRecord(17, "EntryErrorAimeVersion", 0, 1, 4, "账号的游戏版本不同", "", "在当前的版本中无法使用该账号进行游戏", "", -1, ""),
				new WindowMessageTableRecord(18, "EntryErrorAimeVersionInherit", 0, 1, 4, "该账号无法进行数据继承", "", "只有使用Aime玩过maimai FiNALE的卡可以数据继承", "", -1, ""),
				new WindowMessageTableRecord(19, "EntryErrorAimeNetwork", 0, 1, 4, "无法连接网络", "", "请重试", "", -1, ""),
				new WindowMessageTableRecord(20, "EntryErrorAimeFatal", 0, 1, 4, "无法使用当前账号游戏", "", "由于读卡器或机器的原因，无法读取该账号数据", "", -1, ""),
				new WindowMessageTableRecord(21, "EntryErrorAimeOverlap", 0, 1, 4, "当前正在使用该账号", "", "无法使用同一账号参加游戏", "", -1, ""),
				new WindowMessageTableRecord(22, "EntryErrorOldServerDown", 0, 1, 4, "服务器维护中", "", "目前无法进行账号的注册与继承", "", -1, ""),
				new WindowMessageTableRecord(23, "EntryDoneEntryTwoPlayer", 0, 1, 4, "双人游戏奖励！", "", "可玩乐曲已从３首增加至４首！", "", -1, ""),
				new WindowMessageTableRecord(24, "EntryDoneEntryTwoPlayerNew", 0, 1, 4, "双人游戏奖励！", "", "可免费游玩的乐曲已从１首增加至４首！", "", -1, ""),
				new WindowMessageTableRecord(25, "EntryDoneEntryDailyBonus", 0, 1, 4, "每日乐曲数增加的奖励！", "", "可玩乐曲已从３首增加至４首！", "", -1, ""),
				new WindowMessageTableRecord(26, "EntryDoneEntryWeekdayBonus", 0, 1, 4, "每日乐曲数增加的奖励！", "", "可玩乐曲已从３首增加至４首！", "", -1, ""),
				new WindowMessageTableRecord(27, "EntryErrorAccessCodeRead", 0, 1, 4, "读取失败", "", "将兼容Aime的卡或手机与Aime读卡器接触\r\n然后再试一次", "", -1, ""),
				new WindowMessageTableRecord(28, "EntryErrorAccessCodeRegistration", 0, 1, 4, "Aime未注册", "", "由于Aime未注册，无法显示访问代码", "", -1, ""),
				new WindowMessageTableRecord(29, "PhotoNotRegistNet", 0, 1, 3, "关于上传", "", "需要登录舞萌DX官方公众号\r\n除了上传以外还可以进行游戏成绩确！", "", -1, ""),
				new WindowMessageTableRecord(30, "PhotoUploadConfirm", 0, 0, 4, "要上传吗？", "", "单次游戏只可以上传1张", "", -1, ""),
				new WindowMessageTableRecord(31, "PhotoUploadContract", 0, 1, 0, "服务条款", "", "服务条款\r\n\r\n・拍摄到的照片可以通过互联网上传到舞萌DX官方公众号\r\n以及第三方运营的SNS上。请用户针对被上传的肖像\r\n以及周围的场景信息会在互联网上被公开的这一事实，\r\n\u3000在进行过充分理解并慎重考虑的前提下再进行上传。\r\n\r\n・即使发生因上传照片致使用户和第三方机构之间\r\n发生纠纷、造成损失，本公司也概不负责。\r\n\r\n・本公司有权在任何情况任何时间将显示的照片删除。\r\n\r\n・如涉及侵犯肖像权，请联系我司公众号\r\n客服进行后台确认，如有侵权将尽快删除。\r\n\r\n※每次游戏只能上传1张照片。", "", -1, ""),
				new WindowMessageTableRecord(32, "PhotoUploadDone", 0, 1, 3, "已接受上传请求", "", "上传后的照片请在舞萌DX官方公众号中进行确认", "", -1, ""),
				new WindowMessageTableRecord(33, "NameEntryDescription", 0, 1, 4, "请输入名字", "", "按「END」键完成输入", "", -1, ""),
				new WindowMessageTableRecord(34, "NameEntryConfirm", 0, 1, 4, "这个名字可以吗？", "", "※输入的名字会在游戏中以及舞萌DX官方公众号上公开", "", -1, ""),
				new WindowMessageTableRecord(35, "NameEntryNgwordInfo", 1, 2, 4, "名字已变更为“舞萌”", "", "输入的内容中包含屏蔽字", "", -1, ""),
				new WindowMessageTableRecord(36, "NameEntryNotEnteredInfo", 1, 2, 4, "名字已变更为“舞萌”", "", "没有输入名字，或只输入了空格符", "", -1, ""),
				new WindowMessageTableRecord(37, "NameEntryWelcomeInfo", 0, 0, 4, "欢迎来到“舞萌DX”！", "", "将使用该名字登录游戏（您可以随时更改）", "", -1, ""),
				new WindowMessageTableRecord(38, "NameEntryTimeupInfo01", 1, 2, 3, "倒计时结束", "", "将采用已输入的名字信息", "", -1, ""),
				new WindowMessageTableRecord(39, "NameEntryTimeUpInfo02", 1, 2, 3, "倒计时结束", "", "由于输入的内容包含屏蔽字，\r\n名字将自动变更为“maimai”", "", -1, ""),
				new WindowMessageTableRecord(40, "NameEntryTimeUpInfo03", 1, 2, 3, "倒计时结束", "", "由于没有输入名字或只输入了空格符，\r\n名字将自动变更为“maimai”", "", -1, ""),
				new WindowMessageTableRecord(41, "PlayPreparationWait", 0, 2, 4, "", "", "请稍候", "", -1, ""),
				new WindowMessageTableRecord(42, "PlayPreparationCancel", 0, 2, 4, "已回到乐曲选择", "", "可以商量下玩哪首乐曲哦", "", 3000, ""),
				new WindowMessageTableRecord(43, "FreedomModeTimeUp", 0, 1, 4, "倒计时结束！", "", "没有剩余时间了\r\n结束乐曲播放", "", -1, ""),
				new WindowMessageTableRecord(44, "CollectionAttentionMaxFavorite", 1, 1, 4, "最爱收藏栏已满！", "", "最多可以设置20件最爱收藏品\n可在收藏栏内查看已设置的最爱", "", 5000, ""),
				new WindowMessageTableRecord(45, "CollectionAttentionEmptyFavorite", 0, 1, 4, "", "", "没有设置任何最爱收藏品", "", 3000, ""),
				new WindowMessageTableRecord(46, "CollectionCategorySelectAnnounce", 0, 1, 3, "", "", "请选择想要设定的种类", "", -1, ""),
				new WindowMessageTableRecord(47, "CollectionSelectAnnounce", 0, 1, 3, "", "", "请选择想要设置的头像", "", -1, ""),
				new WindowMessageTableRecord(48, "CollectionGotoFavorite", 0, 1, 3, "", "", "使用1P<sprite name=left_arrow>移动\u3000最多可设置20件最爱", "", -1, ""),
				new WindowMessageTableRecord(49, "CollectionGotoCollectionCustom", 0, 1, 3, "", "", "使用1P<sprite name=left_arrow>按钮返回收藏品设置", "", -1, ""),
				new WindowMessageTableRecord(50, "CollectionGetAnnounceInfo", 0, 1, 4, "", "", "详细信息可以在藏品定制中确认！", "", -1, ""),
				new WindowMessageTableRecord(51, "GhostDifficultyInfomation", 0, 0, 3, "友人对战", "", "对战结果会影响到你的“评级”哦！\r\n胜利的话可以增加“区域”中的移动距离！", "", -1, ""),
				new WindowMessageTableRecord(52, "NetworkErrorToGuest", 1, 1, 4, "网络连接错误", "", "网络连接发生错误\r\n开启游客模式进行游戏", "", 5000, ""),
				new WindowMessageTableRecord(53, "TutorialSelectInfo", 0, 1, 4, "", "", "要播放游戏操作说明吗？", "", -1, ""),
				new WindowMessageTableRecord(54, "TutorialEnter", 0, 1, 4, "", "", "开始进行游戏操作说明♪", "", -1, ""),
				new WindowMessageTableRecord(55, "TutorialExit", 0, 1, 4, "", "", "跳过游戏操作说明", "", -1, ""),
				new WindowMessageTableRecord(56, "NextTrackTips01", 0, 1, 3, "首先以通关为目标吧！", "", "达成率达到80%即可通关哦！", "", -1, ""),
				new WindowMessageTableRecord(57, "NextTrackTips02", 0, 1, 3, "目标！FULLCOMBO！", "", "FULLCOMBO的条件是一个MISS都不能出现哦！", "", -1, ""),
				new WindowMessageTableRecord(58, "NextTrackTips03", 0, 1, 3, "按键流？触屏流？", "", "TAP和HOLD的话用按键或触屏都没问题！", "", -1, ""),
				new WindowMessageTableRecord(59, "NextTrackTips04", 0, 1, 3, "别放走那些红色的音符！", "", "BREAK（红色音符）能得高分！", "", -1, ""),
				new WindowMessageTableRecord(60, "NextTrackTips05", 0, 1, 3, "在解决SLIDE音符时不要立刻滑动", "", "拍打☆后等待一拍再开始滑动哦！", "", -1, ""),
				new WindowMessageTableRecord(61, "NextTrackTips06", 0, 1, 3, "大量的TOUCH音符", "", "大量TOUCH音符同时出现时，\r\n把手张开来个一网打尽吧！", "", -1, ""),
				new WindowMessageTableRecord(62, "NextTrackTips07", 0, 1, 3, "耳机插孔在画面下方！", "", "在游戏开始前可以调整音量哟！", "", -1, ""),
				new WindowMessageTableRecord(63, "NextTrackTips08", 0, 1, 3, "建议使用手套", "", "会更容易进行游戏哦！\r\n真的会变得更容易的哟！！", "", -1, ""),
				new WindowMessageTableRecord(64, "NextTrackTips09", 0, 1, 3, "最多可以4人一齐玩哟！", "", "只要有2台机台的话…", "", -1, ""),
				new WindowMessageTableRecord(65, "NextTrackTips10", 0, 1, 3, "想轻松选择乐曲的话", "", "用<color=#51bcf3><sprite name=left_arrow_w></color> <color=#ff4100><sprite name=right_arrow_w></color>按键可以改变乐曲排序哦！", "", -1, ""),
				new WindowMessageTableRecord(66, "NextTrackTips11", 0, 1, 3, "想轻松选择乐曲的话", "", "用<color=#51bcf3><sprite name=left_arrow_w></color> <color=#ff4100><sprite name=right_arrow_w></color>按键可以改变乐曲排序哦！", "", -1, ""),
				new WindowMessageTableRecord(67, "NextTrackTips12", 0, 1, 3, "同时按下左右两边的第二个按键！", "", "便可切换「DX」和「标准」乐谱的啊", "", -1, ""),
				new WindowMessageTableRecord(68, "NextTrackTips13", 0, 1, 3, "来培养角色吧！", "", "到达等级9时会有1次觉醒！\r\n还会获得头像的哦！", "", -1, ""),
				new WindowMessageTableRecord(69, "NextTrackTips14", 0, 1, 3, "推しメンを固定したい！", "", "推しメンはロックしておけば\r\nおまかせで選択しても変わらずそのまま！", "", -1, ""),
				new WindowMessageTableRecord(70, "NextTrackTips15", 0, 1, 3, "试过拍摄头像吗？", "", "进入后即可拍摄！", "", -1, ""),
				new WindowMessageTableRecord(71, "NextTrackTips16", 0, 1, 3, "上传结果！", "", "您可以将您拍摄的照片发布在 SNS 上。", "", -1, ""),
				new WindowMessageTableRecord(72, "NextTrackTips17", 0, 1, 3, "maimai dx NET", "", "免费！功能丰富！搜索！", "", -1, ""),
				new WindowMessageTableRecord(73, "NextTrackTips18", 0, 1, 3, "想要挑战MASTER难度？", "", "在EXPERT难度下获得S评级(97%)\r\n就可以解禁MASTER难度啦！", "", -1, ""),
				new WindowMessageTableRecord(74, "NextTrackTips19", 0, 1, 3, "想要挑战MASTER难度？", "", "在EXPERT难度下获得S评级(97%)\r\n就可以解禁MASTER难度啦！", "", -1, ""),
				new WindowMessageTableRecord(75, "NextTrackTips20", 0, 1, 3, "ゴールドパスなら…", "", "移動距離アップ！MASTER解放！\r\nカードメイカーでゲットだ", "", -1, ""),
				new WindowMessageTableRecord(76, "NextTrackTips21", 0, 1, 3, "ゴールドパスなら…", "", "移動距離アップ！MASTER解放！\r\nカードメイカーでゲットだ", "", -1, ""),
				new WindowMessageTableRecord(77, "NextTrackTips22", 0, 1, 3, "合体する、でらっくすパス", "", "２人で遊ぶとき、同じでらっくすパスを使うと\r\n効果が更にパワーアップ！", "", -1, ""),
				new WindowMessageTableRecord(78, "NextTrackTips23", 0, 1, 3, "でらっくすパスを１回読み込ませれば…", "", "次からパスが無くても選択できるよ", "", -1, ""),
				new WindowMessageTableRecord(79, "NextTrackTips24", 0, 1, 3, "でらっくすレーティングって何？", "", "腕前をあらわす数値だよ\r\n５０００到達で一人前！", "", -1, ""),
				new WindowMessageTableRecord(80, "NextTrackTips25", 0, 1, 3, "でらっくすレーティングって何？", "", "腕前をあらわす数値だよ\r\n５０００到達で一人前！", "", -1, ""),
				new WindowMessageTableRecord(81, "NextTrackTips26", 0, 1, 3, "レーティングが上がらない件 1/3", "", "新曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(82, "NextTrackTips27", 0, 1, 3, "レーティングが上がらない件 1/3", "", "新曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(83, "NextTrackTips28", 0, 1, 3, "レーティングが上がらない件 1/3", "", "新曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(84, "NextTrackTips29", 0, 1, 3, "レーティングが上がらない件 2/3", "", "旧バージョン楽曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(85, "NextTrackTips30", 0, 1, 3, "レーティングが上がらない件 2/3", "", "旧バージョン楽曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(86, "NextTrackTips31", 0, 1, 3, "レーティングが上がらない件 2/3", "", "旧バージョン楽曲枠があるよ", "", -1, ""),
				new WindowMessageTableRecord(87, "NextTrackTips32", 0, 1, 3, "提升评级的小贴士 3/3", "", "在友人对战中获胜！\r\n晋升后便可提升评级！", "", -1, ""),
				new WindowMessageTableRecord(88, "NextTrackTips33", 0, 1, 3, "提升评级的小贴士 3/3", "", "在友人对战中获胜！\r\n晋升后便可提升评级！", "", -1, ""),
				new WindowMessageTableRecord(89, "NextTrackTips34", 0, 1, 3, "提升评级的小贴士 3/3", "", "在友人对战中获胜！\r\n晋升后便可提升评级！", "", -1, ""),
				new WindowMessageTableRecord(90, "NextTrackTips35", 0, 1, 3, "時間貸し！フリーダムモード", "", "エントリー時右上のボタンに注目！\r\n１人プレイ専用モードだよ", "", -1, ""),
				new WindowMessageTableRecord(91, "NextTrackTips36", 0, 1, 3, "でらっくスコア", "", "更なる精度が求められるスコアだよ\r\n限界に挑戦！", "", -1, ""),
				new WindowMessageTableRecord(92, "IconPhotoContract", 0, 1, 0, "服务条款", "", "<align=\"left\">\r\n･拍摄的照片会作为头像在和舞萌DX 官方帐号以下场所\r\n 进行显示。请用户在确认无误的前提下同意并使用。\r\n\r\n      1）游戏进行时的个人信息\r\n      2）舞萌DX官方公众号的个人信息页面\r\n\r\n･即使发生因上传照片致使用户和第三方机构之间发生纠纷、\r\n 造成损失，本公司也概不负责。\r\n\r\n･本公司有权在任何情况任何时间将显示的照片删除。\r\n\r\n<align=\"center\">※最新拍摄的照片会覆盖之前的照片。\r\n\r\n<size=120%>请问同意上述内容吗？</size>", "", -1, ""),
				new WindowMessageTableRecord(93, "ClassicModeSelectMessage", 0, 1, 3, "クラシックモードではじめます", "", "となりのプレイヤーがクラシックモードを選択しました", "", -1, ""),
				new WindowMessageTableRecord(94, "CharacterSelectAutoMessage", 0, 1, 3, "", "", "选择移动力高的旅行伙伴", "", -1, ""),
				new WindowMessageTableRecord(95, "CardReadFailedMessage", 0, 1, 4, "通知信息", "", "カード読み込みに失敗しました\nもしくは使用できないカードです", "", -1, ""),
				new WindowMessageTableRecord(96, "CodeReadInsertCardMessage", 1, 1, 3, "通知信息", "", "カードが挿したままになっています\r\n忘れずにお持ち帰りください", "", 5000, ""),
				new WindowMessageTableRecord(97, "FreedomModeTerminationMessage", 0, 1, 4, "通知信息", "", "強制終了の操作を受け付けました\r\n制限時間をゼロにしますか？", "", -1, ""),
				new WindowMessageTableRecord(98, "DataSaveStart", 0, 1, 4, "与服务器通信中", "", "请耐心等待", "", -1, ""),
				new WindowMessageTableRecord(99, "DataSaveError", 1, 1, 4, "", "", "数据保存失败\r\n无法反映本次游戏成绩", "", -1, ""),
				new WindowMessageTableRecord(100, "TakeOverInfo01", 0, 1, 3, "继承玩家数据①", "", "将「maimai FiNALE」版本为止的得分情况继承本游戏中。\r\n达成率（根据新系统转换为100％或更高）\r\nFULL COMBO / ALL PERFECT图标・SYNC图标（转换为新版标准）\r\n※在「maimai FiNALE」中所 MASTER 和 Re: MASTER的乐谱都将被解锁。", "", -1, ""),
				new WindowMessageTableRecord(101, "TakeOverInfo02", 0, 1, 3, "继承玩家数据②", "", "除部分收藏品外，以下物品将在\r\n凌晨定期维护后继承\r\n\r\n<align=\"left\">\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000・头像\r\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000・姓名框\r\n\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000・称号</align=\"left\">\r\n\r\n※数据继承只能进行一次", "", -1, ""),
				new WindowMessageTableRecord(102, "Warning", 1, 1, 4, "警告", "", "没有连接互联网\r\n无法游玩部分乐曲\r\n无法使用玩家账号", "", -1, ""),
				new WindowMessageTableRecord(103, "CodeReadFirst", 0, 1, 3, "でらっくすパスを使ってみよう！", "", "難易度MASTER解放など色々効果があるよ", "", -1, ""),
				new WindowMessageTableRecord(104, "CodeReadOutOfService", 0, 1, 3, "ブースト期間切れのでらっくすパス", "", "おまけ効果として、\r\nレーティング対象楽曲が見れるようになるよ\r\n\r\n別のAimeで使用しても同じ効果になるので、\r\n使ったことがない友達にプレゼントするのもオススメ！", "", -1, ""),
				new WindowMessageTableRecord(105, "CodeReadNotHave", 0, 1, 4, "別のAimeで印刷したでらっくすパス", "", "おまけ効果として、\r\nレーティング対象楽曲が見れるようになるよ\r\n", "", -1, ""),
				new WindowMessageTableRecord(106, "SimpleSettingFirst", 0, 1, 2, "试着拍一张头像吧！", "", "", "", -1, "UI_CWD_FirstInfo_008"),
				new WindowMessageTableRecord(107, "SinmpeSettingNewFrame", 0, 1, 4, "追加了限时提供的相框！", "", "使用用新相框来制作一张纪念头像照吧！", "", -1, ""),
				new WindowMessageTableRecord(108, "MapSelectFirst", 0, 1, 2, "选择区域吧！", "", "", "", -1, "UI_CWD_FirstInfo_010"),
				new WindowMessageTableRecord(109, "CharaSelectFirst", 0, 1, 2, "进行旅行伙伴编成！", "", "", "", -1, "UI_CWD_FirstInfo_011"),
				new WindowMessageTableRecord(110, "CharaSelectGoodBad", 0, 1, 2, "选择符合该区域的旅行伙伴", "", "", "", -1, "UI_CWD_FirstInfo_012"),
				new WindowMessageTableRecord(111, "CategorySelectFirst", 0, 1, 2, "选择乐曲种类吧", "", "", "", -1, "UI_CWD_FirstInfo_014"),
				new WindowMessageTableRecord(112, "MusicSelectFirst", 0, 1, 2, "选择乐曲吧", "", "", "", -1, "UI_CWD_FirstInfo_015"),
				new WindowMessageTableRecord(113, "PlayReadyFirst", 0, 1, 2, "来进行游玩乐曲前的准备吧！", "", "", "", -1, "UI_CWD_FirstInfo_016"),
				new WindowMessageTableRecord(114, "DxStandardFirst", 0, 1, 2, "DX/标准乐谱的切换", "", "", "", -1, "UI_CWD_FirstInfo_017"),
				new WindowMessageTableRecord(115, "OtomodachiFirst", 0, 1, 2, "尝试一下友人对战吧", "", "", "", -1, "UI_CWD_FirstInfo_038"),
				new WindowMessageTableRecord(116, "RateCategoryFirst", 0, 1, 2, "确认下评级对象乐曲", "", "这些都是会对你的DX评级有影响的乐曲哦\r\n试着找一下可以更新纪录的乐曲吧", "", -1, ""),
				new WindowMessageTableRecord(117, "CharaAwakeFirst", 0, 1, 2, "通过觉醒角色来获得收藏品吧！", "", "在每个旅行伙伴觉醒１次时都能获得头像！\r\n每个区域的总游戏数也会触发奖赏哟", "", -1, ""),
				new WindowMessageTableRecord(118, "TotalResultFirst", 0, 1, 2, "确认一下游戏成绩吧", "", "", "", -1, "UI_CWD_FirstInfo_021"),
				new WindowMessageTableRecord(119, "CollectionCustomFirst", 0, 1, 3, "来设置收藏品吧！", "", "设置后的收藏品会显示在画面上方哟\r\n获得新的收藏品后记得要去确认一下哦", "", -1, ""),
				new WindowMessageTableRecord(120, "DxPassFirst", 0, 1, 2, "でらっくすパスを印刷してみよう！", "", "", "", -1, "UI_CWD_FirstInfo_023"),
				new WindowMessageTableRecord(121, "FreedomFirst", 0, 1, 0, "自由模式规则", "", "<align=\"left\">・となりの画面に制限時間を表示しています\r\n・カテゴリセレクト～楽曲リザルトの間でカウントします\r\n・Aimeをかざし、となりの画面で操作することで\r\n\u3000制限時間をゼロにして強制終了することができます", "", -1, ""),
				new WindowMessageTableRecord(122, "SortFirst", 0, 1, 3, "ソート設定の有効箇所", "", "ここで設定できるカテゴリにのみ\r\nソート設定が反映されます", "", -1, ""),
				new WindowMessageTableRecord(123, "AimeUseNotice", 0, 1, 2, "试着用微信号来玩吧", "", "", "", -1, "UI_CWD_FirstInfo_026"),
				new WindowMessageTableRecord(124, "MusicSelectCanceRecruit", 0, 1, 4, "已取消招募", "", "请重新选择想要玩的乐曲", "", 2000, ""),
				new WindowMessageTableRecord(125, "MusicSelectCancelConfirmHost", 0, 1, 4, "返回乐曲选择", "", "取消店内招募，可以吗？", "", -1, ""),
				new WindowMessageTableRecord(126, "MusicSelectCancelConfirmClient", 0, 1, 4, "返回乐曲选择", "", "不参加店内招募，可以吗？", "", -1, ""),
				new WindowMessageTableRecord(127, "MusicSelectConfirmTrackStart", 0, 2, 4, "有玩家正在准备中", "", "现在就开始游戏可以吗？", "", -1, ""),
				new WindowMessageTableRecord(128, "MusicSelectForceTrackStart", 0, 2, 4, "乐曲开始", "", "招募到的玩家已经开始游戏了", "", 2000, ""),
				new WindowMessageTableRecord(129, "MusicSelectWaitToHost", 0, 2, 4, "有招募到的玩家正在等待", "", "请在招募到的玩家操作结束前耐心等待", "", -1, ""),
				new WindowMessageTableRecord(130, "PhotoAgree", 0, 1, 3, "关于纪念照片", "", "游戏结束时拍摄的照片可在事后进行确认和上传\r\n要拍摄纪念照片吗？", "", -1, ""),
				new WindowMessageTableRecord(131, "NetworkErrorUpPlaylog", 1, 1, 4, "网络连接错误", "", "网络连接发生错误\r\n无法保存后续游戏数据", "", 5000, ""),
				new WindowMessageTableRecord(132, "NetworkError", 1, 1, 4, "网络连接错误", "", "与服务器的连接已断开", "", -1, ""),
				new WindowMessageTableRecord(133, "EntryError", 1, 1, 4, "", "", "因为游戏点数不足无法参加游戏", "", -1, ""),
				new WindowMessageTableRecord(134, "MapCounterStop", 0, 1, 4, "", "", "恭喜通关！\r\n敬请期待新的地图！", "", -1, ""),
				new WindowMessageTableRecord(135, "BossAppearFirst", 0, 1, 2, "ボスオトモダチ登場！", "", "", "", -1, "UI_CWD_FirstInfo_034"),
				new WindowMessageTableRecord(136, "BossStayHint", 0, 1, 2, "クラスを上げるには？", "", "", "", -1, "UI_CWD_FirstInfo_033"),
				new WindowMessageTableRecord(137, "TrackSkip3Second", 1, 1, 3, "注意！", "", "正在跳过曲目\r\n３秒后游戏结束", "", -1, ""),
				new WindowMessageTableRecord(138, "TrackSkip2Second", 1, 1, 3, "注意！", "", "正在跳过曲目\r\n２秒后游戏结束", "", -1, ""),
				new WindowMessageTableRecord(139, "TrackSkip1Second", 1, 1, 3, "注意！", "", "正在跳过曲目\r\n１秒后游戏结束", "", -1, ""),
				new WindowMessageTableRecord(140, "CharaLockInfo", 0, 1, 2, "通知信息", "", "", "", -1, "UI_CWD_FirstInfo_030"),
				new WindowMessageTableRecord(141, "LoginBonusFirst01", 0, 1, 2, "追加了新的伙伴！", "", "", "", -1, "UI_CWD_FirstInfo_027"),
				new WindowMessageTableRecord(142, "LoginBonusFirst02", 0, 1, 2, "パートナーがナビゲーションするよ！", "", "", "", -1, "UI_CWD_FirstInfo_029"),
				new WindowMessageTableRecord(143, "LoginBonusFirst03", 0, 1, 2, "选择一张集邮册并收集邮票", "", "", "", -1, "UI_CWD_FirstInfo_028"),
				new WindowMessageTableRecord(144, "CollectionPartnerFIrst", 0, 1, 4, "通知信息", "", "新しい「パートナー」が選べるよ！", "", -1, ""),
				new WindowMessageTableRecord(145, "TransferDx01", 0, 1, 0, "将从舞萌DX继承以下信息", "", "<align=\"left\">将从舞萌DX继承以下信息\r\n・歌曲（部分除外）\r\n・收藏品\r\n・大师/宗师谱面\r\n・游玩成绩\r\n・段位\r\n・选项设定</align>", "", -1, ""),
				new WindowMessageTableRecord(146, "TransferDx02", 0, 1, 0, "关于段位和评级", "", "由于新歌对象歌曲的变动，评级值可能会发生变动。", "", -1, ""),
				new WindowMessageTableRecord(147, "TransferDx03", 1, 1, 0, "注意！", "", "网络连接失败！\r\n将以“游客模式”游玩", "", -1, ""),
				new WindowMessageTableRecord(148, "PassportCamera", 0, 1, 2, "通知信息", "", "", "", -1, "UI_CWD_FirstInfo_031"),
				new WindowMessageTableRecord(149, "CharaSelectCut", 0, 1, 2, "让我们编成小队！", "", "", "", -1, "UI_CWD_FirstInfo_032"),
				new WindowMessageTableRecord(150, "SpecialBossAppear", 0, 1, 2, "勝利して伝導楽曲をゲット！", "", "", "", -1, "UI_CWD_FirstInfo_035"),
				new WindowMessageTableRecord(151, "PhotoShootCancel", 0, 1, 4, "", "", "取消图标拍摄，可以吗？", "", -1, ""),
				new WindowMessageTableRecord(152, "FBRFirst", 0, 1, 2, "オトモダチに勝利して、クラスポイントを貯めよう！", "", "", "", -1, "UI_CWD_FirstInfo_038"),
				new WindowMessageTableRecord(153, "FBRClassPoint", 0, 1, 2, "クラスポイントについて", "", "", "", -1, "UI_CWD_FirstInfo_039"),
				new WindowMessageTableRecord(154, "FBRLegend", 0, 1, 2, "クラス「LEGEND」到達！", "", "", "", -1, "UI_CWD_FirstInfo_040"),
				new WindowMessageTableRecord(155, "TicketSelectFirst", 0, 1, 2, "チケット登場！", "", "", "", -1, "UI_CWD_FirstInfo_036"),
				new WindowMessageTableRecord(156, "ModeSelectSinRankAdd", 0, 1, 2, "来挑战吧！", "", "", "", -1, "UI_CWD_FirstInfo_037"),
				new WindowMessageTableRecord(157, "ModeSelectFirst", 0, 1, 2, "单击此处获取豪华通行证和摄影！", "", "", "", -1, "UI_CWD_FirstInfo_041"),
				new WindowMessageTableRecord(158, "TicketConnectServer", 0, 1, 4, "", "", "处理Ticket购买请求中", "", -1, ""),
				new WindowMessageTableRecord(159, "TicketConnectFailed", 1, 1, 3, "", "", "Ticket购买失败\r\n将不使用Ticket开始游戏", "", -1, ""),
				new WindowMessageTableRecord(160, "ModeSelectWhichOneTimeOut", 1, 1, 3, "", "", "因未及时确认而取消进入游戏", "", -1, "")
			};
		}
	}
}
