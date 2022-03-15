using System;
using System.Collections;
using System.Collections.Generic;
using DB;
using IO;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.MusicGenreName;
using Manager.MaiStudio.MusicLevelName;
using Manager.MaiStudio.MusicNameSortName;
using Manager.MaiStudio.MusicVersionName;
using Manager.UserDatas;
using Monitor.MusicSelect.ChainList;
using Monitor.MusicSelect.OtherParts;
using Monitor.MusicSelect.UI;
using Process;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;
using Util;

namespace Monitor
{
	public class MusicSelectMonitor : MonitorBase
	{
		public enum FcapIconEnum
		{
			None,
			Fc,
			Fcp,
			Ap,
			App,
			End
		}

		public enum CompIconEnum
		{
			None,
			Single,
			Double,
			End
		}

		public enum IconKindEnum
		{
			Rank,
			Fcap,
			Comp,
			End
		}

		public class OptionSummaryData
		{
			public OptionCategoryID Category;

			public int Index;

			public OptionSummaryData(OptionCategoryID category, OptionCateGameID optionIndex)
			{
				Category = category;
				Index = (int)optionIndex;
			}

			public OptionSummaryData(OptionCategoryID category, OptionCateSoundID optionIndex)
			{
				Category = category;
				Index = (int)optionIndex;
			}

			public OptionSummaryData(OptionCategoryID category, OptionCateJudgeID optionIndex)
			{
				Category = category;
				Index = (int)optionIndex;
			}

			public OptionSummaryData(OptionCategoryID category, OptionCateSpeedID optionIndex)
			{
				Category = category;
				Index = (int)optionIndex;
			}

			public OptionSummaryData(OptionCategoryID category, OptionCateDesignID optionIndex)
			{
				Category = category;
				Index = (int)optionIndex;
			}
		}

		private readonly OptionSummaryData[] _summary = new OptionSummaryData[12]
		{
			new OptionSummaryData(OptionCategoryID.GameSetting, OptionCateJudgeID.DispCenter),
			new OptionSummaryData(OptionCategoryID.JudgeSetting, OptionCateJudgeID.DispJudge),
			new OptionSummaryData(OptionCategoryID.SpeedSetting, OptionCateSpeedID.SlideSpeed),
			new OptionSummaryData(OptionCategoryID.SpeedSetting, OptionCateSpeedID.TouchSpeed),
			new OptionSummaryData(OptionCategoryID.DesignSetting, OptionCateDesignID.StarDesign),
			new OptionSummaryData(OptionCategoryID.GameSetting, OptionCateGameID.AdjustTiming),
			new OptionSummaryData(OptionCategoryID.SoundSetting, OptionCateSoundID.Ans_Vol),
			new OptionSummaryData(OptionCategoryID.SoundSetting, OptionCateSoundID.Tap_Vol),
			new OptionSummaryData(OptionCategoryID.GameSetting, OptionCateGameID.Brightness),
			new OptionSummaryData(OptionCategoryID.DesignSetting, OptionCateDesignID.TapDesign),
			new OptionSummaryData(OptionCategoryID.DesignSetting, OptionCateDesignID.HoldDesign),
			new OptionSummaryData(OptionCategoryID.DesignSetting, OptionCateDesignID.SlideDesign)
		};

		[SerializeField]
		[Header("各種コントローラー")]
		private ButtonSelectController _buttonController;

		[SerializeField]
		private DifficultyContoroller _difficultyContoroller;

		[SerializeField]
		private GenreSelectController _genreTabController;

		[SerializeField]
		private ExtraInfoController _extraInfoController;

		[SerializeField]
		private ConnectMatchingController _connectMatchingController;

		[SerializeField]
		private ButtonSelectController _infoButtonController;

		[SerializeField]
		[Header("カードチェーンリスト")]
		private MusicSelectChainList _musicChainList;

		[SerializeField]
		private NormalChainList _normalChainList;

		[SerializeField]
		private MenuChainList _menuChainList;

		[SerializeField]
		private GenreSelectChainList _genreChainList;

		[SerializeField]
		[Header("オプション")]
		private OptionSummaryObject _optionSummary;

		[SerializeField]
		[Header("背景ポジション")]
		private Transform _selectorBackgroundObjectTargetParent;

		[SerializeField]
		private Transform _testChallengeDecoBGTargetParent;

		[SerializeField]
		private Transform _testChallengeBGTargetParent;

		[SerializeField]
		[Header("背景")]
		private GameObject _originalSelectorBackgroundObject;

		[SerializeField]
		private GameObject _categorySelectorBackgroundObject;

		[SerializeField]
		private GameObject _originalChallengeDecoBGObj;

		[SerializeField]
		private GameObject _originalChallengeBGObj;

		[SerializeField]
		[Header("コース情報")]
		private Transform _courseBGParent;

		[SerializeField]
		private Transform _courseInfoParent;

		[SerializeField]
		private Transform _courseTransitionParent;

		[SerializeField]
		private GameObject _originalCourseBGObj;

		[SerializeField]
		private GameObject _originalCourseInfoObj;

		[SerializeField]
		private GameObject _originalCourseTransitionObj;

		[SerializeField]
		[Header("ブラー")]
		private GameObject _blurObject;

		[SerializeField]
		[Header("フリーダムモードタイムアップ用")]
		private GameObject _originalFreedomModeTimeUp;

		private MessageMusicData _messageMusic;

		private IMusicSelectProcess _musicSelect;

		private AssetManager _assetManager;

		private bool _isStartPlay;

		private Animator _freedomModeTimeUpAnimator;

		private Coroutine _currentCoroutine;

		private SelectorBackgroundController _selectorBgController;

		private GenreBackgroundController _genreSelectorBgController;

		private ChallengeDecoBG _challengeDecoBGController;

		private ChallengeBG _challengeBGController;

		private CourseBGController _courseBg;

		private DaniDataInfo _courseInfo;

		private CourseTransition _courseTransition;

		private Sprite _otherSprite;

		private Dictionary<int, Sprite> _genreSprite;

		private Dictionary<int, Sprite> _levelSprite;

		private Dictionary<int, Sprite> _nameSprite;

		private Dictionary<int, Sprite> _versionSprite;

		private Dictionary<int, Sprite> _tournamentSprite;

		private Dictionary<int, Sprite> _optionSprite;

		private Sprite[] _fcapSprite;

		private Sprite[] _rankSprite;

		private Sprite[] _compSprite;

		public override void Initialize(int playerIndex, bool isActive)
		{
			_genreTabController.Initialize(playerIndex);
			_extraInfoController.Initialize();
			_connectMatchingController.Initialize();
			_optionSummary.gameObject.SetActive(value: false);
			_blurObject.SetActive(value: false);
			MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
			int num = (_musicSelect.IsLevelTab() ? _musicSelect.GetDifficultyByLevel(0) : _musicSelect.GetCurrentDifficulty(playerIndex));
			if (_musicSelect.IsExtraFolder(0))
			{
				num = _musicSelect.GetDifficultyByLevel(0);
			}
			Notes notes = music?.ScoreData[num];
			if (music != null)
			{
				if (notes != null)
				{
					_messageMusic = new MessageMusicData(_assetManager.GetJacketTexture2D(music.MusicData.jacketFile), music.MusicData.name.str, num, notes.musicLevelID, GameManager.GetScoreKind(music.MusicData.name.id));
				}
				else
				{
					_messageMusic = new MessageMusicData(null, null, 0, 0, GameManager.GetScoreKind(music.MusicData.name.id));
				}
			}
			else
			{
				_messageMusic = null;
			}
			_musicSelect.SendMessage(new Message(ProcessType.CommonProcess, 20002, playerIndex, _messageMusic));
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
			int[] charaSlot = userData.Detail.CharaSlot;
			List<UserChara> charaList = userData.CharaList;
			for (int i = 0; i < charaSlot.Length; i++)
			{
				if (charaSlot[i] != 0)
				{
					CharaData charaData = Singleton<DataManager>.Instance.GetChara(charaSlot[i]);
					UserChara userChara = charaList.Find((UserChara a) => a.ID == charaData.GetID());
					if (userChara != null)
					{
						MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(charaData.color.id);
						Color regionColor = new Color32(mapColorData.Color.R, mapColorData.Color.G, mapColorData.Color.B, byte.MaxValue);
						Texture2D characterTexture2D = _assetManager.GetCharacterTexture2D(charaData.imageFile);
						MessageCharactorInfomationData messageCharactorInfomationData = new MessageCharactorInfomationData(i, charaData.genre.id, characterTexture2D, userChara.Level, userChara.Awakening, userChara.NextAwakePercent, regionColor);
						_musicSelect.SendMessage(new Message(ProcessType.CommonProcess, 20021, playerIndex, messageCharactorInfomationData));
					}
				}
			}
			_buttonController.SetVisible(false, InputManager.ButtonSetting.Button05);
			_buttonController.SetVisible(false, InputManager.ButtonSetting.Button02);
			_buttonController.SetVisible(false, InputManager.ButtonSetting.Button07);
			if (isActive)
			{
				_musicChainList.Initialize();
				_musicChainList.AdvancedInitialize(_musicSelect, _assetManager, playerIndex);
				_normalChainList.Initialize();
				_normalChainList.SetPlayerIndex(playerIndex);
				_normalChainList.AdvancedInitialize(_musicSelect, _assetManager, playerIndex);
				_menuChainList.Initialize();
				_menuChainList.AdvancedInitialize(_musicSelect, _assetManager, playerIndex);
				_menuChainList.HideUnUseCard();
				_genreChainList.Initialize();
				_genreChainList.AdvancedInitialize(_musicSelect, _assetManager, playerIndex);
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.Disabled);
				_selectorBgController.SetActiveIndicator(isActive: false);
				_selectorBgController.SetScrollMessage("");
				_selectorBgController.SetBackgroundColor(num);
				_difficultyContoroller.SetRemasterEnable(enable: false);
				_difficultyContoroller.InitDifficulty((MusicDifficultyID)num);
			}
			_musicSelect.SendMessage(new Message(ProcessType.CommonProcess, 20020, playerIndex, isActive));
			_genreSprite = new Dictionary<int, Sprite>();
			_levelSprite = new Dictionary<int, Sprite>();
			_nameSprite = new Dictionary<int, Sprite>();
			_versionSprite = new Dictionary<int, Sprite>();
			_tournamentSprite = new Dictionary<int, Sprite>();
			_optionSprite = new Dictionary<int, Sprite>();
			_otherSprite = Resources.Load<Sprite>("Common/Sprites/Tab/Title/UI_CLC_TabTitle_Others");
			foreach (object value in Enum.GetValues(typeof(Manager.MaiStudio.MusicGenreName.Table)))
			{
				string fileName = Singleton<DataManager>.Instance.GetMusicGenre((int)value).FileName;
				_genreSprite[(int)value] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName);
			}
			foreach (object value2 in Enum.GetValues(typeof(Manager.MaiStudio.MusicLevelName.Table)))
			{
				string fileName2 = Singleton<DataManager>.Instance.GetMusicLevel((int)value2).FileName;
				_levelSprite[(int)value2] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName2);
			}
			foreach (object value3 in Enum.GetValues(typeof(Manager.MaiStudio.MusicNameSortName.Table)))
			{
				string fileName3 = Singleton<DataManager>.Instance.GetMusicNameSort((int)value3).FileName;
				_nameSprite[(int)value3] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName3);
			}
			foreach (object value4 in Enum.GetValues(typeof(Manager.MaiStudio.MusicVersionName.Table)))
			{
				string fileName4 = Singleton<DataManager>.Instance.GetMusicVersion((int)value4).FileName;
				_versionSprite[(int)value4] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName4);
			}
			foreach (KeyValuePair<int, ScoreRankingForMusicSeq> scoreRanking in Singleton<ScoreRankingManager>.Instance.ScoreRankings)
			{
				string fileName5 = scoreRanking.Value.FileName;
				_tournamentSprite[scoreRanking.Key] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName5);
			}
			for (OptionCategoryID optionCategoryID = OptionCategoryID.SpeedSetting; optionCategoryID < OptionCategoryID.End; optionCategoryID++)
			{
				string filename = optionCategoryID.GetFilename();
				_optionSprite[(int)optionCategoryID] = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + filename);
			}
			_fcapSprite = new Sprite[5];
			_rankSprite = new Sprite[14];
			_compSprite = new Sprite[3];
			for (MusicClearrankID musicClearrankID = MusicClearrankID.Rank_D; musicClearrankID < MusicClearrankID.End; musicClearrankID++)
			{
				_rankSprite[(int)musicClearrankID] = null;
			}
			_fcapSprite[0] = null;
			_fcapSprite[1] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_FC");
			_fcapSprite[2] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_FCp");
			_fcapSprite[3] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_AP");
			_fcapSprite[4] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_APp");
			_rankSprite[8] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_S");
			_rankSprite[9] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_Sp");
			_rankSprite[10] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_SS");
			_rankSprite[11] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_SSp");
			_rankSprite[12] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_SSS");
			_rankSprite[13] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_SSSp");
			_compSprite[0] = null;
			_compSprite[1] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_DX_Silver");
			_compSprite[2] = Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/Medal/UI_MSS_Allclear_Icon_DX_Gold");
			_genreChainList.StopAnimatoinAll();
			base.Initialize(playerIndex, isActive);
		}

		public void SetData(int playerIndex, IMusicSelectProcess musicSelectProcess, AssetManager manager, MusicDifficultyID difficulty, bool isActive)
		{
			_musicSelect = musicSelectProcess;
			_assetManager = manager;
			_buttonController.Initialize(playerIndex, isActive);
			_infoButtonController.InitializeForInfo(playerIndex, isActive);
			if (isActive)
			{
				_selectorBgController = UnityEngine.Object.Instantiate(_originalSelectorBackgroundObject, _selectorBackgroundObjectTargetParent).GetComponent<SelectorBackgroundController>();
				_difficultyContoroller.Initialize(playerIndex, musicSelectProcess);
				_genreSelectorBgController = UnityEngine.Object.Instantiate(_categorySelectorBackgroundObject, _selectorBackgroundObjectTargetParent).GetComponent<GenreBackgroundController>();
				_genreSelectorBgController.gameObject.SetActive(value: false);
				_challengeDecoBGController = UnityEngine.Object.Instantiate(_originalChallengeDecoBGObj, _testChallengeDecoBGTargetParent).GetComponent<ChallengeDecoBG>();
				_challengeDecoBGController.gameObject.SetActive(value: false);
				_challengeBGController = UnityEngine.Object.Instantiate(_originalChallengeBGObj, _testChallengeBGTargetParent).GetComponent<ChallengeBG>();
				_challengeBGController.gameObject.SetActive(value: false);
				_courseBg = UnityEngine.Object.Instantiate(_originalCourseBGObj, _courseBGParent).GetComponent<CourseBGController>();
				_courseInfo = UnityEngine.Object.Instantiate(_originalCourseInfoObj, _courseInfoParent).GetComponent<DaniDataInfo>();
				_courseTransition = UnityEngine.Object.Instantiate(_originalCourseTransitionObj, _courseTransitionParent).GetComponent<CourseTransition>();
				_courseBg.gameObject.SetActive(value: false);
				_courseInfo.gameObject.SetActive(value: false);
				_courseTransition.gameObject.SetActive(value: false);
			}
			if (IsActive())
			{
				MechaManager.LedIf[playerIndex].ButtonLedReset();
			}
			if (GameManager.IsFreedomMode)
			{
				_freedomModeTimeUpAnimator = UnityEngine.Object.Instantiate(_originalFreedomModeTimeUp, Main.transform).GetComponent<Animator>();
				_freedomModeTimeUpAnimator.gameObject.SetActive(value: false);
			}
		}

		public void Play()
		{
			if (isPlayerActive)
			{
				StartCoroutine(PlayCoroutine());
			}
		}

		private IEnumerator PlayCoroutine()
		{
			if (GameManager.IsCourseMode)
			{
				_buttonController.SetVisibleImmediate(false, 2, 5, 3);
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.In);
				_difficultyContoroller.gameObject.SetActive(value: false);
			}
			else if (GameManager.MusicTrackNumber == 1)
			{
				_buttonController.SetVisibleImmediate(false, 2, 5, 3, 4);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.In);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Loop);
				_genreSelectorBgController.SetSampleImage(_musicSelect.GetGenreTextureList());
			}
			else
			{
				if (_musicSelect.IsConnectionFolder())
				{
					_buttonController.SetVisibleImmediate(false, 2, 5);
				}
				else
				{
					_buttonController.SetVisibleImmediate(false, 2, 5, 3, 4);
				}
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.In);
			}
			yield return new WaitForSeconds(0.5f);
			if (GameManager.IsCourseMode)
			{
				_buttonController.SetVisible(true, 3);
				_buttonController.SetVisible(false, 4);
				SetVisibleOptionSummary(isVisible: false);
			}
			else if (GameManager.MusicTrackNumber == 1)
			{
				_buttonController.SetVisible(true, 3);
				_buttonController.SetVisible(false, 4);
				SetVisibleOptionSummary(isVisible: false);
				_musicChainList.gameObject.SetActive(value: false);
				_normalChainList.gameObject.SetActive(value: false);
				_menuChainList.gameObject.SetActive(value: false);
				_genreChainList.gameObject.SetActive(value: true);
				_genreChainList.DeployGenreList();
				_genreChainList.Play();
				SetSideMessage(CommonMessageID.Scroll_Category_Select.GetName());
				_buttonController.ChangeButtonImage(InputManager.ButtonSetting.Button04, 5);
				_difficultyContoroller.gameObject.SetActive(value: true);
				_difficultyContoroller.DifficultyBaseIn();
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.SetDifficulty((MusicDifficultyID)_musicSelect.GetCurrentDifficulty(monitorIndex));
				_selectorBgController.SetBackgroundColor(_musicSelect.GetCurrentDifficulty(monitorIndex));
				_genreSelectorBgController.MoveIndicator(_musicSelect.CurrentCategorySelect);
				int num = _musicSelect.GenreSelectDataList[base.MonitorIndex].Count;
				if (!GameManager.IsFreedomMode)
				{
					num--;
				}
				if (_musicSelect.CurrentCategorySelect != num - 1)
				{
					_buttonController.SetVisible(true, 2);
				}
				if (_musicSelect.CurrentCategorySelect != 0)
				{
					_buttonController.SetVisible(true, 5);
				}
				_genreSelectorBgController.InitIndicator(num);
				_genreSelectorBgController.MoveIndicator(_musicSelect.CurrentCategorySelect);
			}
			else
			{
				_musicChainList.gameObject.SetActive(value: true);
				_normalChainList.gameObject.SetActive(value: false);
				_menuChainList.gameObject.SetActive(value: false);
				_genreChainList.gameObject.SetActive(value: false);
				_musicChainList.DeployMusicList();
				_musicChainList.Play();
				SetSideMessage(CommonMessageID.Scroll_Music_Select.GetName());
				int num2 = (_musicSelect.IsLevelTab() ? _musicSelect.GetDifficultyByLevel(0) : _musicSelect.GetCurrentDifficulty(monitorIndex));
				if (_musicSelect.IsExtraFolder(0))
				{
					num2 = _musicSelect.GetDifficultyByLevel(0);
				}
				if (_musicSelect.IsConnectionFolder())
				{
					_buttonController.SetVisibleImmediate(true, 2, 5);
				}
				else
				{
					_buttonController.SetVisibleImmediate(true, 2, 5, 3, 4);
				}
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsExtraFolder(0))
				{
					_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num2, isButtonChange: false);
				}
				else
				{
					DifficultyButtonCheck();
					_difficultyContoroller.ForcedSetDifficulty((MusicDifficultyID)num2);
				}
				_selectorBgController.SetBackgroundColor(num2);
				List<TabDataBase> list = new List<TabDataBase>();
				foreach (MusicSelectProcess.GenreSelectData item in _musicSelect.GenreSelectDataList[monitorIndex])
				{
					Sprite tabSprite = GetTabSprite(item);
					string tabString = getTabString(item);
					Color tabColor = getTabColor(item);
					list.Add(new TabDataBase(tabColor, tabSprite, tabString));
				}
				SetCompIconSprite();
				_genreTabController.SortType2Genre(list, 0);
				_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				_selectorBgController.SetActiveIndicator(isActive: true);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
			}
			_isStartPlay = true;
		}

		public Sprite GetTabSprite(MusicSelectProcess.GenreSelectData data)
		{
			Sprite sprite = null;
			if (data.isExtra)
			{
				if (data.isTournament)
				{
					int key = data.categoryID - 10000;
					if (_tournamentSprite.ContainsKey(key))
					{
						sprite = _tournamentSprite[key];
					}
				}
				else if (_genreSprite.ContainsKey(data.categoryID))
				{
					sprite = _genreSprite[data.categoryID];
				}
			}
			else
			{
				switch (_musicSelect.CategorySortSetting)
				{
				case SortTabID.Genre:
					if (_genreSprite.ContainsKey(data.categoryID))
					{
						sprite = _genreSprite[data.categoryID];
					}
					break;
				case SortTabID.All:
					sprite = _genreSprite[199];
					break;
				case SortTabID.Version:
					if (_versionSprite.ContainsKey(data.categoryID))
					{
						sprite = _versionSprite[data.categoryID];
					}
					break;
				case SortTabID.Level:
					if (_levelSprite.ContainsKey(data.categoryID))
					{
						sprite = _levelSprite[data.categoryID];
					}
					break;
				case SortTabID.Name:
					if (_nameSprite.ContainsKey(data.categoryID))
					{
						sprite = _nameSprite[data.categoryID];
					}
					break;
				}
			}
			if (sprite == null)
			{
				sprite = _otherSprite;
			}
			return sprite;
		}

		public string getTabString(MusicSelectProcess.GenreSelectData data)
		{
			string result = "";
			if (data.isExtra)
			{
				if (data.isTournament)
				{
					int rankingID = data.categoryID - 10000;
					result = Singleton<ScoreRankingManager>.Instance.getSrDataForMS(rankingID).GenreName;
				}
				else
				{
					result = Singleton<DataManager>.Instance.GetMusicGenre(data.categoryID).genreNameTwoLine;
				}
			}
			else
			{
				switch (_musicSelect.CategorySortSetting)
				{
				case SortTabID.Genre:
					result = Singleton<DataManager>.Instance.GetMusicGenre(data.categoryID).genreNameTwoLine;
					break;
				case SortTabID.All:
					result = Singleton<DataManager>.Instance.GetMusicGenre(199).genreNameTwoLine;
					break;
				case SortTabID.Version:
					result = Singleton<DataManager>.Instance.GetMusicVersion(data.categoryID).genreNameTwoLine;
					break;
				case SortTabID.Level:
					result = Singleton<DataManager>.Instance.GetMusicLevel(data.categoryID).genreNameTwoLine;
					break;
				case SortTabID.Name:
					result = Singleton<DataManager>.Instance.GetMusicNameSort(data.categoryID).genreNameTwoLine;
					break;
				}
			}
			return result;
		}

		public Color getTabColor(MusicSelectProcess.GenreSelectData data)
		{
			Color result = Color.red;
			if (data.isExtra)
			{
				if (data.isTournament)
				{
					int rankingID = data.categoryID - 10000;
					result = Utility.ConvertColor(Singleton<ScoreRankingManager>.Instance.getSrDataForMS(rankingID).GenreColor);
				}
				else
				{
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(data.categoryID).Color);
				}
			}
			else
			{
				switch (_musicSelect.CategorySortSetting)
				{
				case SortTabID.Genre:
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(data.categoryID).Color);
					break;
				case SortTabID.All:
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(199).Color);
					break;
				case SortTabID.Version:
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicVersion(data.categoryID).Color);
					break;
				case SortTabID.Level:
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicLevel(data.categoryID).Color);
					break;
				case SortTabID.Name:
					result = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicNameSort(data.categoryID).Color);
					break;
				}
			}
			return result;
		}

		public Sprite[] GetCompIconSprite(MusicClearrankID rank, FcapIconEnum fcap)
		{
			Sprite[] obj = new Sprite[3]
			{
				_rankSprite[(int)rank],
				_fcapSprite[(int)fcap],
				null
			};
			int num = 0;
			if (rank >= MusicClearrankID.Rank_S && fcap >= FcapIconEnum.Fc)
			{
				num++;
			}
			if (rank >= MusicClearrankID.Rank_SSS && fcap >= FcapIconEnum.Ap)
			{
				num++;
			}
			obj[2] = _compSprite[num];
			return obj;
		}

		private void SetCompIconSprite()
		{
			if (!isPlayerActive)
			{
				return;
			}
			MusicSelectProcess.GenreSelectData genreSelectData = _musicSelect.GetGenreSelectData(monitorIndex, 0);
			if (genreSelectData == null)
			{
				return;
			}
			MusicClearrankID rank = MusicClearrankID.Rank_D;
			FcapIconEnum fcapIconEnum = FcapIconEnum.None;
			fcapIconEnum = ((genreSelectData.totalMusicNum == genreSelectData.appNum) ? FcapIconEnum.App : ((genreSelectData.totalMusicNum == genreSelectData.apNum) ? FcapIconEnum.Ap : ((genreSelectData.totalMusicNum == genreSelectData.fcpNum) ? FcapIconEnum.Fcp : ((genreSelectData.totalMusicNum == genreSelectData.fcNum) ? FcapIconEnum.Fc : FcapIconEnum.None))));
			for (int num = 13; num >= 0; num--)
			{
				if (genreSelectData.totalMusicNum == genreSelectData.GetOverRankNum((MusicClearrankID)num))
				{
					rank = (MusicClearrankID)num;
					break;
				}
			}
			Sprite[] compIconSprite = GetCompIconSprite(rank, fcapIconEnum);
			if (genreSelectData.isExtra)
			{
				_genreTabController.SetLeftIcon(null);
				_genreTabController.SetRightIcon(null);
			}
			else
			{
				_genreTabController.SetLeftIcon(compIconSprite[0]);
				_genreTabController.SetRightIcon(compIconSprite[1]);
			}
		}

		public override void Release()
		{
			Resources.UnloadUnusedAssets();
			base.Release();
		}

		protected override void SetVisible(bool isVisible)
		{
			_buttonController.gameObject.SetActive(isVisible);
			_difficultyContoroller.gameObject.SetActive(isVisible);
		}

		public override void ViewUpdate()
		{
			_difficultyContoroller.ViewUpdate();
			_buttonController.ViewUpdate();
			_infoButtonController.ViewUpdate();
			_genreTabController.UpdateButtonAnimation();
			_selectorBgController?.UpdateScroll();
			_genreSelectorBgController?.UpdateScroll();
			_musicChainList.ViewUpdate();
			_normalChainList.ViewUpdate();
			_menuChainList.ViewUpdate();
			_genreChainList.ViewUpdate();
			_connectMatchingController?.UpdateTIme();
		}

		public void AnimationUpdate(float progress)
		{
		}

		private void SetUpperMusicInfo(MusicSelectProcess.MusicSelectData musicData, Notes notes, int difficulty)
		{
		}

		private void SetUpperMusicInfo(Notes notes, MusicDifficultyID difficulty)
		{
		}

		public void SetGhostData(int diff, bool playable)
		{
			if (!isPlayerActive)
			{
				return;
			}
			if (monitorIndex < GameManager.SelectGhostID.Length)
			{
				GhostManager.GhostTarget ghostTarget = _musicSelect.GetCombineMusic(0).musicSelectData[(int)_musicSelect.ScoreType].GhostTarget;
				int difficulty = _musicSelect.GetCombineMusic(0).musicSelectData[(int)_musicSelect.ScoreType].Difficulty;
				bool flag = playable || diff <= 2 || diff <= difficulty;
				UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(ghostTarget);
				long ghostPlayTime = ghostToEnum?.UnixTime ?? 0;
				MessageUserInformationData messageUserInformationData = null;
				if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Player)
				{
					messageUserInformationData = new MessageUserInformationData(2, _assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					}, OptionDisprateID.AllDisp, isSubMonitor: false);
				}
				else if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.MapNpc)
				{
					messageUserInformationData = new MessageUserInformationData(_assetManager, new UserGhost
					{
						Name = ghostToEnum.Name,
						IconId = ghostToEnum.IconId,
						PlateId = ghostToEnum.PlateId,
						TitleId = ghostToEnum.TitleId,
						Rate = ghostToEnum.Rate,
						ClassValue = ghostToEnum.ClassValue,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					});
				}
				else if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Boss)
				{
					messageUserInformationData = new MessageUserInformationData(_assetManager, new UserDetail
					{
						UserName = ghostToEnum.Name,
						EquipIconID = ghostToEnum.IconId,
						EquipPlateID = ghostToEnum.PlateId,
						EquipTitleID = ghostToEnum.TitleId,
						Rating = (uint)ghostToEnum.Rate,
						ClassRank = (uint)UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue),
						CourseRank = ghostToEnum.CourseRank
					});
				}
				for (int i = 0; i < 2; i++)
				{
					if (messageUserInformationData != null)
					{
						bool isBoss = false;
						switch (ghostToEnum?.Type)
						{
						case UserGhost.GhostType.MapNpc:
						case UserGhost.GhostType.Player:
							_extraInfoController.GhostObj[i].SetUserData(messageUserInformationData);
							_extraInfoController.GhostObj[i].SetAchievementRate((uint)ghostToEnum.Achievement);
							_extraInfoController.GhostObj[i].gameObject.SetActive(value: true);
							_extraInfoController.BossObj[i].gameObject.SetActive(value: false);
							isBoss = false;
							break;
						case UserGhost.GhostType.Boss:
							_extraInfoController.BossObj[i].SetUserData(messageUserInformationData);
							_extraInfoController.BossObj[i].SetAchievementRate((uint)ghostToEnum.Achievement);
							_extraInfoController.GhostObj[i].gameObject.SetActive(value: false);
							_extraInfoController.BossObj[i].gameObject.SetActive(value: true);
							isBoss = true;
							break;
						}
						bool isSpecialBoss = UserUdemae.IsBossSpecial(UserUdemae.GetRateToUdemaeID(ghostToEnum.ClassValue));
						_extraInfoController.SetGhostActive(difficulty <= diff && flag, isBoss, isSpecialBoss);
						_extraInfoController.setGhostPlayTime(ghostPlayTime);
					}
					else
					{
						_extraInfoController.SetAllHide();
					}
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					_extraInfoController.SetAllHide();
				}
			}
		}

		public void ActiveGhostData(bool isActive)
		{
			_extraInfoController.gameObject.SetActive(isActive);
		}

		public void ExtraInfoReset()
		{
			_extraInfoController.SetAllHide();
		}

		public void SetChallengeData(ExtraInfoController.EnumChallengeInfoSeq seq, int life, int diff, int day, bool info)
		{
			if (isPlayerActive)
			{
				_extraInfoController.gameObject.SetActive(value: true);
				_extraInfoController.SetChallengeView(seq);
				if (life > 0)
				{
					_extraInfoController.SetChallengeParam(life, diff, day, info);
				}
			}
		}

		public void CheckGhostJump()
		{
			if (isPlayerActive)
			{
				bool isActive = _musicSelect.GetCombineMusic(0).IsJumpGhostBattle(diff: _musicSelect.GetDifficulty(monitorIndex), scoreType: _musicSelect.ScoreType);
				bool isReturn = _musicSelect.IsGhostFolder(0);
				_extraInfoController.SetGhostJump(isActive, isReturn);
			}
		}

		public void SetGhostJumpHide()
		{
			if (isPlayerActive)
			{
				_extraInfoController.SetGhostJump(isActive: false, isReturn: false);
			}
		}

		public void SetChallengeMusicData()
		{
			if (!isPlayerActive)
			{
				return;
			}
			MusicSelectProcess.CombineMusicSelectData combineMusic = _musicSelect.GetCombineMusic(0);
			MusicSelectProcess.MusicSelectDetailData msDetailData = combineMusic.msDetailData;
			if (_musicSelect.IsChallengeFolder(0))
			{
				if (Singleton<NotesListManager>.Instance.GetNotesList()[combineMusic.GetID(_musicSelect.ScoreType)].ChallengeDetail[base.MonitorIndex].isEnable)
				{
					SetChallengeData(ExtraInfoController.EnumChallengeInfoSeq.Music, msDetailData.startLife, msDetailData.challengeUnlockDiff, msDetailData.nextRelaxDay, msDetailData.infoEnable);
				}
				else
				{
					_extraInfoController.SetAllHide();
				}
				_challengeDecoBGController.gameObject.SetActive(value: true);
				_challengeDecoBGController.SetAnim(ChallengeDecoBG.Anim.Loop);
				_challengeBGController.gameObject.SetActive(value: true);
				_challengeBGController.SetAnim(ChallengeBG.Anim.Loop);
			}
			else
			{
				_extraInfoController.SetAllHide();
				_challengeDecoBGController.gameObject.SetActive(value: false);
				_challengeBGController.gameObject.SetActive(value: false);
			}
		}

		public void ActiveScoreAttackRule(bool isActive)
		{
			_extraInfoController.SetScoreAttackRuleInfo(isActive);
		}

		public void SetChallengeBGDiffSeq()
		{
			if (isPlayerActive && _musicSelect.IsChallengeFolder(0))
			{
				_challengeDecoBGController.gameObject.SetActive(value: true);
				_challengeDecoBGController.SetAnim(ChallengeDecoBG.Anim.DiffIn);
			}
		}

		public void SetChallengeBGGameStart()
		{
			if (isPlayerActive && _musicSelect.IsChallengeFolder(0))
			{
				_challengeDecoBGController.gameObject.SetActive(value: true);
				_challengeDecoBGController.SetAnim(ChallengeDecoBG.Anim.Out);
				_extraInfoController.SetAllHide();
			}
		}

		public void SetCourseStart()
		{
			if (isPlayerActive && GameManager.IsCourseMode)
			{
				_courseTransition.SetAnim(CourseTransition.BGAnim.Out, base.MonitorIndex);
			}
		}

		public void SetChallengeBGHide()
		{
			if (isPlayerActive)
			{
				_challengeDecoBGController.gameObject.SetActive(value: false);
				_challengeBGController.gameObject.SetActive(value: false);
			}
		}

		public void SetCourseFadeIn(int courseMode)
		{
			if (isPlayerActive)
			{
				StartCoroutine(CourseFadeInCoroutine(courseMode));
			}
		}

		private IEnumerator CourseFadeInCoroutine(int courseMode)
		{
			_courseTransition.gameObject.SetActive(value: true);
			_courseTransition.SetAnim(CourseTransition.BGAnim.Idle, base.MonitorIndex);
			_courseBg.gameObject.SetActive(value: true);
			if (courseMode == 2)
			{
				_courseBg.SetAnim(CourseBGController.BGAnim.SinDani_Loop);
			}
			else
			{
				_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Loop);
			}
			yield return new WaitForSeconds(0.1f);
			_courseTransition.SetAnim(CourseTransition.BGAnim.In, base.MonitorIndex);
		}

		public void SetCourseInfo(CourseCardData infoData)
		{
			if (isPlayerActive)
			{
				_courseInfo.Prepare(infoData);
				_courseInfo.SetPlayAnim("Loop");
			}
		}

		public void ActiveCourseInfo(bool isActive)
		{
			if (isPlayerActive)
			{
				_courseInfo.gameObject.SetActive(isActive);
				if (isActive)
				{
					_courseInfo.SetPlayAnim("Loop");
				}
			}
		}

		public bool ScrollMusicRight(bool scrollEnd = true)
		{
			if (isPlayerActive)
			{
				bool flag = _musicChainList.Scroll(Direction.Right);
				if (flag)
				{
					_genreTabController.Change(_musicSelect.CurrentCategorySelect, Direction.Right);
					SetCompIconSprite();
					_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
					_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				}
				else
				{
					_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				}
				MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
				if (scrollEnd || flag)
				{
					_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
					int num;
					Notes notes;
					if (_musicSelect.IsLevelTab())
					{
						num = _musicSelect.GetDifficultyByLevel(0);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
					}
					else if (_musicSelect.IsExtraFolder(0))
					{
						num = _musicSelect.GetDifficultyByLevel(0);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num);
					}
					else
					{
						num = _musicSelect.GetCurrentDifficulty(monitorIndex);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
					}
					_selectorBgController.SetBackgroundColor(num);
					SetUpperMusicInfo(music, notes, num);
				}
				_buttonController.Pressed(InputManager.ButtonSetting.Button06);
				SetChallengeMusicData();
				CheckGhostJump();
				return flag;
			}
			return false;
		}

		public bool ScrollMusicLeft(bool scrollEnd = true)
		{
			if (isPlayerActive)
			{
				bool flag = _musicChainList.Scroll(Direction.Left);
				if (flag)
				{
					_genreTabController.Change(_musicSelect.CurrentCategorySelect, Direction.Left);
					SetCompIconSprite();
					_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
					_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				}
				else
				{
					_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				}
				MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
				if (scrollEnd || flag)
				{
					_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
					int num;
					Notes notes;
					if (_musicSelect.IsLevelTab())
					{
						num = _musicSelect.GetDifficultyByLevel(0);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
					}
					else if (_musicSelect.IsExtraFolder(0))
					{
						num = _musicSelect.GetDifficultyByLevel(0);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num);
					}
					else
					{
						num = _musicSelect.GetCurrentDifficulty(monitorIndex);
						notes = music?.ScoreData[num];
						_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
					}
					_selectorBgController.SetBackgroundColor(num);
					SetUpperMusicInfo(music, notes, num);
				}
				_buttonController.Pressed(InputManager.ButtonSetting.Button03);
				SetChallengeMusicData();
				CheckGhostJump();
				return flag;
			}
			return false;
		}

		public void SetDeployList(bool isAnimation = true, bool isChangeAnimation = false)
		{
			if (isPlayerActive)
			{
				_musicChainList.gameObject.SetActive(value: true);
				_normalChainList.gameObject.SetActive(value: false);
				_menuChainList.gameObject.SetActive(value: false);
				_genreChainList.gameObject.SetActive(value: false);
				_musicChainList.DeployMusicList(isAnimation, isChangeAnimation);
				_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				int num = (_musicSelect.IsLevelTab() ? _musicSelect.GetDifficultyByLevel(0) : _musicSelect.GetCurrentDifficulty(monitorIndex));
				if (_musicSelect.IsExtraFolder(0))
				{
					num = _musicSelect.GetDifficultyByLevel(0);
				}
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				_difficultyContoroller.DifficultyBaseIn();
				if (_musicSelect.IsExtraFolder(0))
				{
					_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num, isButtonChange: false);
				}
				else
				{
					_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
				}
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				_selectorBgController.SetBackgroundColor(num);
				CheckGhostJump();
			}
		}

		public void SetScrollMusicCard(bool isVisible)
		{
			if (isPlayerActive)
			{
				_musicChainList.SetScrollCard(isVisible);
			}
		}

		public void SetScrollGenreCard(bool isVisible)
		{
			if (isPlayerActive)
			{
				_genreChainList.SetScrollCard(isVisible);
			}
		}

		public void SetScrollOptionCard(bool isVisible)
		{
			if (isPlayerActive)
			{
				_normalChainList.SetScrollCard(isVisible);
			}
		}

		public void ScrollGenreCategoryRight()
		{
			if (isPlayerActive)
			{
				_genreChainList.Scroll(Direction.Right);
				int currentDifficulty = _musicSelect.GetCurrentDifficulty(monitorIndex);
				_buttonController.Pressed(InputManager.ButtonSetting.Button06);
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.SetDifficulty((MusicDifficultyID)currentDifficulty);
				_selectorBgController.SetBackgroundColor(currentDifficulty);
				_genreSelectorBgController.MoveIndicator(_musicSelect.CurrentCategorySelect);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.ChangeOut);
			}
		}

		public void ScrollGenreCategoryLeft()
		{
			if (isPlayerActive)
			{
				_genreChainList.Scroll(Direction.Left);
				int currentDifficulty = _musicSelect.GetCurrentDifficulty(monitorIndex);
				_buttonController.Pressed(InputManager.ButtonSetting.Button03);
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.SetDifficulty((MusicDifficultyID)currentDifficulty);
				_selectorBgController.SetBackgroundColor(currentDifficulty);
				_genreSelectorBgController.MoveIndicator(_musicSelect.CurrentCategorySelect);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.ChangeOut);
			}
		}

		public bool UpdateGenreBackground()
		{
			if (isPlayerActive && _genreSelectorBgController.GetLoopEnd())
			{
				int index = ++_genreSelectorBgController.LoopCount;
				_genreSelectorBgController.SetNextSampleImage(_musicSelect.GetGenreTexture(index));
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Loop);
				return true;
			}
			return false;
		}

		public bool ChangeGenreBackground()
		{
			if (isPlayerActive && _genreSelectorBgController.GetChangeEnd())
			{
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.ChangeIn);
				_genreSelectorBgController.SetSampleImage(_musicSelect.GetGenreTextureList());
				return true;
			}
			return false;
		}

		public void ScrollCategoryRight(InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				_musicChainList.DeployMusicList();
				_musicChainList.Play();
				int num = _musicSelect.GetCurrentDifficulty(monitorIndex);
				if (_musicSelect.IsLevelTab() || _musicSelect.IsExtraFolder(0))
				{
					num = _musicSelect.GetDifficultyByLevel(0);
				}
				_buttonController.Pressed(button);
				_genreTabController.Change(_musicSelect.CurrentCategorySelect, Direction.Right);
				SetCompIconSprite();
				_genreTabController.PressedTabButton(isRight: false);
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.InSelect);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
				Notes notes = music?.ScoreData[num];
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsExtraFolder(0))
				{
					_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num);
				}
				else
				{
					_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
				}
				_selectorBgController.SetBackgroundColor(num);
				SetUpperMusicInfo(music, notes, num);
				SetChallengeMusicData();
				CheckGhostJump();
			}
		}

		public void ScrollCategoryLeft(InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				_musicChainList.DeployMusicList();
				_musicChainList.Play();
				int num = _musicSelect.GetCurrentDifficulty(monitorIndex);
				if (_musicSelect.IsLevelTab() || _musicSelect.IsExtraFolder(0))
				{
					num = _musicSelect.GetDifficultyByLevel(0);
				}
				_buttonController.Pressed(button);
				_genreTabController.Change(_musicSelect.CurrentCategorySelect, Direction.Left);
				SetCompIconSprite();
				_genreTabController.PressedTabButton(isRight: true);
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.InSelect);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
				Notes notes = music?.ScoreData[num];
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsExtraFolder(0))
				{
					_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num);
				}
				else
				{
					_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
				}
				_selectorBgController.SetBackgroundColor(num);
				SetUpperMusicInfo(music, notes, num);
				SetChallengeMusicData();
				CheckGhostJump();
			}
		}

		public void ScrollOptionCategory(Direction direction, int optionIndex)
		{
			_normalChainList.OptionDeploy();
			_normalChainList.Play();
			_genreTabController.Change(optionIndex, direction);
			_genreTabController.PressedTabButton(direction == Direction.Left);
		}

		public void setForceDiffControlParam(MusicDifficultyID d)
		{
			_difficultyContoroller.setForceDiffParam(d);
		}

		public void AddDifficulty(MusicDifficultyID difficulty, InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_musicChainList.DeployMusicList(isAnimation: false);
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsLevelTab())
				{
					_difficultyContoroller.SetDifficultyLevelSort(difficulty);
					_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				}
				else
				{
					_difficultyContoroller.AddDifficulty();
				}
				_selectorBgController.SetBackgroundColor((int)difficulty);
				SetCompIconSprite();
				SetUpperMusicInfo(notes, difficulty);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				_buttonController.Pressed(button);
				CheckGhostJump();
			}
		}

		public void SubDifficulty(MusicDifficultyID difficulty, InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_musicChainList.DeployMusicList(isAnimation: false);
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsLevelTab())
				{
					_difficultyContoroller.SetDifficultyLevelSort(difficulty);
					_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				}
				else
				{
					_difficultyContoroller.SubDifficulty();
				}
				_selectorBgController.SetBackgroundColor((int)difficulty);
				SetCompIconSprite();
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				SetUpperMusicInfo(notes, difficulty);
				_buttonController.Pressed(button);
				CheckGhostJump();
			}
		}

		public void ChangeDifficulty(MusicDifficultyID difficulty, bool deployList = true)
		{
			if (isPlayerActive)
			{
				if (_musicSelect.IsExtraFolder(0))
				{
					difficulty = (MusicDifficultyID)_musicSelect.GetDifficultyByLevel(0);
				}
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				if (_musicSelect.IsExtraFolder(0))
				{
					_difficultyContoroller.SetDifficultyExtra(difficulty);
				}
				else
				{
					_difficultyContoroller.SetDifficulty(difficulty);
				}
				_selectorBgController.SetBackgroundColor((int)difficulty);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				SetUpperMusicInfo(notes, difficulty);
				if (deployList)
				{
					_musicChainList.DeployMusicList(isAnimation: false);
				}
			}
		}

		public void ChangeScoreKind(MusicDifficultyID difficulty)
		{
			if (isPlayerActive)
			{
				Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_musicChainList.DeployMusicList(isAnimation: false, isChangeAnimation: true);
				_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
				_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
				SetUpperMusicInfo(notes, difficulty);
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
				if (_musicSelect.IsLevelTab())
				{
					_difficultyContoroller.SetDifficultyLevelSort(difficulty);
					_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				}
				else if (_musicSelect.IsMaiList() && _musicSelect.IsVersionCategory)
				{
					_genreTabController.Change(_musicSelect.CurrentCategorySelect);
				}
				SetCompIconSprite();
				CheckGhostJump();
			}
		}

		public void AddChategoryDifficulty(MusicDifficultyID difficulty, InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				_ = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_genreChainList.DeployGenreList(isAnimation: false);
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.AddDifficulty();
				_selectorBgController.SetBackgroundColor((int)difficulty);
				_buttonController.Pressed(button);
			}
		}

		public void SubCategoryDifficulty(MusicDifficultyID difficulty, InputManager.ButtonSetting button)
		{
			if (isPlayerActive)
			{
				_ = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_genreChainList.DeployGenreList(isAnimation: false);
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.SubDifficulty();
				_selectorBgController.SetBackgroundColor((int)difficulty);
				_buttonController.Pressed(button);
			}
		}

		public void ChangeMenuDifficulty(MusicDifficultyID difficulty, InputManager.ButtonSetting button)
		{
			_menuChainList.ChangeDifficulty(difficulty);
			_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
			_difficultyContoroller.SetDifficulty(difficulty);
			_buttonController.Pressed(button);
			_selectorBgController.SetBackgroundColor((int)difficulty);
			SetUpperMusicInfo(notes, difficulty);
		}

		public void ScrollDifficultyRight()
		{
			int currentDifficulty = _musicSelect.GetCurrentDifficulty(monitorIndex);
			Notes notes = _musicSelect.GetMusic(0)?.ScoreData[currentDifficulty];
			_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			_difficultyContoroller.SubDifficulty();
			_buttonController.Pressed(InputManager.ButtonSetting.Button08);
			_buttonController.Pressed(InputManager.ButtonSetting.Button06);
			_selectorBgController.SetBackgroundColor(currentDifficulty);
			SetUpperMusicInfo(notes, (MusicDifficultyID)currentDifficulty);
		}

		public void ScrollDifficultyLeft()
		{
			int currentDifficulty = _musicSelect.GetCurrentDifficulty(monitorIndex);
			Notes notes = _musicSelect.GetMusic(0)?.ScoreData[currentDifficulty];
			_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			_difficultyContoroller.AddDifficulty();
			_buttonController.Pressed(InputManager.ButtonSetting.Button01);
			_buttonController.Pressed(InputManager.ButtonSetting.Button03);
			_selectorBgController.SetBackgroundColor(currentDifficulty);
			SetUpperMusicInfo(notes, (MusicDifficultyID)currentDifficulty);
		}

		public void ScrollRight()
		{
			if (isPlayerActive)
			{
				_menuChainList.Scroll(Direction.Right);
				_menuChainList.SetChangeConnectCardSize();
				_normalChainList.Scroll(Direction.Right);
				_buttonController.Pressed(InputManager.ButtonSetting.Button06);
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			}
		}

		public void ScrollLeft()
		{
			if (isPlayerActive)
			{
				_menuChainList.Scroll(Direction.Left);
				_menuChainList.SetChangeConnectCardSize();
				_normalChainList.Scroll(Direction.Left);
				_buttonController.Pressed(InputManager.ButtonSetting.Button03);
				_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			}
		}

		public void ScrollDifficultySelect(Direction direction, InputManager.ButtonSetting button, MusicDifficultyID difficulty)
		{
			if (isPlayerActive)
			{
				_musicChainList.Scroll(direction);
				_buttonController.Pressed(button);
				Notes notes = _musicSelect.GetMusic(0)?.ScoreData[(int)difficulty];
				_selectorBgController.SetBackgroundColor((int)difficulty);
				MechaManager.LedIf[base.MonitorIndex].SetColor((byte)button, DifficultyContoroller.GetButtonLEDColor((int)difficulty));
				_difficultyContoroller.SetButtonVisible(isVisible: false, difficulty);
				SetUpperMusicInfo(notes, difficulty);
			}
		}

		public void ScrollOptionRight(int categoryID)
		{
			if (isPlayerActive)
			{
				if (_normalChainList.Scroll(Direction.Right))
				{
					_genreTabController.Change(categoryID, Direction.Right);
				}
				_buttonController.Pressed(InputManager.ButtonSetting.Button06);
			}
		}

		public void ScrollOptionLeft(int categoryID)
		{
			if (isPlayerActive)
			{
				if (_normalChainList.Scroll(Direction.Left))
				{
					_genreTabController.Change(categoryID, Direction.Left);
				}
				_buttonController.Pressed(InputManager.ButtonSetting.Button03);
			}
		}

		public void SetVisibleButton(bool isVisible, InputManager.ButtonSetting button)
		{
			_buttonController.SetVisible(isVisible, button);
		}

		public bool GetVisibleButton(InputManager.ButtonSetting button)
		{
			return _buttonController.GetVisible((int)button);
		}

		public void ChangeButtonImage(InputManager.ButtonSetting button, int index)
		{
			_buttonController.ChangeButtonImage(button, index);
		}

		public void SetVolume(OptionHeadphonevolumeID volume, float amount)
		{
			_menuChainList.SetVolume(volume, amount);
		}

		public void SetSideMessage(string message)
		{
			_selectorBgController?.SetScrollMessage(message);
			_genreSelectorBgController?.SetScrollMessage(message);
		}

		public void SetSortSetting(SortRootID root, MusicSelectProcess.SubSequence seq)
		{
			if (isPlayerActive)
			{
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutSelect);
				_buttonController.SetVisible(false, 1, 4, 6);
				_buttonController.SetVisible(true, 3);
				_buttonController.ChangeButtonImage(InputManager.ButtonSetting.Button04, 2);
				_selectorBgController.SetActiveIndicator(isActive: false);
				switch (seq)
				{
				case MusicSelectProcess.SubSequence.Genre:
					_genreChainList.RemoveAll();
					_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Out);
					break;
				case MusicSelectProcess.SubSequence.Music:
					_musicChainList.RemoveAll();
					_genreTabController.PlayOutAnimation();
					break;
				}
				_musicChainList.gameObject.SetActive(value: false);
				_normalChainList.gameObject.SetActive(value: true);
				_menuChainList.gameObject.SetActive(value: false);
				_genreChainList.gameObject.SetActive(value: false);
				_normalChainList.SortDeploy();
				_normalChainList.Play();
				_difficultyContoroller.DifficultyBaseOut();
			}
		}

		public void SetSortValueChange(SortRootID currentSortTarget)
		{
			if (isPlayerActive)
			{
				SortData sortData = _musicSelect.GetSortData(currentSortTarget);
				_normalChainList.ValueChange(sortData.SortTypeName, "", _musicSelect.GetSortValueSprite(currentSortTarget));
			}
		}

		public void SetValueChange(string value, string valueDetails, Sprite valueSprite)
		{
			_normalChainList.ValueChange(value, valueDetails, valueSprite);
		}

		public void SetOptionPreview(OptionCategoryID category, int optionIndex, float speed)
		{
			_normalChainList.SetSpeed(category, optionIndex, speed);
		}

		public void SetOptionSlideSpeed(int optionValue)
		{
			_normalChainList.SetSlideSpeedOptionValue(optionValue);
		}

		public void SetOptionHide()
		{
			_normalChainList.Hide();
		}

		public void SetCharacterSelect()
		{
			Main.alpha = 0f;
			_menuChainList.RemoveAll();
			_buttonController.SetVisible(false, 2, 5);
			_difficultyContoroller.gameObject.SetActive(value: false);
		}

		public void OutGenreTab()
		{
			_genreTabController.PlayOutAnimation();
		}

		public void RetrunCharacterSelect()
		{
			Main.alpha = 1f;
		}

		public void PressedTouchPanel(InputManager.TouchPanelArea area, bool isLongTouch = false, bool toOut = false)
		{
			switch (area)
			{
			case InputManager.TouchPanelArea.B4:
				_menuChainList.PressedButton(Direction.Right);
				_normalChainList.Preseedbutton(Direction.Right, isLongTouch, toOut);
				break;
			case InputManager.TouchPanelArea.B5:
				_menuChainList.PressedButton(Direction.Left);
				_normalChainList.Preseedbutton(Direction.Left, isLongTouch, toOut);
				break;
			}
		}

		public void SetVisibleCardTouchButton(bool isVisible, InputManager.TouchPanelArea area)
		{
			if (isPlayerActive)
			{
				switch (area)
				{
				case InputManager.TouchPanelArea.B4:
					_normalChainList.SetVisibleButton(isVisible, Direction.Right);
					break;
				case InputManager.TouchPanelArea.B5:
					_normalChainList.SetVisibleButton(isVisible, Direction.Left);
					break;
				}
			}
		}

		public void OptionPreviewPressed(OptionCategoryID category, int optionIndex)
		{
			_normalChainList.PressedPreview(category, optionIndex);
		}

		public void PressedButton(InputManager.ButtonSetting button)
		{
			_buttonController.Pressed(button);
		}

		public void SetCharacterSelectMessage(bool isActive, string message)
		{
			_menuChainList.SetCharacterSelectMessage(isActive, message);
		}

		public void ChangeOptionCard(OptionKindID kind)
		{
			_menuChainList.ChangeOptionCard(kind);
		}

		public void UpdateDispRate(OptionDisprateID dispRate)
		{
			_musicSelect.SendMessage(new Message(ProcessType.CommonProcess, 30006, monitorIndex, dispRate));
		}

		public void UpdateAppealFrame(OptionAppealID appeal)
		{
			_musicSelect.SendMessage(new Message(ProcessType.CommonProcess, 30008, monitorIndex, appeal));
		}

		public void SetConnectMember()
		{
			_menuChainList.SetConnectMember();
		}

		public void SetMenuReset()
		{
			_musicChainList.gameObject.SetActive(value: false);
			_normalChainList.gameObject.SetActive(value: false);
			_menuChainList.gameObject.SetActive(value: true);
			_genreChainList.gameObject.SetActive(value: false);
			_menuChainList.Deploy();
			_menuChainList.Positioning(isImmediate: true);
		}

		public void SetConnectingInfo(Texture2D[] textureList, string[] playerNumList, string[] playerNameList)
		{
			if (isPlayerActive)
			{
				_connectMatchingController.SetConnectingInfo(textureList, playerNumList, playerNameList);
			}
		}

		public void SetCancelInfo()
		{
			if (isPlayerActive)
			{
				_connectMatchingController.SetCancelInfo();
			}
		}

		public void SetConnectCancelWarning()
		{
			_infoButtonController.SetVisible(true, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
			_blurObject.SetActive(value: true);
		}

		public void CloseConnectCancelWarning()
		{
			_infoButtonController.SetVisible(false, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
			_blurObject.SetActive(value: false);
		}

		public void SetForceStartWarning()
		{
			_infoButtonController.SetVisible(true, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
			_blurObject.SetActive(value: true);
		}

		public void CloseForceStartWarning()
		{
			_infoButtonController.SetVisible(false, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
			_blurObject.SetActive(value: false);
		}

		public void SetOptionSummary(UserOption option)
		{
			int num = 0;
			OptionSummaryData[] summary = _summary;
			foreach (OptionSummaryData optionSummaryData in summary)
			{
				_optionSummary.SetOptionData(num, option.GetOptionName(optionSummaryData.Category, optionSummaryData.Index), option.GetOptionValue(optionSummaryData.Category, optionSummaryData.Index));
				num++;
			}
		}

		public void SetSpeed(string speed)
		{
			_menuChainList.SetSpeed(speed);
		}

		public void SetDetils(string mirror, string trackSkip)
		{
			_menuChainList.SetDetils(mirror, trackSkip);
		}

		public void SetVisibleOptionSummary(bool isVisible)
		{
			_optionSummary.gameObject.SetActive(isVisible);
		}

		public void OnStartMusicSelect()
		{
			if (!isPlayerActive)
			{
				return;
			}
			if (_currentCoroutine != null)
			{
				StopCoroutine(_currentCoroutine);
				_currentCoroutine = null;
			}
			_musicChainList.RemoveAll();
			_menuChainList.RemoveAll();
			_normalChainList.RemoveAll();
			_genreChainList.RemoveAll();
			int num = (_musicSelect.IsLevelTab() ? _musicSelect.GetDifficultyByLevel(0) : _musicSelect.GetCurrentDifficulty(monitorIndex));
			if (_musicSelect.IsExtraFolder(0))
			{
				num = _musicSelect.GetDifficultyByLevel(0);
			}
			_buttonController.SetVisible(true, 2, 5);
			SetVisibleOptionSummary(isVisible: false);
			if (_isStartPlay)
			{
				_musicChainList.gameObject.SetActive(value: true);
				_normalChainList.gameObject.SetActive(value: false);
				_menuChainList.gameObject.SetActive(value: false);
				_genreChainList.gameObject.SetActive(value: false);
				_musicChainList.DeployMusicList();
				_musicChainList.Play();
			}
			_buttonController.ChangeButtonImage(InputManager.ButtonSetting.Button04, 5);
			MusicSelectProcess.MusicSelectData music = _musicSelect.GetMusic(0);
			if (music != null)
			{
				Notes notes = music.ScoreData[num];
				SetUpperMusicInfo(music, notes, num);
			}
			_difficultyContoroller.gameObject.SetActive(value: true);
			_difficultyContoroller.SetRemasterEnable(_musicSelect.IsRemasterEnable());
			_difficultyContoroller.DifficultyBaseIn();
			if (_musicSelect.IsExtraFolder(0))
			{
				_difficultyContoroller.SetDifficultyExtra((MusicDifficultyID)num, isButtonChange: false);
			}
			else
			{
				_difficultyContoroller.SetDifficulty((MusicDifficultyID)num);
			}
			_selectorBgController.SetBackgroundColor(num);
			SetSideMessage(CommonMessageID.Scroll_Music_Select.GetName());
			List<TabDataBase> list = new List<TabDataBase>();
			foreach (MusicSelectProcess.GenreSelectData item in _musicSelect.GenreSelectDataList[monitorIndex])
			{
				Sprite tabSprite = GetTabSprite(item);
				string tabString = getTabString(item);
				Color tabColor = getTabColor(item);
				list.Add(new TabDataBase(tabColor, tabSprite, tabString));
			}
			_genreTabController.PlayInAnimation();
			SetCompIconSprite();
			_genreTabController.SortType2Genre(list, 0);
			_genreTabController.Change(_musicSelect.CurrentCategorySelect);
			_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Out);
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.In);
			_selectorBgController.SetActiveIndicator(isActive: true);
			_selectorBgController.PrepareIndicator(_musicSelect.GetCurrentListIndex(monitorIndex), _musicSelect.GetCurrentCategoryMax(monitorIndex));
			_selectorBgController.UpdateIndicator(_musicSelect.GetCurrentListIndex(monitorIndex));
		}

		public void OnStartGenreSelect()
		{
			if (isPlayerActive)
			{
				if (_currentCoroutine != null)
				{
					StopCoroutine(_currentCoroutine);
					_currentCoroutine = null;
				}
				_genreTabController.PlayOutAnimation();
				_musicChainList.RemoveAll();
				_menuChainList.RemoveAll();
				_normalChainList.RemoveAll();
				_genreChainList.RemoveAll();
				_buttonController.SetVisible(true, 3);
				_buttonController.SetVisible(false, 4);
				SetVisibleOptionSummary(isVisible: false);
				if (_isStartPlay)
				{
					_musicChainList.gameObject.SetActive(value: false);
					_normalChainList.gameObject.SetActive(value: false);
					_menuChainList.gameObject.SetActive(value: false);
					_genreChainList.gameObject.SetActive(value: true);
					_genreChainList.DeployGenreList();
					_genreChainList.Play();
				}
				_buttonController.ChangeButtonImage(InputManager.ButtonSetting.Button04, 5);
				DifficultyButtonCheck();
				_difficultyContoroller.gameObject.SetActive(value: true);
				_difficultyContoroller.SetRemasterEnable(enable: true);
				_difficultyContoroller.DifficultyBaseIn();
				_difficultyContoroller.SetDifficulty((MusicDifficultyID)_musicSelect.GetCurrentDifficulty(monitorIndex));
				SetSideMessage(CommonMessageID.Scroll_Category_Select.GetName());
				_selectorBgController.SetBackgroundColor(_musicSelect.GetCurrentDifficulty(monitorIndex));
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.Out);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.In);
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Loop);
				int num = _musicSelect.GenreSelectDataList[base.MonitorIndex].Count;
				if (!GameManager.IsFreedomMode)
				{
					num--;
				}
				if (_musicSelect.CurrentCategorySelect != num - 1)
				{
					_buttonController.SetVisible(true, 2);
				}
				if (_musicSelect.CurrentCategorySelect != 0)
				{
					_buttonController.SetVisible(true, 5);
				}
				if (_musicSelect.CurrentCategorySelect != num - 1)
				{
					_buttonController.SetVisible(true, 2);
				}
				if (_musicSelect.CurrentCategorySelect != 0)
				{
					_buttonController.SetVisible(true, 5);
				}
				_genreSelectorBgController.InitIndicator(num);
				_selectorBgController.SetActiveIndicator(isActive: false);
				_genreSelectorBgController.MoveIndicator(_musicSelect.CurrentCategorySelect);
				_genreSelectorBgController.SetSampleImage(_musicSelect.GetGenreTextureList());
			}
		}

		public void OnStartDifficultySelect(MusicDifficultyID selectDifficulty, Action<MusicSelectMonitor> buttonCheck)
		{
			if (isPlayerActive)
			{
				_currentCoroutine = StartCoroutine(OnStartDifficultySelectCoroutine(selectDifficulty, buttonCheck));
			}
		}

		private IEnumerator OnStartDifficultySelectCoroutine(MusicDifficultyID selectDifficulty, Action<MusicSelectMonitor> buttonCheck)
		{
			_buttonController.SetVisible(true, InputManager.ButtonSetting.Button05);
			_buttonController.ChangeButtonImage(InputManager.ButtonSetting.Button04, 5);
			_difficultyContoroller.DifficultyBaseIn();
			_difficultyContoroller.SetButtonVisible(isVisible: false);
			_difficultyContoroller.SetButtonVisible(isVisible: false, selectDifficulty);
			_selectorBgController.SetActiveIndicator(isActive: false);
			_musicChainList.gameObject.SetActive(value: true);
			_normalChainList.gameObject.SetActive(value: false);
			_menuChainList.gameObject.SetActive(value: false);
			_genreChainList.gameObject.SetActive(value: false);
			_musicChainList.DeployDifficulty(selectDifficulty);
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutSelect);
			_buttonController.SetVisible(false, 2, 5, 1, 6);
			yield return new WaitForSeconds(1f);
			buttonCheck(this);
			_currentCoroutine = null;
		}

		public void OnBackDifficultySelect()
		{
			if (isPlayerActive)
			{
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutBack);
			}
		}

		public void OnStartMenuSelect(Action<MusicSelectMonitor> onAction)
		{
			if (isPlayerActive)
			{
				_buttonController.SetVisible(false, 1, 6, 2, 3, 4, 5);
				_normalChainList.RemoveAll();
				SetSideMessage(CommonMessageID.Scroll_Play_Setting.GetName());
				_currentCoroutine = StartCoroutine(OnStartMenuSelectCoroutine(onAction));
			}
		}

		private IEnumerator OnStartMenuSelectCoroutine(Action<MusicSelectMonitor> onAction)
		{
			_musicChainList.RemoveOut(this);
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutSelect);
			_selectorBgController.SetBackgroundColor(_musicSelect.DifficultySelectIndex[monitorIndex]);
			_difficultyContoroller.DifficultyBaseOut();
			_musicChainList.gameObject.SetActive(value: false);
			_normalChainList.gameObject.SetActive(value: false);
			_menuChainList.gameObject.SetActive(value: true);
			_genreChainList.gameObject.SetActive(value: false);
			yield return new WaitForSeconds(0.26f);
			_menuChainList.Deploy();
			_menuChainList.Positioning(isImmediate: true, isAnimation: true);
			_menuChainList.Play();
			yield return new WaitForSeconds(0.74f);
			onAction?.Invoke(this);
			_currentCoroutine = null;
		}

		public void OnBackMenuSelect()
		{
			if (isPlayerActive)
			{
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutBack);
				_menuChainList.RemoveOut(this);
			}
		}

		public void SetOptionCard(Action action)
		{
			_buttonController.SetVisible(false, 2, 4, 5);
			SetSideMessage(CommonMessageID.Scroll_Option.GetName());
			_currentCoroutine = StartCoroutine(SetOptionCardCoroutine(action));
			List<TabDataBase> list = new List<TabDataBase>();
			for (OptionCategoryID optionCategoryID = OptionCategoryID.SpeedSetting; optionCategoryID < OptionCategoryID.End; optionCategoryID++)
			{
				Sprite sprite = _optionSprite[(int)optionCategoryID];
				string title = optionCategoryID.GetName();
				Color baseColor = Utility.ConvertColor(optionCategoryID.GetMainColor());
				list.Add(new TabDataBase(baseColor, sprite, title));
			}
			_genreTabController.PlayInAnimation();
			_genreTabController.SetLeftIcon(null);
			_genreTabController.SetRightIcon(null);
			_genreTabController.SortType2Genre(list, 0);
			_genreTabController.Change(0);
		}

		private IEnumerator SetOptionCardCoroutine(Action action)
		{
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.OutSelect);
			_menuChainList.RemoveOut(this);
			yield return new WaitForSeconds(0.68f);
			_selectorBgController.SetBackgroundColor(7);
			_musicChainList.gameObject.SetActive(value: false);
			_normalChainList.gameObject.SetActive(value: true);
			_menuChainList.gameObject.SetActive(value: false);
			_genreChainList.gameObject.SetActive(value: false);
			yield return new WaitForSeconds(0.3f);
			_normalChainList.OptionDeploy();
			_normalChainList.Play();
			_buttonController.SetVisible(true, 0, 2, 5, 7);
			action();
			_currentCoroutine = null;
		}

		public void OnGameStartWait()
		{
			_menuChainList.PlayOut();
			SetCourseStart();
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.InSelect);
			_buttonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
		}

		public void OnGameStart()
		{
			if (isPlayerActive)
			{
				_buttonController.SetVisible(false, 0, 1, 2, 3, 4, 5, 6, 7);
				_musicChainList.RemoveOut(this);
				_genreTabController.PlayOutAnimation();
				_difficultyContoroller.DifficultyBaseOut();
				_genreChainList.RemoveOut(this);
				_menuChainList.RemoveOut(this);
				_normalChainList.RemoveOut(this);
				SetChallengeBGGameStart();
				SetCourseStart();
				_genreSelectorBgController.AnimPlay(GenreBackgroundController.AnimationType.Out);
				_selectorBgController.Play(SelectorBackgroundController.AnimationType.Out);
			}
		}

		public void OnClientTimeOut()
		{
			if (isPlayerActive)
			{
				_genreTabController.PlayOutAnimation();
				_musicChainList.RemoveOut(this);
				_normalChainList.RemoveOut(this);
				_menuChainList.Deploy();
				_menuChainList.Positioning(isImmediate: true, isAnimation: true);
				_menuChainList.PlayOut();
			}
		}

		private void DifficultyButtonCheck()
		{
			MusicDifficultyID currentDifficulty = (MusicDifficultyID)_musicSelect.GetCurrentDifficulty(monitorIndex);
			_buttonController.SetVisible(currentDifficulty != MusicDifficultyID.Strong, 1);
			_buttonController.SetVisible(currentDifficulty != MusicDifficultyID.Basic, 6);
		}

		public void PlayFreedomModeTimeUp()
		{
			StartCoroutine(FreedomModeTimeUpCoroutine());
		}

		private IEnumerator FreedomModeTimeUpCoroutine()
		{
			_selectorBgController.Play(SelectorBackgroundController.AnimationType.Out);
			_menuChainList.RemoveOut(this);
			_normalChainList.RemoveOut(this);
			_musicChainList.RemoveOut(this);
			_genreChainList.RemoveOut(this);
			_buttonController.SetVisible(false, 1, 2, 5, 6);
			_freedomModeTimeUpAnimator.gameObject.SetActive(value: true);
			_freedomModeTimeUpAnimator.Play(Animator.StringToHash("In"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeTimeUpAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeTimeUpAnimator.Play(Animator.StringToHash("Loop"));
		}

		public void PlayFreedomModeOut()
		{
			StartCoroutine(FreedomModeTimeUpOutCoroutine());
		}

		private IEnumerator FreedomModeTimeUpOutCoroutine()
		{
			_freedomModeTimeUpAnimator.Play(Animator.StringToHash("Out"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeTimeUpAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeTimeUpAnimator.gameObject.SetActive(value: false);
		}

		public void SetRecruitInfo(string path)
		{
			if (isPlayerActive)
			{
				_connectMatchingController.SetRecritInfo(_assetManager.GetJacketThumbTexture2D(path));
			}
		}
	}
}
