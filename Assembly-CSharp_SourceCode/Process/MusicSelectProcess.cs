using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;
using Monitor;
using PartyLink;
using Process.SubSequence;
using UnityEngine;

namespace Process
{
	public class MusicSelectProcess : ProcessBase, IMusicSelectProcess, IMusicSelectProcessProcessing
	{
		private enum MainSequence
		{
			Init,
			Fade,
			FirstStaging,
			Update,
			ReleaseWait,
			Release
		}

		public enum SubSequence
		{
			Music,
			Genre,
			SortSetting,
			Difficulty,
			Menu,
			Option,
			Character
		}

		public enum MenuType
		{
			Matching,
			GameStart,
			Option,
			Volume,
			Max
		}

		public class MusicSelectDetailData
		{
			public int musicId = -1;

			public int difficultyId = -1;

			public int targetPlayer = -1;

			public int startLife = -1;

			public int challengeUnlockDiff = -1;

			public int nextRelaxDay = -1;

			public bool infoEnable;

			public bool jumpOtherCategoryStandard;

			public bool jumpOtherCategoryDeluxe;

			public MusicDifficultyID jumpOtherCategoryLevelStd;

			public MusicDifficultyID jumpOtherCategoryLevelDx;
		}

		public class MusicSelectData
		{
			public MusicData MusicData;

			public List<Notes> ScoreData;

			public int Difficulty;

			public int RatingPlayer = -1;

			public GhostManager.GhostTarget GhostTarget;

			public MusicSelectData(MusicData data, List<Notes> score, int difficulty)
			{
				MusicData = data;
				ScoreData = score;
				Difficulty = difficulty;
				RatingPlayer = -1;
				GhostTarget = GhostManager.GhostTarget.End;
			}

			public MusicSelectData(MusicData data, List<Notes> score, int difficulty, int ratingPlayer, GhostManager.GhostTarget target)
			{
				MusicData = data;
				ScoreData = score;
				Difficulty = difficulty;
				RatingPlayer = ratingPlayer;
				GhostTarget = target;
			}
		}

		public class GenreSelectData
		{
			public int categoryID;

			public int totalMusicNum;

			public bool isExtra;

			public bool isTournament;

			public Color genreCategoryColor = Color.black;

			public int fcNum;

			public int fcpNum;

			public int apNum;

			public int appNum;

			public int fsNum;

			public int fspNum;

			public int fsdNum;

			public int fsdpNum;

			public Dictionary<MusicClearrankID, int> rankNumList = new Dictionary<MusicClearrankID, int>();

			public Dictionary<MusicClearrankID, int> overRankNumList = new Dictionary<MusicClearrankID, int>();

			public void InputRank(MusicClearrankID rank)
			{
				if (rankNumList.ContainsKey(rank))
				{
					rankNumList[rank]++;
				}
				else
				{
					rankNumList[rank] = 1;
				}
				for (MusicClearrankID musicClearrankID = rank; musicClearrankID >= MusicClearrankID.Rank_D; musicClearrankID--)
				{
					if (overRankNumList.ContainsKey(musicClearrankID))
					{
						overRankNumList[musicClearrankID]++;
					}
					else
					{
						overRankNumList[musicClearrankID] = 1;
					}
				}
			}

			public void InputCombo(PlayComboflagID comboFlag)
			{
				switch (comboFlag)
				{
				default:
					return;
				case PlayComboflagID.AllPerfectPlus:
					appNum++;
					goto case PlayComboflagID.AllPerfect;
				case PlayComboflagID.AllPerfect:
					apNum++;
					goto case PlayComboflagID.Gold;
				case PlayComboflagID.Gold:
					fcpNum++;
					break;
				case PlayComboflagID.Silver:
					break;
				}
				fcNum++;
			}

			public void InputSync(PlaySyncflagID syncFlag)
			{
				switch (syncFlag)
				{
				default:
					return;
				case PlaySyncflagID.SyncHi:
					fsdpNum++;
					goto case PlaySyncflagID.SyncLow;
				case PlaySyncflagID.SyncLow:
					fsdNum++;
					goto case PlaySyncflagID.ChainHi;
				case PlaySyncflagID.ChainHi:
					fspNum++;
					break;
				case PlaySyncflagID.ChainLow:
					break;
				}
				fsNum++;
			}

			public int GetRankNum(MusicClearrankID rank)
			{
				if (rankNumList.ContainsKey(rank))
				{
					return rankNumList[rank];
				}
				return 0;
			}

			public int GetOverRankNum(MusicClearrankID rank)
			{
				if (overRankNumList.ContainsKey(rank))
				{
					return overRankNumList[rank];
				}
				return 0;
			}
		}

		public class CombineMusicSelectData
		{
			public List<MusicSelectData> musicSelectData;

			public MusicSelectDetailData msDetailData;

			public bool existStandardScore { get; set; }

			public bool existDeluxeScore { get; set; }

			public bool isWaitConnectScore { get; set; }

			public CombineMusicSelectData()
			{
				existStandardScore = false;
				existDeluxeScore = false;
				isWaitConnectScore = false;
				musicSelectData = new List<MusicSelectData>();
				musicSelectData.Clear();
				msDetailData = new MusicSelectDetailData();
			}

			public CombineMusicSelectData(int musicId, int difficultyId, GhostManager.GhostTarget target, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				inputMusicSelectData_old(musicId, difficultyId, -1, target, ref inputMusicId, isOtherTypeInput);
			}

			public CombineMusicSelectData(MusicSelectDetailData msdd, GhostManager.GhostTarget target, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				inputMusicSelectData(msdd, -1, target, ref inputMusicId, isOtherTypeInput);
				msDetailData = msdd;
			}

			public CombineMusicSelectData(int musicId, int difficultyId, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				inputMusicSelectData_old(musicId, difficultyId, -1, GhostManager.GhostTarget.End, ref inputMusicId, isOtherTypeInput);
			}

			public CombineMusicSelectData(MusicSelectDetailData msdd, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				inputMusicSelectData(msdd, -1, GhostManager.GhostTarget.End, ref inputMusicId, isOtherTypeInput);
				msDetailData = msdd;
			}

			public CombineMusicSelectData(int musicId, int difficultyId, int ratingPlayer, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				int inputMusicId2 = -1;
				inputMusicSelectData_old(musicId, difficultyId, ratingPlayer, GhostManager.GhostTarget.End, ref inputMusicId2, isOtherTypeInput);
			}

			public CombineMusicSelectData(MusicSelectDetailData msdd, int ratingPlayer, ref int inputMusicId, bool isOtherTypeInput = true)
			{
				int inputMusicId2 = -1;
				inputMusicSelectData(msdd, ratingPlayer, GhostManager.GhostTarget.End, ref inputMusicId2, isOtherTypeInput);
				msDetailData = msdd;
			}

			public CombineMusicSelectData(int musicId, int difficultyId, bool isOtherTypeInput = true)
			{
				int inputMusicId = -1;
				inputMusicSelectData_old(musicId, difficultyId, -1, GhostManager.GhostTarget.End, ref inputMusicId, isOtherTypeInput);
			}

			public CombineMusicSelectData(MusicSelectDetailData msdd, bool isOtherTypeInput = true)
			{
				int inputMusicId = -1;
				inputMusicSelectData(msdd, -1, GhostManager.GhostTarget.End, ref inputMusicId, isOtherTypeInput);
				msDetailData = msdd;
			}

			public bool isChangeScoreKind()
			{
				if (existStandardScore)
				{
					return existDeluxeScore;
				}
				return false;
			}

			public bool IsJumpGhostBattle(ConstParameter.ScoreKind scoreType, int diff)
			{
				if (musicSelectData[(int)scoreType] != null)
				{
					bool flag = musicSelectData[(int)scoreType].MusicData.GetID() >= 10000;
					if (flag && msDetailData.jumpOtherCategoryDeluxe && diff >= (int)msDetailData.jumpOtherCategoryLevelDx)
					{
						return true;
					}
					if (!flag && msDetailData.jumpOtherCategoryStandard && diff >= (int)msDetailData.jumpOtherCategoryLevelStd)
					{
						return true;
					}
				}
				return false;
			}

			public int GetID(ConstParameter.ScoreKind scoreType)
			{
				int result = 0;
				if (musicSelectData[(int)scoreType] != null)
				{
					result = musicSelectData[(int)scoreType].MusicData.GetID();
				}
				else if (isWaitConnectScore)
				{
					result = 2;
				}
				return result;
			}

			private int inputMusicSelectData_old(int musicId, int difficultyId, int ratingPlayer, GhostManager.GhostTarget target, ref int inputMusicId, bool isOtherTypeInput)
			{
				existStandardScore = false;
				existDeluxeScore = false;
				isWaitConnectScore = false;
				int num = -1;
				MusicData music = Singleton<DataManager>.Instance.GetMusic(musicId);
				List<Notes> notesList = Singleton<NotesListManager>.Instance.GetNotesList()[musicId].NotesList;
				musicSelectData = new List<MusicSelectData>();
				MusicData data = music;
				MusicData data2 = music;
				List<Notes> score = notesList;
				List<Notes> score2 = notesList;
				if (musicId < 10000)
				{
					existStandardScore = true;
					num = musicId + 10000;
				}
				else if (10000 < musicId && musicId < 20000)
				{
					existDeluxeScore = true;
					num = musicId - 10000;
				}
				if (num != -1 && isOtherTypeInput)
				{
					MusicData music2 = Singleton<DataManager>.Instance.GetMusic(num);
					if (Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(num))
					{
						NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num];
						List<Notes> notesList2 = notesWrapper.NotesList;
						if (notesWrapper.IsPlayable)
						{
							inputMusicId = num;
							if (num < 10000)
							{
								existStandardScore = true;
								data = music2;
								score = notesList2;
							}
							else if (10000 < num && num < 20000)
							{
								existDeluxeScore = true;
								data2 = music2;
								score2 = notesList2;
							}
						}
					}
				}
				musicSelectData.Add(new MusicSelectData(data, score, difficultyId, ratingPlayer, target));
				musicSelectData.Add(new MusicSelectData(data2, score2, difficultyId, ratingPlayer, target));
				return inputMusicId;
			}

			private int inputMusicSelectData(MusicSelectDetailData msdd, int ratingPlayer, GhostManager.GhostTarget target, ref int inputMusicId, bool isOtherTypeInput)
			{
				existStandardScore = false;
				existDeluxeScore = false;
				isWaitConnectScore = false;
				int num = -1;
				MusicData music = Singleton<DataManager>.Instance.GetMusic(msdd.musicId);
				List<Notes> notesList = Singleton<NotesListManager>.Instance.GetNotesList()[msdd.musicId].NotesList;
				musicSelectData = new List<MusicSelectData>();
				MusicData data = music;
				MusicData data2 = music;
				List<Notes> score = notesList;
				List<Notes> score2 = notesList;
				if (msdd.musicId < 10000)
				{
					existStandardScore = true;
					num = msdd.musicId + 10000;
				}
				else if (10000 < msdd.musicId && msdd.musicId < 20000)
				{
					existDeluxeScore = true;
					num = msdd.musicId - 10000;
				}
				if (num != -1 && isOtherTypeInput)
				{
					MusicData music2 = Singleton<DataManager>.Instance.GetMusic(num);
					if (Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(num))
					{
						NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num];
						List<Notes> notesList2 = notesWrapper.NotesList;
						if (notesWrapper.IsPlayable)
						{
							inputMusicId = num;
							if (num < 10000)
							{
								existStandardScore = true;
								data = music2;
								score = notesList2;
							}
							else if (10000 < num && num < 20000)
							{
								existDeluxeScore = true;
								data2 = music2;
								score2 = notesList2;
							}
						}
					}
				}
				musicSelectData.Add(new MusicSelectData(data, score, msdd.difficultyId, ratingPlayer, target));
				musicSelectData.Add(new MusicSelectData(data2, score2, msdd.difficultyId, ratingPlayer, target));
				return inputMusicId;
			}
		}

		public class LevelCategoryData
		{
			public int Category;

			public int Index;
		}

		public class CategoryData
		{
			public MedalData[] MedalDatas;
		}

		public class MedalData
		{
			public MusicClearrankID MinimumClearRank;

			public PlayComboflagID MinimumCombo;

			public MedalData()
			{
				MinimumClearRank = MusicClearrankID.Rank_SSSP;
				MinimumCombo = PlayComboflagID.AllPerfectPlus;
			}

			public void Failed()
			{
				MinimumClearRank = MusicClearrankID.Rank_D;
				MinimumCombo = PlayComboflagID.None;
			}
		}

		public static readonly Cue[] DifficultySe = new Cue[5]
		{
			Cue.SE_LEVEL_BASIC,
			Cue.SE_LEVEL_ADVANCED,
			Cue.SE_LEVEL_EXPERT,
			Cue.SE_LEVEL_MASTER,
			Cue.SE_LEVEL_REMASTER
		};

		private SortTabID _categorySortSetting;

		private SortTabID _beforeCategorySortSetting;

		private SortMusicID _musicSortSetting;

		private MainSequence _mainSequence;

		private List<ReadOnlyCollection<CombineMusicSelectData>> _combineMusicDataList;

		private List<CombineMusicSelectData> _connectCombineMusicDataList;

		private ReadOnlyCollection<CombineMusicSelectData> _newCombineMusicDataList;

		private ReadOnlyCollection<CombineMusicSelectData> _combineRankingDataList;

		private ReadOnlyCollection<CombineMusicSelectData>[] _combineRatingDataList;

		private ReadOnlyCollection<CombineMusicSelectData>[] _combineBonusDataList;

		private ReadOnlyCollection<CombineMusicSelectData>[] _combineGhostDataList;

		private ReadOnlyCollection<CombineMusicSelectData>[] _combineMapTaskDataList;

		private ReadOnlyCollection<CombineMusicSelectData>[] _combineChallengeDataList;

		private Dictionary<int, ReadOnlyCollection<CombineMusicSelectData>> _combineTournamentDataListList;

		private ReadOnlyCollection<CombineMusicSelectData> _combineMaiListDataList;

		private List<GenreSelectData>[] _genreSelectDataList;

		private ReadOnlyDictionary<string, Sprite> _optionValueSprites;

		private ReadOnlyDictionary<string, Sprite> _sortValueSprites;

		private List<CategoryData>[] _categoryMedalList;

		private SequenceBase[][] _subSequenceArray;

		private bool _isExtraExpand;

		private SubSequence[] _currentPlayerSubSequence;

		private SubSequence[] _beforePlayerSubSequence;

		private readonly Dictionary<int, LevelCategoryData[]> _levelCategoryPositionList = new Dictionary<int, LevelCategoryData[]>();

		private CharacterSelectProces _characterSelectProces;

		private bool _isShowCancelMessage;

		private bool[] _isUseCharacterSelect;

		private float _messageWaitTimer;

		private float _timeCounter;

		private int _frameCount;

		private int _currentExtraCategoryCount;

		private int _extraCategoryCount;

		private int _extraSlotFlag;

		private int _connectNum;

		private MusicDifficultyID[] _otherDifficulty;

		private bool _isHostWait;

		private bool _clientFinishDeploy;

		private bool _isTimeUpConnect;

		private const bool _isDebugForceOpenCategory = false;

		public SortTabID CategorySortSetting => _categorySortSetting;

		public bool IsLevelCategory { get; private set; }

		public bool IsVersionCategory { get; private set; }

		public bool IsScoreSort { get; private set; }

		public bool IsConnectCategoryEnable { get; private set; }

		public MusicDifficultyID[] CurrentDifficulty { get; set; }

		public MenuType[] CurrentSelectMenu { get; set; }

		public ConstParameter.ScoreKind ScoreType { get; set; }

		public int SortDecidePlayer { get; set; }

		public List<ReadOnlyCollection<CombineMusicSelectData>> CombineMusicDataList => _combineMusicDataList;

		public List<GenreSelectData>[] GenreSelectDataList => _genreSelectDataList;

		public List<string> CategoryNameList { get; private set; }

		public SortRootID CurrentSortRootID { get; set; }

		public int CurrentMusicSelect { get; set; }

		public int CurrentCategorySelect { get; set; }

		public int[] DifficultySelectIndex { get; set; }

		public bool SharedIsInputLock { get; set; }

		public bool SharedSlideScrollToRight { get; set; }

		public float SharedSlideScrollTime { get; set; }

		public int SharedSlideScrollCount { get; set; }

		public bool[] IsPreparationCompletes { get; set; }

		public int PlayingScoreID { get; set; }

		public bool[] IsForceMusicBackConfirm { get; private set; }

		public bool IsForceMusicBack { get; set; }

		public int IsForceMusicBackPlayer { get; private set; }

		public bool IsConnectingMusic { get; set; }

		public MusicSelectMonitor[] MonitorArray { get; private set; }

		public RecruitInfo RecruitData { get; private set; }

		public bool JoinActive { get; set; }

		public bool RecruitActive { get; set; }

		public bool RecruitCancel { get; set; }

		public bool PrepareFinish { get; set; }

		public bool BackSetting { get; set; }

		public bool ConnectMusicAllDecide { get; set; }

		public void SendMessage(Message message)
		{
			container.processManager.SendMessage(message);
		}

		public MusicSelectProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		private void SyncNext(SubSequence nextSequence)
		{
			for (int i = 0; i < _currentPlayerSubSequence.Length; i++)
			{
				Next(i, nextSequence);
			}
		}

		private void Next(int playerIndex, SubSequence nextSequence)
		{
			_subSequenceArray[playerIndex][(int)_currentPlayerSubSequence[playerIndex]].Reset();
			_beforePlayerSubSequence[playerIndex] = _currentPlayerSubSequence[playerIndex];
			_currentPlayerSubSequence[playerIndex] = nextSequence;
			_subSequenceArray[playerIndex][(int)nextSequence].OnStartSequence();
		}

		private void OnReturnCharacterSelect(int playerIndex)
		{
			Next(playerIndex, SubSequence.Menu);
			MonitorArray[playerIndex].RetrunCharacterSelect();
			SetInputLockInfo(playerIndex, 1500f);
		}

		public override void OnStart()
		{
			if (Manager.Party.Party.Party.Get() == null)
			{
				Manager.Party.Party.Party.CreateManager(new PartyLink.Party.InitParam());
			}
			for (int i = 2; i < 4; i++)
			{
				Singleton<UserDataManager>.Instance.GetUserData(i).Initialize();
				Singleton<UserDataManager>.Instance.SetDefault(i);
			}
			_combineMusicDataList = new List<ReadOnlyCollection<CombineMusicSelectData>>();
			CategoryNameList = new List<string>();
			_genreSelectDataList = new List<GenreSelectData>[2];
			PlayingScoreID = 0;
			IsForceMusicBackConfirm = new bool[2];
			IsForceMusicBack = false;
			IsForceMusicBackPlayer = 0;
			ScoreType = ConstParameter.ScoreKind.Deluxe;
			IsConnectingMusic = false;
			RecruitData = null;
			JoinActive = false;
			RecruitActive = false;
			RecruitCancel = false;
			PrepareFinish = false;
			BackSetting = false;
			ConnectMusicAllDecide = false;
			IsConnectCategoryEnable = false;
			_connectNum = 0;
			_otherDifficulty = new MusicDifficultyID[2];
			_clientFinishDeploy = false;
			for (int j = 0; j < 2; j++)
			{
				_otherDifficulty[j] = MusicDifficultyID.Basic;
				IsForceMusicBackConfirm[j] = false;
			}
			Singleton<GhostManager>.Instance.UpdateGhost();
			Singleton<NotesListManager>.Instance.CreateScore();
			_categoryMedalList = new List<CategoryData>[2];
			_isUseCharacterSelect = new bool[2];
			for (int k = 0; k < 2; k++)
			{
				_categoryMedalList[k] = new List<CategoryData>();
				_genreSelectDataList[k] = new List<GenreSelectData>();
			}
			if (GameManager.IsCourseMode)
			{
				registerCourseMusic();
			}
			else
			{
				registerMusic();
			}
			CategoryTabSort(0);
			GameObject prefs = Resources.Load<GameObject>("Process/MusicSelect/MusicSelectProcess");
			MonitorArray = new MusicSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<MusicSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<MusicSelectMonitor>()
			};
			_musicSortSetting = GameManager.SortMusicSetting;
			_categorySortSetting = GameManager.SortCategorySetting;
			_beforeCategorySortSetting = _categorySortSetting;
			for (int l = 0; l < MonitorArray.Length; l++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry)
				{
					SortDecidePlayer = l;
					break;
				}
			}
			CurrentSelectMenu = new MenuType[MonitorArray.Length];
			CurrentDifficulty = new MusicDifficultyID[MonitorArray.Length];
			ScoreType = (ConstParameter.ScoreKind)GameManager.SelectScoreType;
			for (int m = 0; m < MonitorArray.Length; m++)
			{
				CurrentDifficulty[m] = (MusicDifficultyID)GameManager.SelectDifficultyID[m];
				CurrentSelectMenu[m] = MenuType.GameStart;
			}
			bool isForceChangeMusic = GameManager.IsForceChangeMusic;
			for (int n = 0; n < MonitorArray.Length; n++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(n);
				if (!userData.IsEntry)
				{
					continue;
				}
				if (GameManager.MusicTrackNumber == 1)
				{
					bool flag = false;
					UserData.UserIDType userType = userData.UserType;
					if (userType == UserData.UserIDType.Exist)
					{
						UserExtend extend = userData.Extend;
						GameManager.SelectDifficultyID[n] = extend.SelectDifficultyID;
						GameManager.SelectMusicID[0] = extend.SelectMusicID;
						GameManager.SelectMusicID[1] = extend.SelectMusicID;
						GameManager.SelectScoreType = extend.SelectScoreType;
						flag = true;
					}
					if (flag)
					{
						break;
					}
				}
				else
				{
					UserExtend extend2 = userData.Extend;
					GameManager.SelectDifficultyID[n] = extend2.SelectDifficultyID;
					if (GameManager.SelectMusicID[n] != extend2.SelectMusicID)
					{
						GameManager.SelectMusicID[0] = extend2.SelectMusicID;
						GameManager.SelectMusicID[1] = extend2.SelectMusicID;
						break;
					}
				}
			}
			IsLevelCategory = false;
			IsVersionCategory = false;
			IsScoreSort = false;
			if (GameManager.IsCourseMode)
			{
				_categorySortSetting = SortTabID.Version;
				IsVersionCategory = true;
			}
			OnSortChange(SortDecidePlayer);
			Dictionary<string, Sprite> dictionary = new Dictionary<string, Sprite>();
			Sprite[] array = Resources.LoadAll<Sprite>("Process/MusicSelect/Sprites/Option");
			foreach (Sprite sprite in array)
			{
				dictionary.Add(sprite.name, sprite);
			}
			_optionValueSprites = new ReadOnlyDictionary<string, Sprite>(dictionary);
			dictionary = new Dictionary<string, Sprite>();
			array = Resources.LoadAll<Sprite>("Process/MusicSelect/Sprites/Sort");
			foreach (Sprite sprite2 in array)
			{
				dictionary.Add(sprite2.name, sprite2);
			}
			_sortValueSprites = new ReadOnlyDictionary<string, Sprite>(dictionary);
			_subSequenceArray = new SequenceBase[MonitorArray.Length][];
			_currentPlayerSubSequence = new SubSequence[MonitorArray.Length];
			_beforePlayerSubSequence = new SubSequence[MonitorArray.Length];
			IsPreparationCompletes = new bool[MonitorArray.Length];
			DifficultySelectIndex = new int[MonitorArray.Length];
			for (int num2 = 0; num2 < MonitorArray.Length; num2++)
			{
				bool isEntry = Singleton<UserDataManager>.Instance.GetUserData(num2).IsEntry;
				MonitorArray[num2].SetOptionSummary(Singleton<UserDataManager>.Instance.GetUserData(num2).Option);
				IsPreparationCompletes[num2] = false;
				DifficultySelectIndex[num2] = -1;
				MonitorArray[num2].gameObject.name = "MusicSelectMonitor:" + num2;
				_isUseCharacterSelect[num2] = false;
				if (GameManager.IsCourseMode)
				{
					_currentPlayerSubSequence[num2] = SubSequence.Menu;
				}
				else if (GameManager.MusicTrackNumber == 1)
				{
					_currentPlayerSubSequence[num2] = SubSequence.Genre;
				}
				else
				{
					_currentPlayerSubSequence[num2] = SubSequence.Music;
				}
				_beforePlayerSubSequence[num2] = _currentPlayerSubSequence[num2];
				_subSequenceArray[num2] = new SequenceBase[7]
				{
					new MusicSelectSequence(num2, isEntry, this),
					new GenreSelectSequence(num2, isEntry, this),
					new SortSettingSequence(num2, isEntry, this, OnSortChange),
					new DifficultySelectSequence(num2, isEntry, this),
					new MenuSelectSequence(num2, isEntry, this, OnGameStart),
					new OptionSelectSequence(num2, isEntry, this, Singleton<UserDataManager>.Instance.GetUserData(num2).Option),
					new CharacterSelectSequence(num2, isEntry, this)
				};
				for (int num3 = 0; num3 < _subSequenceArray[num2].Length; num3++)
				{
					_subSequenceArray[num2][num3].Initialize();
					_subSequenceArray[num2][num3].Next = Next;
					_subSequenceArray[num2][num3].SyncNext = SyncNext;
				}
				MonitorArray[num2].SetData(num2, this, container.assetManager, CurrentDifficulty[num2], isEntry);
				MonitorArray[num2].Initialize(num2, isEntry);
				bool flag2 = false;
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, num2, isEntry, flag2));
				if (isEntry)
				{
					UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(num2).Detail;
					UserOption option = Singleton<UserDataManager>.Instance.GetUserData(num2).Option;
					MessageUserInformationData messageUserInformationData = new MessageUserInformationData(num2, container.assetManager, detail, option.DispRate, isSubMonitor: true);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, num2, messageUserInformationData));
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30008, num2, option.SubmonitorAppeal));
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20030, num2, true));
					if (GameManager.IsEventMode || Singleton<UserDataManager>.Instance.GetUserData(num2).IsGuest())
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, num2, 1));
					}
					else
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, num2, detail.SelectMapID));
					}
				}
				else
				{
					IsPreparationCompletes[num2] = true;
				}
			}
			if (GameManager.IsCourseMode)
			{
				CurrentCategorySelect = 0;
				CurrentMusicSelect = 0;
				int nextDifficultyId = Singleton<CourseManager>.Instance.GetNextDifficultyId();
				for (int num4 = 0; num4 < MonitorArray.Length; num4++)
				{
					CurrentDifficulty[num4] = (MusicDifficultyID)nextDifficultyId;
					DifficultySelectIndex[num4] = nextDifficultyId;
				}
				for (int num5 = 0; num5 < MonitorArray.Length; num5++)
				{
					SoundManager.StopBGM(num5);
				}
				ChangeBGM();
				for (int num6 = 0; num6 < MonitorArray.Length; num6++)
				{
					_subSequenceArray[num6][(int)_currentPlayerSubSequence[num6]].OnStartSequence();
				}
				return;
			}
			CurrentSortRootID = SortRootID.Tab;
			CurrentCategorySelect = GameManager.CategoryIndex;
			CurrentMusicSelect = GameManager.MusicIndex;
			int num7 = _extraSlotFlag ^ GameManager.ExtraFlag;
			if (num7 > 0)
			{
				int flagTrueCount = GetFlagTrueCount(GameManager.ExtraFlag, _extraCategoryCount);
				int num8 = 0;
				int num9 = 0;
				if (CurrentCategorySelect < flagTrueCount)
				{
					for (int num10 = 0; num10 < _extraCategoryCount; num10++)
					{
						if ((GameManager.ExtraFlag & (1 << num10)) > 0)
						{
							if (num8 == CurrentCategorySelect)
							{
								num9 = num10;
								break;
							}
							num8++;
						}
					}
					int num11 = 0;
					for (int num12 = 0; num12 < _extraCategoryCount; num12++)
					{
						if ((num7 & (1 << num12)) > 0)
						{
							num11 = (((_extraSlotFlag & (1 << num12)) <= 0) ? (num11 - 1) : (num11 + 1));
						}
						if (num12 == num9)
						{
							break;
						}
					}
					if (CurrentCategorySelect + num11 < 0)
					{
						CurrentCategorySelect = 0;
					}
					else
					{
						CurrentCategorySelect += num11;
					}
				}
				else
				{
					int num13 = 0;
					for (int num14 = 0; num14 < _extraCategoryCount; num14++)
					{
						if ((num7 & (1 << num14)) > 0)
						{
							num13 = (((_extraSlotFlag & (1 << num14)) <= 0) ? (num13 - 1) : (num13 + 1));
						}
					}
					if (CurrentCategorySelect + num13 < 0)
					{
						CurrentCategorySelect = 0;
					}
					else
					{
						CurrentCategorySelect += num13;
					}
				}
			}
			if (CurrentCategorySelect < 0 || CurrentCategorySelect >= _combineMusicDataList.Count)
			{
				CurrentCategorySelect = 0;
			}
			if (CurrentMusicSelect >= _combineMusicDataList[CurrentCategorySelect].Count)
			{
				CurrentMusicSelect = 0;
			}
			for (int num15 = 0; num15 < MonitorArray.Length; num15++)
			{
				if (!Singleton<UserDataManager>.Instance.GetUserData(num15).IsEntry)
				{
					continue;
				}
				if (IsMaiList())
				{
					SearchMusicIndex(isForceChangeMusic, num15);
				}
				else if ((IsSortEnableCategory() && !IsGhostFolder(0)) || isForceChangeMusic)
				{
					if (!(!SetSortIndexToExtraGenre(GameManager.SelectMusicID[num15], CategoryNameList[CurrentCategorySelect]) && isForceChangeMusic))
					{
						break;
					}
					bool flag3 = false;
					foreach (string categoryName in CategoryNameList)
					{
						if (!IsMaiList(categoryName) && SetSortIndexToExtraGenre(GameManager.SelectMusicID[num15], categoryName))
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3 && isForceChangeMusic)
					{
						SearchMusicIndex(isForceChangeMusic, num15);
					}
				}
				else if (IsGhostFolder(0))
				{
					SetSortIndexToExtraGenre(GameManager.SelectMusicID[num15], CategoryNameList[CurrentCategorySelect]);
				}
				else if (!SetGenreSortIndex(CategoryNameList[CurrentCategorySelect]))
				{
					SearchMusicIndex(isForceChangeMusic, num15);
				}
				break;
			}
			GameManager.IsForceChangeMusic = false;
			if (GameManager.MusicTrackNumber == 1)
			{
				bool flag4 = false;
				for (int num16 = 0; num16 < MonitorArray.Length; num16++)
				{
					UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(num16);
					if (userData2.IsEntry && userData2.UserType == UserData.UserIDType.Exist)
					{
						flag4 = true;
						break;
					}
				}
				if (IsConnectionFolder() || !flag4)
				{
					CurrentCategorySelect = 0;
					CurrentMusicSelect = 0;
				}
			}
			for (int num17 = MonitorArray.Length - 1; num17 >= 0; num17--)
			{
				if (GameManager.EncountNewNpcGhost[num17])
				{
					SetGenreSortIndex(Singleton<DataManager>.Instance.GetMusicGenre(12).genreName);
					GameManager.EncountNewNpcGhost[num17] = false;
				}
			}
			if (GameManager.MusicTrackNumber != 1)
			{
				for (int num18 = 0; num18 < MonitorArray.Length; num18++)
				{
					SoundManager.StopBGM(num18);
				}
			}
			for (int num19 = 0; num19 < 2; num19++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(num19).IsEntry)
				{
					MonitorArray[num19].SetCharacterSelectMessage(isActive: false, "選択中のちほーは\n変更できません");
				}
			}
			for (int num20 = 0; num20 < MonitorArray.Length; num20++)
			{
				_subSequenceArray[num20][(int)_currentPlayerSubSequence[num20]].OnStartSequence();
			}
		}

		private void SearchMusicIndex(bool isForceChangeMusic, int i)
		{
			if (!SetSortIndexToMaiList(GameManager.SelectMusicID[i], GameManager.SelectDifficultyID[i]) && isForceChangeMusic && !SetSortIndexToExtraGenre(GameManager.SelectMusicID[i], Singleton<DataManager>.Instance.GetMusicGenre(196).genreName) && !SetSortIndexToExtraGenre(GameManager.SelectMusicID[i], Singleton<DataManager>.Instance.GetMusicGenre(195).genreName))
			{
				SetSortIndexToExtraGenre(GameManager.SelectMusicID[i], Singleton<DataManager>.Instance.GetMusicGenre(12).genreName);
			}
		}

		public void InputGenreSelectData(ReadOnlyCollection<CombineMusicSelectData> combineList, int categoryID, bool isExtra = false, int insert = -1)
		{
			for (int i = 0; i < 2; i++)
			{
				GenreSelectData genreSelectData = new GenreSelectData();
				foreach (CombineMusicSelectData combine in combineList)
				{
					genreSelectData.totalMusicNum++;
					genreSelectData.categoryID = categoryID;
					genreSelectData.isExtra = isExtra;
					genreSelectData.isTournament = IsTournamentFolderForCategoryID(categoryID);
					List<UserScore>[] scoreList = Singleton<UserDataManager>.Instance.GetUserData(i).ScoreList;
					int musicId = combine.GetID(ScoreType);
					int num = 0;
					num = ((!isExtra && _categorySortSetting == SortTabID.Level) ? combine.musicSelectData[(int)ScoreType].Difficulty : ((isExtra && (categoryID == Singleton<DataManager>.Instance.GetMusicGenre(12).GetID() || categoryID == Singleton<DataManager>.Instance.GetMusicGenre(14).GetID())) ? combine.musicSelectData[(int)ScoreType].Difficulty : ((CurrentDifficulty == null) ? GameManager.SelectDifficultyID[i] : ((int)CurrentDifficulty[i]))));
					if (num == 4 && !IsRemasterEnableForMusicID(musicId))
					{
						num = 3;
					}
					UserScore userScore = scoreList[num].Find((UserScore item) => item.id == musicId);
					if (userScore != null)
					{
						MusicClearrankID clearRank = GameManager.GetClearRank((int)userScore.achivement);
						genreSelectData.InputCombo(userScore.combo);
						genreSelectData.InputSync(userScore.sync);
						genreSelectData.InputRank(clearRank);
					}
				}
				if (insert != -1)
				{
					_genreSelectDataList[i].Insert(insert, genreSelectData);
				}
				else
				{
					_genreSelectDataList[i].Add(genreSelectData);
				}
			}
		}

		private void registerMusic()
		{
			List<CombineMusicSelectData> list = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData> list2 = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData> list3 = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData>[] array = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array2 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array3 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array4 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array5 = new List<CombineMusicSelectData>[2];
			Dictionary<int, List<CombineMusicSelectData>> dictionary = new Dictionary<int, List<CombineMusicSelectData>>();
			List<int> list4 = new List<int>();
			for (int i = 0; i < 2; i++)
			{
				array[i] = new List<CombineMusicSelectData>();
				array2[i] = new List<CombineMusicSelectData>();
				array3[i] = new List<CombineMusicSelectData>();
				array4[i] = new List<CombineMusicSelectData>();
				array5[i] = new List<CombineMusicSelectData>();
			}
			new SortedDictionary<int, List<CombineMusicSelectData>>();
			HashSet<int> hashSet = new HashSet<int>();
			bool flag = false;
			bool flag2 = false;
			for (int j = 0; j < 2; j++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData.IsEntry)
				{
					if (userData.IsGuest())
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
				}
			}
			foreach (KeyValuePair<int, MusicData> music in Singleton<DataManager>.Instance.GetMusics())
			{
				int id = music.Value.name.id;
				int inputMusicId = -1;
				if (!Singleton<DataManager>.Instance.GetMusics().ContainsKey(id))
				{
					continue;
				}
				_ = Singleton<DataManager>.Instance.GetMusic(id).genreName.id;
				if (!Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(id))
				{
					continue;
				}
				NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[id];
				int key = ((id < 10000) ? (id + 10000) : (id - 10000));
				NotesWrapper notesWrapper2 = null;
				if (Singleton<DataManager>.Instance.GetMusics().ContainsKey(key) && Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(key))
				{
					notesWrapper2 = Singleton<NotesListManager>.Instance.GetNotesList()[key];
				}
				NotesWrapper notesWrapper3 = ((id < 10000) ? notesWrapper : notesWrapper2);
				NotesWrapper notesWrapper4 = ((id >= 10000) ? notesWrapper : notesWrapper2);
				bool jumpOtherCategoryStandard = false;
				bool jumpOtherCategoryStandard2 = false;
				MusicDifficultyID musicDifficultyID = MusicDifficultyID.Invalid;
				if (notesWrapper3 != null)
				{
					jumpOtherCategoryStandard = notesWrapper3.IsPlayable;
					int num = 0;
					for (int k = 0; k < 2; k++)
					{
						num += notesWrapper3.BossGhost[k].Count;
						num += notesWrapper3.VsGhost[k].Count;
						num += notesWrapper3.MapGhost[k].Count;
						if (musicDifficultyID == MusicDifficultyID.Invalid)
						{
							if (notesWrapper3.BossGhost[k].Count != 0)
							{
								musicDifficultyID = notesWrapper3.BossGhost[k][0].Difficulty;
							}
							else if (notesWrapper3.VsGhost[k].Count != 0)
							{
								musicDifficultyID = notesWrapper3.VsGhost[k][0].Difficulty;
							}
							else if (notesWrapper3.MapGhost[k].Count != 0)
							{
								musicDifficultyID = notesWrapper3.MapGhost[k][0].Difficulty;
							}
						}
					}
					jumpOtherCategoryStandard2 = num != 0;
				}
				bool jumpOtherCategoryDeluxe = false;
				bool jumpOtherCategoryDeluxe2 = false;
				MusicDifficultyID musicDifficultyID2 = MusicDifficultyID.Invalid;
				if (notesWrapper4 != null)
				{
					jumpOtherCategoryDeluxe = notesWrapper4.IsPlayable;
					int num2 = 0;
					for (int l = 0; l < 2; l++)
					{
						num2 += notesWrapper4.BossGhost[l].Count;
						num2 += notesWrapper4.VsGhost[l].Count;
						num2 += notesWrapper4.MapGhost[l].Count;
						if (musicDifficultyID2 == MusicDifficultyID.Invalid)
						{
							if (notesWrapper4.BossGhost[l].Count != 0)
							{
								musicDifficultyID2 = notesWrapper4.BossGhost[l][0].Difficulty;
							}
							else if (notesWrapper4.VsGhost[l].Count != 0)
							{
								musicDifficultyID2 = notesWrapper4.VsGhost[l][0].Difficulty;
							}
							else if (notesWrapper4.MapGhost[l].Count != 0)
							{
								musicDifficultyID2 = notesWrapper4.MapGhost[l][0].Difficulty;
							}
						}
					}
					jumpOtherCategoryDeluxe2 = num2 != 0;
				}
				if (notesWrapper.IsPlayable)
				{
					if (notesWrapper.IsNewMusic && !GameManager.IsEventMode)
					{
						MusicSelectDetailData musicSelectDetailData = new MusicSelectDetailData();
						musicSelectDetailData.musicId = id;
						musicSelectDetailData.difficultyId = 0;
						list.Add(new CombineMusicSelectData(musicSelectDetailData, ref inputMusicId, isOtherTypeInput: false));
					}
					if (notesWrapper.Ranking > 0 && !GameManager.IsEventMode && flag)
					{
						MusicSelectDetailData musicSelectDetailData2 = new MusicSelectDetailData();
						musicSelectDetailData2.musicId = id;
						musicSelectDetailData2.difficultyId = 0;
						list2.Add(new CombineMusicSelectData(musicSelectDetailData2, ref inputMusicId, isOtherTypeInput: false));
					}
					if (!GameManager.IsEventMode && flag2)
					{
						foreach (int scoreRanking in notesWrapper.ScoreRankings)
						{
							bool flag3 = false;
							int num3 = 0;
							foreach (int item in list4)
							{
								if (scoreRanking == item)
								{
									flag3 = true;
									break;
								}
								num3++;
							}
							if (!flag3)
							{
								dictionary.Add(scoreRanking, new List<CombineMusicSelectData>());
								list4.Add(scoreRanking);
							}
							MusicSelectDetailData musicSelectDetailData3 = new MusicSelectDetailData();
							musicSelectDetailData3.musicId = id;
							musicSelectDetailData3.difficultyId = 0;
							dictionary[scoreRanking].Add(new CombineMusicSelectData(musicSelectDetailData3, ref inputMusicId, isOtherTypeInput: false));
						}
					}
					bool flag4 = false;
					for (int m = 0; m < 2; m++)
					{
						if (m < notesWrapper.IsMapBonus.Count && notesWrapper.IsMapBonus[m] && !hashSet.Contains(id) && !GameManager.IsEventMode && flag2)
						{
							MusicSelectDetailData musicSelectDetailData4 = new MusicSelectDetailData();
							musicSelectDetailData4.musicId = id;
							musicSelectDetailData4.difficultyId = 0;
							array2[m].Add(new CombineMusicSelectData(musicSelectDetailData4, ref inputMusicId));
						}
						for (int n = 0; n <= 4; n++)
						{
							if (notesWrapper.IsRating[m][n] && !GameManager.IsEventMode && GameManager.IsCardOpenRating[m])
							{
								MusicSelectDetailData musicSelectDetailData5 = new MusicSelectDetailData();
								musicSelectDetailData5.musicId = id;
								musicSelectDetailData5.difficultyId = n;
								array[m].Add(new CombineMusicSelectData(musicSelectDetailData5, m, ref inputMusicId, isOtherTypeInput: false));
							}
						}
						if (!GameManager.IsEventMode && flag2)
						{
							MusicSelectDetailData musicSelectDetailData6 = new MusicSelectDetailData();
							musicSelectDetailData6.musicId = id;
							musicSelectDetailData6.jumpOtherCategoryStandard = jumpOtherCategoryStandard;
							musicSelectDetailData6.jumpOtherCategoryDeluxe = jumpOtherCategoryDeluxe;
							foreach (GhostMatchData item2 in notesWrapper.BossGhost[m])
							{
								musicSelectDetailData6.difficultyId = (int)item2.Difficulty;
								array3[m].Add(new CombineMusicSelectData(musicSelectDetailData6, item2.Target, ref inputMusicId, isOtherTypeInput: false));
							}
							foreach (GhostMatchData item3 in notesWrapper.VsGhost[m])
							{
								musicSelectDetailData6.difficultyId = (int)item3.Difficulty;
								array3[m].Add(new CombineMusicSelectData(musicSelectDetailData6, item3.Target, ref inputMusicId, isOtherTypeInput: false));
							}
							foreach (GhostMatchData item4 in notesWrapper.MapGhost[m])
							{
								musicSelectDetailData6.difficultyId = (int)item4.Difficulty;
								array3[m].Add(new CombineMusicSelectData(musicSelectDetailData6, item4.Target, ref inputMusicId, isOtherTypeInput: false));
							}
						}
						if (!GameManager.IsEventMode && flag2)
						{
							if (notesWrapper.IsMapTaskMusic[m] && !flag4)
							{
								MusicSelectDetailData musicSelectDetailData7 = new MusicSelectDetailData();
								musicSelectDetailData7.musicId = id;
								musicSelectDetailData7.difficultyId = 0;
								array4[m].Add(new CombineMusicSelectData(musicSelectDetailData7, m, ref inputMusicId, isOtherTypeInput: false));
								flag4 = true;
							}
							if (notesWrapper.ChallengeDetail[m].isEnable)
							{
								MusicSelectDetailData musicSelectDetailData8 = new MusicSelectDetailData();
								musicSelectDetailData8.musicId = id;
								musicSelectDetailData8.difficultyId = 0;
								musicSelectDetailData8.startLife = notesWrapper.ChallengeDetail[m].startLife;
								musicSelectDetailData8.challengeUnlockDiff = (int)notesWrapper.ChallengeDetail[m].unlockDifficulty;
								musicSelectDetailData8.nextRelaxDay = notesWrapper.ChallengeDetail[m].nextRelaxDay;
								musicSelectDetailData8.infoEnable = notesWrapper.ChallengeDetail[m].infoEnable;
								musicSelectDetailData8.targetPlayer = m;
								array5[m].Add(new CombineMusicSelectData(musicSelectDetailData8, m, ref inputMusicId, isOtherTypeInput: false));
							}
						}
					}
					if (!hashSet.Contains(id))
					{
						MusicSelectDetailData musicSelectDetailData9 = new MusicSelectDetailData();
						musicSelectDetailData9.musicId = id;
						musicSelectDetailData9.difficultyId = 0;
						musicSelectDetailData9.jumpOtherCategoryStandard = jumpOtherCategoryStandard2;
						musicSelectDetailData9.jumpOtherCategoryDeluxe = jumpOtherCategoryDeluxe2;
						musicSelectDetailData9.jumpOtherCategoryLevelStd = musicDifficultyID;
						musicSelectDetailData9.jumpOtherCategoryLevelDx = musicDifficultyID2;
						musicSelectDetailData9.difficultyId = 0;
						list3.Add(new CombineMusicSelectData(musicSelectDetailData9, ref inputMusicId));
						if (inputMusicId != -1)
						{
							hashSet.Add(inputMusicId);
						}
					}
					continue;
				}
				bool flag5 = false;
				for (int num4 = 0; num4 < 2; num4++)
				{
					if (!GameManager.IsEventMode && flag2)
					{
						MusicSelectDetailData musicSelectDetailData10 = new MusicSelectDetailData();
						musicSelectDetailData10.musicId = id;
						musicSelectDetailData10.difficultyId = 0;
						musicSelectDetailData10.jumpOtherCategoryStandard = jumpOtherCategoryStandard;
						musicSelectDetailData10.jumpOtherCategoryDeluxe = jumpOtherCategoryDeluxe;
						foreach (GhostMatchData item5 in notesWrapper.BossGhost[num4])
						{
							musicSelectDetailData10.difficultyId = (int)item5.Difficulty;
							array3[num4].Add(new CombineMusicSelectData(musicSelectDetailData10, item5.Target, ref inputMusicId, isOtherTypeInput: false));
						}
						foreach (GhostMatchData item6 in notesWrapper.VsGhost[num4])
						{
							musicSelectDetailData10.difficultyId = (int)item6.Difficulty;
							array3[num4].Add(new CombineMusicSelectData(musicSelectDetailData10, item6.Target, ref inputMusicId, isOtherTypeInput: false));
						}
						foreach (GhostMatchData item7 in notesWrapper.MapGhost[num4])
						{
							musicSelectDetailData10.difficultyId = (int)item7.Difficulty;
							array3[num4].Add(new CombineMusicSelectData(musicSelectDetailData10, item7.Target, ref inputMusicId, isOtherTypeInput: false));
						}
					}
					if (!GameManager.IsEventMode && flag2)
					{
						if (notesWrapper.IsMapTaskMusic[num4] && !flag5)
						{
							MusicSelectDetailData musicSelectDetailData11 = new MusicSelectDetailData();
							musicSelectDetailData11.musicId = id;
							musicSelectDetailData11.difficultyId = 0;
							array4[num4].Add(new CombineMusicSelectData(musicSelectDetailData11, num4, ref inputMusicId, isOtherTypeInput: false));
							flag5 = true;
						}
						if (notesWrapper.ChallengeDetail[num4].isEnable)
						{
							MusicSelectDetailData musicSelectDetailData12 = new MusicSelectDetailData();
							musicSelectDetailData12.musicId = id;
							musicSelectDetailData12.difficultyId = 0;
							musicSelectDetailData12.startLife = notesWrapper.ChallengeDetail[num4].startLife;
							musicSelectDetailData12.challengeUnlockDiff = (int)notesWrapper.ChallengeDetail[num4].unlockDifficulty;
							musicSelectDetailData12.nextRelaxDay = notesWrapper.ChallengeDetail[num4].nextRelaxDay;
							musicSelectDetailData12.infoEnable = notesWrapper.ChallengeDetail[num4].infoEnable;
							musicSelectDetailData12.targetPlayer = num4;
							array5[num4].Add(new CombineMusicSelectData(musicSelectDetailData12, num4, ref inputMusicId, isOtherTypeInput: false));
						}
					}
				}
			}
			_connectCombineMusicDataList = new List<CombineMusicSelectData>();
			if (!GameManager.IsFreedomMode && !GameManager.IsCourseMode)
			{
				SetConnectData();
			}
			_newCombineMusicDataList = new ReadOnlyCollection<CombineMusicSelectData>(list);
			_combineRankingDataList = new ReadOnlyCollection<CombineMusicSelectData>(list2);
			_combineTournamentDataListList = new Dictionary<int, ReadOnlyCollection<CombineMusicSelectData>>();
			int num5 = 0;
			foreach (KeyValuePair<int, List<CombineMusicSelectData>> item8 in dictionary)
			{
				if (num5 < list4.Count)
				{
					_combineTournamentDataListList.Add(item8.Key, new ReadOnlyCollection<CombineMusicSelectData>(item8.Value));
				}
				num5++;
			}
			_combineRatingDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
			_combineBonusDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
			_combineGhostDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
			_combineMapTaskDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
			_combineChallengeDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
			for (int num6 = 0; num6 < 2; num6++)
			{
				_combineRatingDataList[num6] = new ReadOnlyCollection<CombineMusicSelectData>(array[num6]);
				_combineBonusDataList[num6] = new ReadOnlyCollection<CombineMusicSelectData>(array2[num6]);
				_combineGhostDataList[num6] = new ReadOnlyCollection<CombineMusicSelectData>(array3[num6]);
				_combineMapTaskDataList[num6] = new ReadOnlyCollection<CombineMusicSelectData>(array4[num6]);
				_combineChallengeDataList[num6] = new ReadOnlyCollection<CombineMusicSelectData>(array5[num6]);
			}
			_combineMaiListDataList = new ReadOnlyCollection<CombineMusicSelectData>(list3);
		}

		private bool registerCourseMusic()
		{
			List<CombineMusicSelectData> list = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData> list2 = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData> list3 = new List<CombineMusicSelectData>();
			List<CombineMusicSelectData>[] array = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array2 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array3 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array4 = new List<CombineMusicSelectData>[2];
			List<CombineMusicSelectData>[] array5 = new List<CombineMusicSelectData>[2];
			Dictionary<int, List<CombineMusicSelectData>> dictionary = new Dictionary<int, List<CombineMusicSelectData>>();
			List<int> list4 = new List<int>();
			for (int i = 0; i < 2; i++)
			{
				array[i] = new List<CombineMusicSelectData>();
				array2[i] = new List<CombineMusicSelectData>();
				array3[i] = new List<CombineMusicSelectData>();
				array4[i] = new List<CombineMusicSelectData>();
				array5[i] = new List<CombineMusicSelectData>();
			}
			int nextMusicId = Singleton<CourseManager>.Instance.GetNextMusicId();
			if (!Singleton<DataManager>.Instance.GetMusics().ContainsKey(nextMusicId))
			{
				return false;
			}
			Singleton<DataManager>.Instance.GetMusic(nextMusicId);
			if (Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(nextMusicId))
			{
				_ = Singleton<NotesListManager>.Instance.GetNotesList()[nextMusicId];
				MusicSelectDetailData musicSelectDetailData = new MusicSelectDetailData();
				musicSelectDetailData.musicId = nextMusicId;
				musicSelectDetailData.difficultyId = 0;
				musicSelectDetailData.jumpOtherCategoryStandard = false;
				musicSelectDetailData.jumpOtherCategoryDeluxe = false;
				musicSelectDetailData.difficultyId = 0;
				list3.Add(new CombineMusicSelectData(musicSelectDetailData, isOtherTypeInput: false));
				_connectCombineMusicDataList = new List<CombineMusicSelectData>();
				if (!GameManager.IsFreedomMode && GameManager.IsCourseMode)
				{
					SetConnectData();
				}
				_newCombineMusicDataList = new ReadOnlyCollection<CombineMusicSelectData>(list);
				_combineRankingDataList = new ReadOnlyCollection<CombineMusicSelectData>(list2);
				_combineTournamentDataListList = new Dictionary<int, ReadOnlyCollection<CombineMusicSelectData>>();
				int num = 0;
				foreach (KeyValuePair<int, List<CombineMusicSelectData>> item in dictionary)
				{
					if (num < list4.Count)
					{
						_combineTournamentDataListList.Add(item.Key, new ReadOnlyCollection<CombineMusicSelectData>(item.Value));
					}
					num++;
				}
				_combineRatingDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
				_combineBonusDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
				_combineGhostDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
				_combineMapTaskDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
				_combineChallengeDataList = new ReadOnlyCollection<CombineMusicSelectData>[2];
				for (int j = 0; j < 2; j++)
				{
					_combineRatingDataList[j] = new ReadOnlyCollection<CombineMusicSelectData>(array[j]);
					_combineBonusDataList[j] = new ReadOnlyCollection<CombineMusicSelectData>(array2[j]);
					_combineGhostDataList[j] = new ReadOnlyCollection<CombineMusicSelectData>(array3[j]);
					_combineMapTaskDataList[j] = new ReadOnlyCollection<CombineMusicSelectData>(array4[j]);
					_combineChallengeDataList[j] = new ReadOnlyCollection<CombineMusicSelectData>(array5[j]);
				}
				_combineMaiListDataList = new ReadOnlyCollection<CombineMusicSelectData>(list3);
				return true;
			}
			return false;
		}

		private static int GetFlagTrueCount(int data, int roopCount)
		{
			int num = 0;
			for (int i = 0; i < roopCount; i++)
			{
				if ((data & (1 << i)) > 0)
				{
					num++;
				}
			}
			return num;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_mainSequence)
			{
			case MainSequence.Init:
				_frameCount++;
				if (_frameCount <= 3)
				{
					break;
				}
				container.processManager.NotificationFadeIn();
				_frameCount = 0;
				if (GameManager.IsFreedomMode)
				{
					if (!GameManager.IsFreedomCountDown && GameManager.MusicTrackNumber == 1)
					{
						container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20004));
					}
				}
				else
				{
					container.processManager.PrepareTimer(99, 0, isEntry: false, TimerCountUp);
				}
				_timeCounter = 0f;
				_mainSequence = MainSequence.Fade;
				if (!GameManager.IsFreedomMode && !GameManager.IsCourseMode)
				{
					Manager.Party.Party.Party.Get().SelectMusic();
				}
				break;
			case MainSequence.Fade:
				_timeCounter += GameManager.GetGameMSecAdd();
				if (_timeCounter >= 100f)
				{
					_timeCounter = 0f;
					MusicSelectMonitor[] monitorArray = MonitorArray;
					for (int l = 0; l < monitorArray.Length; l++)
					{
						monitorArray[l].Play();
					}
					_mainSequence = MainSequence.FirstStaging;
				}
				break;
			case MainSequence.FirstStaging:
			{
				for (int i = 0; i < MonitorArray.Length; i++)
				{
					MonitorArray[i].ViewUpdate();
				}
				_timeCounter += GameManager.GetGameMSecAdd();
				if (!(_timeCounter >= 1000f))
				{
					break;
				}
				if (GameManager.IsFreedomMode)
				{
					if (!GameManager.IsFreedomCountDown && GameManager.MusicTrackNumber == 1)
					{
						GameManager.StartFreedomModeTimer(GameManager.GetFreedomStartTime());
					}
					else
					{
						GameManager.PauseFreedomModeTimer(isPause: false);
						if (!GameManager.IsFreedomTimeUp)
						{
							container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20002, false));
						}
					}
					container.processManager.PrepareTimer(65535, 0, isEntry: false, null);
				}
				_timeCounter = 0f;
				_mainSequence = MainSequence.Update;
				break;
			}
			case MainSequence.Update:
			{
				if (!GameManager.IsFreedomMode && !GameManager.IsCourseMode)
				{
					PartyExec();
				}
				CheckDifficultyBack();
				for (int j = 0; j < _subSequenceArray.Length && !_subSequenceArray[j][(int)_currentPlayerSubSequence[j]].Update(); j++)
				{
				}
				IsForceMusicBack = false;
				IsForceMusicBackPlayer = 0;
				for (int k = 0; k < MonitorArray.Length; k++)
				{
					MonitorArray[k].ViewUpdate();
				}
				if (_isShowCancelMessage)
				{
					_messageWaitTimer += GameManager.GetGameMSecAdd();
					if (_messageWaitTimer >= 3000f)
					{
						container.processManager.CloseWindow(0, WindowPositionID.Lower);
						container.processManager.CloseWindow(1, WindowPositionID.Lower);
						_messageWaitTimer = 0f;
						_isShowCancelMessage = false;
					}
				}
				if ((GameManager.IsFreedomMode || GameManager.IsCourseMode) && PrepareFinish)
				{
					ConnectMusicAllDecide = true;
				}
				if (GameManager.IsFreedomMode && GameManager.GetFreedomModeMSec() <= 0)
				{
					TimerCountUp();
					_timeCounter = 0f;
				}
				break;
			}
			case MainSequence.ReleaseWait:
				if (_timeCounter >= 1000f)
				{
					_timeCounter = 0f;
					GameStart();
					_mainSequence = MainSequence.Release;
				}
				_timeCounter += GameManager.GetGameMSecAdd();
				break;
			case MainSequence.Release:
				break;
			}
		}

		private void CheckDifficultyBack()
		{
			for (int i = 0; i < _subSequenceArray.Length; i++)
			{
				if (!IsEntry(i) || _currentPlayerSubSequence[i] != SubSequence.Difficulty || IsInputLocking(i))
				{
					continue;
				}
				if (!IsForceMusicBackConfirm[i])
				{
					if (!InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
					{
						continue;
					}
					if (Manager.Party.Party.Party.Get().IsHost())
					{
						IsForceMusicBackConfirm[i] = true;
						SoundManager.PlaySE(Cue.SE_INFO_ATTENTION, i);
						SetInputLockInfo(i, 100f);
						CallMessage(i, WindowMessageID.MusicSelectCancelConfirmHost);
						MonitorArray[i].SetConnectCancelWarning();
						continue;
					}
					if (!Manager.Party.Party.Party.Get().IsClient())
					{
						IsForceMusicBackPlayer = i;
						IsForceMusicBack = true;
						IsForceMusicBackConfirm[0] = (IsForceMusicBackConfirm[1] = false);
						break;
					}
					IsForceMusicBackConfirm[i] = true;
					SoundManager.PlaySE(Cue.SE_INFO_ATTENTION, i);
					SetInputLockInfo(i, 100f);
					CallMessage(i, WindowMessageID.MusicSelectCancelConfirmClient);
					MonitorArray[i].SetConnectCancelWarning();
				}
				else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button05))
				{
					IsForceMusicBackConfirm[0] = (IsForceMusicBackConfirm[1] = false);
					CloseWindow();
					MonitorArray[0].CloseConnectCancelWarning();
					MonitorArray[1].CloseConnectCancelWarning();
				}
				else if (InputManager.GetButtonDown(i, InputManager.ButtonSetting.Button04))
				{
					IsForceMusicBackPlayer = i;
					IsForceMusicBack = true;
					IsForceMusicBackConfirm[0] = (IsForceMusicBackConfirm[1] = false);
					CloseWindow();
					MonitorArray[0].CloseConnectCancelWarning();
					MonitorArray[1].CloseConnectCancelWarning();
				}
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			if (MonitorArray != null)
			{
				for (int i = 0; i < MonitorArray.Length; i++)
				{
					SequenceBase[][] subSequenceArray = _subSequenceArray;
					if (((subSequenceArray != null) ? subSequenceArray[i] : null) != null)
					{
						SequenceBase[] array = _subSequenceArray[i];
						for (int j = 0; j < array.Length; j++)
						{
							array[j].OnRelease();
						}
					}
					Object.Destroy(MonitorArray[i].gameObject);
				}
			}
			if (_characterSelectProces != null)
			{
				container.processManager.ReleaseProcess(_characterSelectProces);
			}
			Resources.UnloadUnusedAssets();
		}

		private void TimerCountUp()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < 2; i++)
			{
				if (IsEntry(i))
				{
					if (_currentPlayerSubSequence[i] == SubSequence.Genre)
					{
						flag = true;
					}
					else if (_currentPlayerSubSequence[i] == SubSequence.SortSetting)
					{
						flag2 = true;
					}
					else if (_currentPlayerSubSequence[i] == SubSequence.Music)
					{
						flag3 = true;
					}
				}
			}
			if (flag || flag2)
			{
				if (PlayingScoreID != 0)
				{
					int musicID = PlayingScoreID / 100;
					int difficultyID = PlayingScoreID % 100;
					if (IsMaiList())
					{
						SetSortIndexToMaiList(musicID, difficultyID);
					}
					else if (!SetSortIndexToExtraGenre(musicID, GetMusicCategoryNameFromGenreIndex(0)))
					{
						SetSortIndexToMaiList(musicID, difficultyID);
					}
				}
				else
				{
					CurrentMusicSelect = 0;
				}
			}
			if ((flag3 || flag2) && GetCombineMusic(0).isWaitConnectScore)
			{
				CurrentCategorySelect = 0;
				CurrentMusicSelect = 0;
			}
			if (flag3 || flag || flag2)
			{
				MusicSelectData musicSelectData = GetCombineMusic(0).musicSelectData[(int)ScoreType];
				for (int j = 0; j < 2; j++)
				{
					bool flag4 = IsLevelTab() || IsExtraFolder(0);
					DifficultySelectIndex[j] = (flag4 ? musicSelectData.Difficulty : ((int)CurrentDifficulty[j]));
				}
			}
			if ((flag3 || flag2) && IsConnectionFolder())
			{
				UserData[] array = new UserData[2]
				{
					new UserData(),
					new UserData()
				};
				for (int k = 0; k < 2; k++)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(k);
					array[k].IsEntry = userData.IsEntry;
					array[k].Detail.UserID = userData.Detail.UserID;
					array[k].Detail.UserName = userData.Detail.UserName;
					array[k].Detail.EquipIconID = userData.Detail.EquipIconID;
				}
				MusicDifficultyID[] fumenDifs = new MusicDifficultyID[2]
				{
					CurrentDifficulty[0],
					CurrentDifficulty[1]
				};
				Manager.Party.Party.Party.Get().SelectMusic();
				if (!Manager.Party.Party.Party.Get().StartJoin(RecruitData.IpAddress, new MechaInfo(array, fumenDifs, GetCombineMusic(0).GetID(ScoreType))))
				{
					CurrentCategorySelect = 0;
					CurrentMusicSelect = 0;
					OnGameStart();
				}
				else
				{
					_isTimeUpConnect = true;
					ClientStartWait();
				}
			}
			else if (Manager.Party.Party.Party.Get().IsClient())
			{
				if (!_isHostWait)
				{
					Manager.Party.Party.Party.Get().FinishSetting();
					ChangeUserInfo();
					PrepareFinish = false;
					_isHostWait = true;
					CallMessage(WindowMessageID.MusicSelectWaitToHost);
					ClientStartWait();
				}
			}
			else if (Manager.Party.Party.Party.Get().IsHost())
			{
				if (IsActiveJoined())
				{
					ChangeUserInfo();
					if (IsClientFinishSetting())
					{
						Manager.Party.Party.Party.Get().GoToTrackStart();
					}
					else
					{
						Manager.Party.Party.Party.Get().GoToTrackStart();
					}
				}
				else
				{
					Manager.Party.Party.Party.Get().Wait();
				}
				ConnectMusicAllDecide = true;
				OnGameStart();
			}
			else
			{
				OnGameStart();
			}
		}

		private void ClientStartWait()
		{
			for (int i = 0; i < 2; i++)
			{
				if (IsEntry(i))
				{
					IsPreparationCompletes[i] = true;
					CurrentSelectMenu[i] = MenuType.GameStart;
					_subSequenceArray[i][(int)_currentPlayerSubSequence[i]].Reset();
					_beforePlayerSubSequence[i] = _currentPlayerSubSequence[i];
					_currentPlayerSubSequence[i] = SubSequence.Menu;
					MonitorArray[i].OnClientTimeOut();
				}
			}
		}

		private void GameStart()
		{
			if (GameManager.IsCourseMode)
			{
				container.processManager.AddProcess(new TrackStartProcess(container), 50);
				container.processManager.ReleaseProcess(this);
			}
			else
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new TrackStartProcess(container)), 50);
			}
			SoundManager.PreviewEnd();
		}

		private void OnGameStart()
		{
			if (_mainSequence == MainSequence.ReleaseWait)
			{
				return;
			}
			_mainSequence = MainSequence.ReleaseWait;
			container.processManager.ClearTimeoutAction();
			NotificationCharacterSelectProcess();
			SoundManager.StopBGM(2);
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				_subSequenceArray[i][(int)_currentPlayerSubSequence[i]].OnGameStart();
				MonitorArray[i].OnGameStart();
				SoundManager.PreviewEnd();
				SoundManager.PlaySE(Cue.SE_TRACK_START_BUTTON, i);
			}
			CombineMusicSelectData combineMusic = GetCombineMusic(0);
			if (combineMusic.musicSelectData == null || combineMusic.GetID(ScoreType) == 2)
			{
				CurrentMusicSelect = 0;
				CurrentCategorySelect = 0;
				combineMusic = GetCombineMusic(0);
			}
			MusicSelectData musicSelectData = combineMusic.musicSelectData[(int)ScoreType];
			int num = 0;
			if (IsGhostFolder(0))
			{
				num = musicSelectData.Difficulty;
			}
			bool[] array = IsPlayableMusic(combineMusic.GetID(ScoreType));
			if (!IsConnectionFolder())
			{
				for (int j = 0; j < 2; j++)
				{
					if (DifficultySelectIndex[j] == -1)
					{
						DifficultySelectIndex[j] = 0;
					}
					if (array[DifficultySelectIndex[j]] || DifficultySelectIndex[j] <= num)
					{
						continue;
					}
					for (int num2 = array.Length - 1; num2 >= 0; num2--)
					{
						if (array[num2] || num2 <= 2 || num2 <= num)
						{
							DifficultySelectIndex[j] = num2;
							break;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < 2; k++)
				{
					if (DifficultySelectIndex[k] == -1)
					{
						DifficultySelectIndex[k] = 0;
					}
					if (DifficultySelectIndex[k] < 3 || array[DifficultySelectIndex[k]])
					{
						continue;
					}
					for (int num3 = array.Length - 1; num3 >= 0; num3--)
					{
						if (array[num3] || num3 <= 2)
						{
							DifficultySelectIndex[k] = num3;
							break;
						}
					}
				}
			}
			GameManager.SelectMusicID[0] = combineMusic.GetID(ScoreType);
			GameManager.SelectMusicID[1] = GameManager.SelectMusicID[0];
			_ = Singleton<NotesListManager>.Instance.GetNotesList()[combineMusic.GetID(ScoreType)];
			GameManager.SelectDifficultyID[0] = DifficultySelectIndex[0];
			GameManager.SelectDifficultyID[1] = DifficultySelectIndex[1];
			if (IsGhostFolder(0))
			{
				int difficulty = combineMusic.musicSelectData[(int)ScoreType].Difficulty;
				GhostManager.GhostTarget ghostTarget = combineMusic.musicSelectData[(int)ScoreType].GhostTarget;
				for (int l = 0; l < MonitorArray.Length; l++)
				{
					bool flag = Singleton<UserDataManager>.Instance.GetUserData(l).IsGuest();
					if (difficulty <= DifficultySelectIndex[l] && !flag)
					{
						GameManager.SelectGhostID[l] = ghostTarget;
					}
					else
					{
						GameManager.SelectGhostID[l] = GhostManager.GhostTarget.End;
					}
				}
				GameManager.SelectedDeleteGhostID = ghostTarget;
				if (GhostManager.GhostTarget.End != GameManager.SelectedDeleteGhostID)
				{
					UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectedDeleteGhostID);
					for (int m = 0; m < MonitorArray.Length; m++)
					{
						if (GhostManager.IsYourGhost(GameManager.SelectedDeleteGhostID, m))
						{
							if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.MapNpc)
							{
								Singleton<UserDataManager>.Instance.GetUserData(m).Extend.ClearEncountMapNpc((int)ghostToEnum.Id);
								break;
							}
							if (ghostToEnum != null && ghostToEnum.Type == UserGhost.GhostType.Player)
							{
								Singleton<GhostManager>.Instance.VsGhostUsed(GameManager.SelectedDeleteGhostID, m);
								break;
							}
						}
					}
				}
			}
			GameManager.IsPerfectChallenge = IsChallengeFolder();
			if (!GameManager.IsCourseMode)
			{
				GameManager.SortMusicSetting = _musicSortSetting;
				GameManager.SortCategorySetting = _categorySortSetting;
			}
			GameManager.CategoryIndex = CurrentCategorySelect;
			GameManager.MusicIndex = CurrentMusicSelect;
			GameManager.ExtraFlag = _extraSlotFlag;
			GameManager.SelectScoreType = (int)ScoreType;
			for (int n = 0; n < MonitorArray.Length; n++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(n);
				if (userData.IsEntry)
				{
					userData.Extend.SortMusicSetting = GameManager.SortMusicSetting;
					userData.Extend.SortCategorySetting = GameManager.SortCategorySetting;
					userData.Extend.CategoryIndex = GameManager.CategoryIndex;
					userData.Extend.MusicIndex = GameManager.MusicIndex;
					userData.Extend.ExtraFlag = GameManager.ExtraFlag;
					userData.Extend.SelectScoreType = GameManager.SelectScoreType;
					userData.Extend.SelectDifficultyID = GameManager.SelectDifficultyID[n];
					userData.Extend.SelectMusicID = GameManager.SelectMusicID[n];
				}
			}
			container.processManager.SetVisibleTimers(isVisible: false);
			container.processManager.IsTimeCounting(isTimeCount: false);
		}

		private void ExtraMusic(int player)
		{
			int num = 0;
			int num2 = 0;
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>();
			HashSet<int> hashSet = new HashSet<int>();
			foreach (CombineMusicSelectData newCombineMusicData in _newCombineMusicDataList)
			{
				if (!hashSet.Contains(newCombineMusicData.GetID(ScoreType)))
				{
					sortedList2.Add(newCombineMusicData);
					hashSet.Add(newCombineMusicData.GetID(ScoreType));
				}
			}
			if (sortedList2.Count > 0)
			{
				int id = Singleton<DataManager>.Instance.GetMusicGenre(1).name.id;
				SetSortList(player, id, ref sortedList2);
				sortedList[id] = new List<CombineMusicSelectData>(sortedList2);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			if (_combineRankingDataList.Count > 0)
			{
				List<CombineMusicSelectData> sortedList3 = new List<CombineMusicSelectData>();
				HashSet<int> hashSet2 = new HashSet<int>();
				foreach (CombineMusicSelectData combineRankingData in _combineRankingDataList)
				{
					if (!hashSet2.Contains(combineRankingData.GetID(ScoreType)))
					{
						sortedList3.Add(combineRankingData);
						hashSet2.Add(combineRankingData.GetID(ScoreType));
					}
				}
				if (sortedList3.Count > 0)
				{
					int id2 = Singleton<DataManager>.Instance.GetMusicGenre(2).name.id;
					SetSortList(player, id2, ref sortedList3);
					sortedList[id2] = new List<CombineMusicSelectData>(sortedList3);
					_extraSlotFlag |= 1 << num2++;
				}
			}
			else
			{
				num2++;
			}
			List<CombineMusicSelectData> sortedList4 = new List<CombineMusicSelectData>();
			HashSet<int> hashSet3 = new HashSet<int>();
			for (int i = 0; i < 2; i++)
			{
				foreach (CombineMusicSelectData item in _combineBonusDataList[i])
				{
					if (!hashSet3.Contains(item.GetID(ScoreType)))
					{
						sortedList4.Add(item);
						hashSet3.Add(item.GetID(ScoreType));
					}
				}
			}
			if (sortedList4.Count > 0)
			{
				int id3 = Singleton<DataManager>.Instance.GetMusicGenre(10).name.id;
				SetSortList(player, id3, ref sortedList4);
				sortedList[id3] = new List<CombineMusicSelectData>(sortedList4);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			List<CombineMusicSelectData> sortedList5 = new List<CombineMusicSelectData>();
			for (int j = 0; j < 2; j++)
			{
				foreach (CombineMusicSelectData item2 in _combineGhostDataList[j])
				{
					sortedList5.Add(item2);
				}
			}
			if (sortedList5.Count > 0)
			{
				int id4 = Singleton<DataManager>.Instance.GetMusicGenre(12).name.id;
				SetGhostSortList(player, ref sortedList5);
				sortedList[id4] = new List<CombineMusicSelectData>(sortedList5);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			List<CombineMusicSelectData> sortedList6 = new List<CombineMusicSelectData>();
			for (int k = 0; k < 2; k++)
			{
				foreach (CombineMusicSelectData item3 in _combineRatingDataList[k])
				{
					sortedList6.Add(item3);
				}
			}
			if (sortedList6.Count > 0)
			{
				int id5 = Singleton<DataManager>.Instance.GetMusicGenre(14).name.id;
				SetSortList(player, id5, ref sortedList6);
				sortedList[id5] = new List<CombineMusicSelectData>(sortedList6);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			List<CombineMusicSelectData> list = new List<CombineMusicSelectData>();
			for (int l = 0; l < 2; l++)
			{
				foreach (CombineMusicSelectData item4 in _combineMapTaskDataList[l])
				{
					list.Add(item4);
				}
			}
			if (list.Count > 0)
			{
				int id6 = Singleton<DataManager>.Instance.GetMusicGenre(195).name.id;
				sortedList[id6] = new List<CombineMusicSelectData>(list);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			List<CombineMusicSelectData> list2 = new List<CombineMusicSelectData>();
			for (int m = 0; m < 2; m++)
			{
				foreach (CombineMusicSelectData item5 in _combineChallengeDataList[m])
				{
					list2.Add(item5);
				}
			}
			if (list2.Count > 0)
			{
				int id7 = Singleton<DataManager>.Instance.GetMusicGenre(196).name.id;
				sortedList[id7] = new List<CombineMusicSelectData>(list2);
				_extraSlotFlag |= 1 << num2++;
			}
			else
			{
				num2++;
			}
			List<int> list3 = new List<int>();
			foreach (KeyValuePair<int, ReadOnlyCollection<CombineMusicSelectData>> combineTournamentDataList in _combineTournamentDataListList)
			{
				if (combineTournamentDataList.Value.Count > 0)
				{
					List<CombineMusicSelectData> sortedList7 = new List<CombineMusicSelectData>();
					HashSet<int> hashSet4 = new HashSet<int>();
					foreach (CombineMusicSelectData item6 in combineTournamentDataList.Value)
					{
						if (!hashSet4.Contains(item6.GetID(ScoreType)))
						{
							sortedList7.Add(item6);
							hashSet4.Add(item6.GetID(ScoreType));
						}
					}
					if (sortedList7.Count > 0)
					{
						int num3 = combineTournamentDataList.Key + 10000;
						SetSortList(player, num3, ref sortedList7);
						sortedList[num3] = new List<CombineMusicSelectData>(sortedList7);
						_extraSlotFlag |= 1 << num2++;
						list3.Add(num3);
					}
				}
				else
				{
					num2++;
				}
			}
			int[] array = new int[7]
			{
				Singleton<DataManager>.Instance.GetMusicGenre(1).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(2).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(10).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(12).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(14).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(195).name.id,
				Singleton<DataManager>.Instance.GetMusicGenre(196).name.id
			};
			foreach (int num4 in array)
			{
				if (sortedList.ContainsKey(num4) && sortedList[num4].Count > 0)
				{
					InputGenreSelectData(new ReadOnlyCollection<CombineMusicSelectData>(sortedList[num4]), num4, isExtra: true, num);
					_combineMusicDataList.Insert(num++, new ReadOnlyCollection<CombineMusicSelectData>(sortedList[num4]));
				}
			}
			foreach (int item7 in list3)
			{
				if (sortedList.ContainsKey(item7) && sortedList[item7].Count > 0)
				{
					InputGenreSelectData(new ReadOnlyCollection<CombineMusicSelectData>(sortedList[item7]), item7, isExtra: true, num);
					_combineMusicDataList.Insert(num++, new ReadOnlyCollection<CombineMusicSelectData>(sortedList[item7]));
				}
			}
			if (_connectCombineMusicDataList.Count > 0)
			{
				InputGenreSelectData(new ReadOnlyCollection<CombineMusicSelectData>(_connectCombineMusicDataList), Singleton<DataManager>.Instance.GetMusicGenre(198).name.id, isExtra: true);
				_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(_connectCombineMusicDataList));
				IsConnectCategoryEnable = true;
			}
			_currentExtraCategoryCount = num;
			_extraCategoryCount = num2;
			ExtraCategoryName();
		}

		private void ExtraCategoryName()
		{
			int num = 0;
			if (_newCombineMusicDataList.Count > 0)
			{
				CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(1).genreName);
				num++;
			}
			if (_combineRankingDataList.Count > 0)
			{
				CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(2).genreName);
				num++;
			}
			for (int i = 0; i < 2; i++)
			{
				if (_combineBonusDataList[i].Count > 0)
				{
					CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(10).genreName);
					num++;
					break;
				}
			}
			for (int j = 0; j < 2; j++)
			{
				if (_combineGhostDataList[j].Count > 0)
				{
					CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(12).genreName);
					num++;
					break;
				}
			}
			for (int k = 0; k < 2; k++)
			{
				if (_combineRatingDataList[k].Count > 0)
				{
					CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(14).genreName);
					num++;
					break;
				}
			}
			for (int l = 0; l < 2; l++)
			{
				if (_combineMapTaskDataList[l].Count > 0)
				{
					CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(195).genreName);
					num++;
					break;
				}
			}
			for (int m = 0; m < 2; m++)
			{
				if (_combineChallengeDataList[m].Count > 0)
				{
					CategoryNameList.Insert(num, Singleton<DataManager>.Instance.GetMusicGenre(196).genreName);
					num++;
					break;
				}
			}
			foreach (KeyValuePair<int, ReadOnlyCollection<CombineMusicSelectData>> combineTournamentDataList in _combineTournamentDataListList)
			{
				if (combineTournamentDataList.Value.Count > 0)
				{
					int key = combineTournamentDataList.Key;
					if (Singleton<ScoreRankingManager>.Instance.ScoreRankings.ContainsKey(key))
					{
						CategoryNameList.Insert(num, Singleton<ScoreRankingManager>.Instance.ScoreRankings[key].GenreName);
						num++;
					}
				}
			}
			if (_connectCombineMusicDataList.Count > 0)
			{
				CategoryNameList.Add(Singleton<DataManager>.Instance.GetMusicGenre(198).genreName);
			}
			_isExtraExpand = true;
		}

		public void ExecuteSort(int player)
		{
			int currentCategorySelect = CurrentCategorySelect;
			CategoryTabSort(player);
			ExtraMusic(player);
			CurrentCategorySelect = currentCategorySelect;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabSort(int player)
		{
			SortedList<int, List<CombineMusicSelectData>> result = null;
			SortedList<int, string> nameList = null;
			bool flag = IsMaiList();
			_combineMusicDataList.Clear();
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(_combineMaiListDataList));
			IsLevelCategory = false;
			IsVersionCategory = false;
			switch (_categorySortSetting)
			{
			case SortTabID.Genre:
				result = CategoryTabGenre(player, ref _combineMusicDataList, out nameList);
				break;
			case SortTabID.All:
				result = CategoryTabAll(player, ref _combineMusicDataList, out nameList);
				break;
			case SortTabID.Name:
				result = CagegoryTabMusicName(player, ref _combineMusicDataList, out nameList);
				break;
			case SortTabID.Level:
				IsLevelCategory = true;
				result = CategoryTabLevel(player, ref _combineMusicDataList, out nameList);
				break;
			case SortTabID.Version:
				IsVersionCategory = true;
				result = CategoryTabVersion(player, ref _combineMusicDataList, out nameList);
				break;
			}
			if (nameList != null)
			{
				CategoryNameList.Clear();
				foreach (int key in nameList.Keys)
				{
					CategoryNameList.Add(nameList[key]);
				}
				if ((_currentPlayerSubSequence == null || _currentPlayerSubSequence[player] != SubSequence.Genre) && flag)
				{
					CurrentCategorySelect = _currentExtraCategoryCount;
				}
			}
			return result;
		}

		private void OnSortChange(int player, bool deployList = true)
		{
			CombineMusicSelectData combineMusic = GetCombineMusic(0);
			int num = 0;
			if (_currentPlayerSubSequence != null && _currentPlayerSubSequence[player] == SubSequence.SortSetting && GetBeforeSubSeq() == SubSequence.Genre)
			{
				num = PlayingScoreID / 100;
			}
			IsScoreSort = false;
			bool flag = IsSortEnableCategory();
			bool flag2 = IsMaiList();
			string musicCategoryNameFromMusicIndex = GetMusicCategoryNameFromMusicIndex(0);
			SortedList<int, List<CombineMusicSelectData>> sortedList = null;
			sortedList = CategoryTabSort(player);
			_combineMusicDataList.Clear();
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				_genreSelectDataList[i].Clear();
			}
			if (sortedList != null)
			{
				foreach (int key in sortedList.Keys)
				{
					_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(sortedList[key]));
					InputGenreSelectData(new ReadOnlyCollection<CombineMusicSelectData>(sortedList[key]), key);
				}
			}
			ExtraMusic(player);
			int num2 = _currentExtraCategoryCount;
			if (sortedList != null)
			{
				foreach (int key2 in sortedList.Keys)
				{
					if (!IsLevelCategory)
					{
						continue;
					}
					for (int j = 0; j < sortedList[key2].Count; j++)
					{
						int iD = sortedList[key2][j].GetID(ScoreType);
						int difficulty = sortedList[key2][j].musicSelectData[(int)ScoreType].Difficulty;
						if (difficulty <= 4)
						{
							if (_levelCategoryPositionList.ContainsKey(iD))
							{
								_levelCategoryPositionList[iD][difficulty] = new LevelCategoryData
								{
									Category = num2,
									Index = j
								};
							}
							else
							{
								LevelCategoryData[] array = new LevelCategoryData[5];
								array[difficulty] = new LevelCategoryData
								{
									Category = num2,
									Index = j
								};
								_levelCategoryPositionList.Add(iD, array);
							}
						}
					}
					num2++;
				}
			}
			if (_currentPlayerSubSequence != null)
			{
				if (_categorySortSetting == SortTabID.Level && _currentPlayerSubSequence[player] == SubSequence.SortSetting && IsMaiList())
				{
					MusicDifficultyID musicDifficultyID = CurrentDifficulty[player];
					for (int k = 0; k < MonitorArray.Length; k++)
					{
						CurrentDifficulty[k] = musicDifficultyID;
						MonitorArray[k].ChangeDifficulty(musicDifficultyID, deployList);
					}
				}
			}
			else if (_categorySortSetting == SortTabID.Level && IsMaiList())
			{
				MusicDifficultyID musicDifficultyID2 = CurrentDifficulty[player];
				for (int l = 0; l < MonitorArray.Length; l++)
				{
					CurrentDifficulty[l] = musicDifficultyID2;
					MonitorArray[l].ChangeDifficulty(musicDifficultyID2, deployList);
				}
			}
			CategoryMedal();
			if (_currentPlayerSubSequence != null && _currentPlayerSubSequence[player] == SubSequence.Genre)
			{
				if (CurrentCategorySelect > CategoryNameList.Count - 1)
				{
					CurrentCategorySelect = CategoryNameList.Count - 1;
				}
			}
			else if (combineMusic != null && flag2)
			{
				SetSortIndexToMaiList(combineMusic, num);
			}
			else if (combineMusic != null && flag)
			{
				int musicID = ((num != 0) ? num : combineMusic.GetID(ScoreType));
				SetSortIndexToExtraGenre(musicID, musicCategoryNameFromMusicIndex);
			}
			else
			{
				SetGenreSortIndex(musicCategoryNameFromMusicIndex);
			}
			_beforeCategorySortSetting = _categorySortSetting;
		}

		private void SetSortList(int player, int key, ref List<CombineMusicSelectData> sortedList)
		{
			switch (_musicSortSetting)
			{
			case SortMusicID.Name:
				SetNameSortList(player, ref sortedList);
				return;
			case SortMusicID.Level:
				SetLevelSortList(player, ref sortedList, 1000 <= key && key < 10000);
				return;
			case SortMusicID.Rank:
				SetRankSortList(player, ref sortedList);
				return;
			case SortMusicID.ApFc:
				SetApFcSortList(player, ref sortedList);
				return;
			case SortMusicID.Sync:
				SetSyncSortList(player, ref sortedList);
				return;
			}
			if (key == Singleton<DataManager>.Instance.GetMusicGenre(2).name.id)
			{
				SetRankingSortList(player, ref sortedList);
				return;
			}
			if (key == Singleton<DataManager>.Instance.GetMusicGenre(14).name.id)
			{
				SetRatingSortList(player, ref sortedList);
				return;
			}
			switch (_categorySortSetting)
			{
			case SortTabID.Genre:
				SetDefaultSortList(player, ref sortedList);
				break;
			case SortTabID.All:
				SetDefaultSortList(player, ref sortedList);
				break;
			case SortTabID.Version:
				SetVersionSortList(player, ref sortedList);
				break;
			case SortTabID.Level:
				SetDefaultSortList(player, ref sortedList);
				break;
			case SortTabID.Name:
				SetNameSortList(player, ref sortedList);
				break;
			}
		}

		private void SetGhostSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				GhostManager.GhostTarget ghostTarget = a.musicSelectData[(int)ScoreType].GhostTarget;
				int ghostTarget2 = (int)b.musicSelectData[(int)ScoreType].GhostTarget;
				return (int)(ghostTarget - ghostTarget2);
			});
		}

		private void SetLevelSortList(int player, ref SortedList<int, List<CombineMusicSelectData>> sortedList, bool isMaiList)
		{
			foreach (int key in sortedList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = sortedList[key];
				SetLevelSortList(player, ref sortedList2, isMaiList);
				sortedList[key] = sortedList2;
			}
		}

		private void SetLevelSortList(int player, ref List<CombineMusicSelectData> sortedList, bool isMaiList)
		{
			IsScoreSort = true;
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				int num = ((isMaiList && IsLevelCategory) ? a.musicSelectData[(int)ScoreType].Difficulty : GetCurrentDifficulty(player));
				int num2 = ((isMaiList && IsLevelCategory) ? b.musicSelectData[(int)ScoreType].Difficulty : GetCurrentDifficulty(player));
				if (num == 4 && !IsRemasterEnableForMusicID(a.musicSelectData[(int)ScoreType].MusicData.name.id))
				{
					num = 3;
				}
				if (num2 == 4 && !IsRemasterEnableForMusicID(b.musicSelectData[(int)ScoreType].MusicData.name.id))
				{
					num2 = 3;
				}
				int num3 = ((a.musicSelectData[(int)ScoreType].ScoreData[num].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[num2].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[num].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[num2].musicLevelID) ? (-1) : 0));
				if (num3 == 0)
				{
					num3 = ((num > num2) ? 1 : ((num < num2) ? (-1) : 0));
					if (num3 == 0)
					{
						num3 = ((a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority < b.musicSelectData[(int)ScoreType].MusicData.priority) ? (-1) : 0));
					}
				}
				return num3;
			});
		}

		private void SetRankSortList(int player, ref SortedList<int, List<CombineMusicSelectData>> sortedList)
		{
			foreach (int key in sortedList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = sortedList[key];
				SetRankSortList(player, ref sortedList2);
				sortedList[key] = sortedList2;
			}
		}

		private void SetRankSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			int difficulty = GetCurrentDifficulty(player);
			IsScoreSort = true;
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				UserScore userScore;
				UserScore userScore2;
				if (IsLevelCategory)
				{
					userScore = GetUserScore(player, a.musicSelectData[(int)ScoreType].Difficulty, a.GetID(ScoreType));
					userScore2 = GetUserScore(player, b.musicSelectData[(int)ScoreType].Difficulty, b.GetID(ScoreType));
				}
				else
				{
					int difficulty2 = difficulty;
					int difficulty3 = difficulty;
					if (difficulty == 4)
					{
						if (!IsRemasterEnableForMusicID(a.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							difficulty2 = 3;
						}
						if (!IsRemasterEnableForMusicID(b.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							difficulty3 = 3;
						}
					}
					userScore = GetUserScore(player, difficulty2, a.GetID(ScoreType));
					userScore2 = GetUserScore(player, difficulty3, b.GetID(ScoreType));
				}
				int num;
				if (userScore == null || userScore2 == null)
				{
					if (userScore == null && userScore2 == null)
					{
						num = ((a.musicSelectData[(int)ScoreType].Difficulty > b.musicSelectData[(int)ScoreType].Difficulty) ? 1 : ((a.musicSelectData[(int)ScoreType].Difficulty < b.musicSelectData[(int)ScoreType].Difficulty) ? (-1) : 0));
						if (num == 0)
						{
							if (a.musicSelectData[(int)ScoreType].MusicData.priority <= b.musicSelectData[(int)ScoreType].MusicData.priority)
							{
								if (a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority)
								{
									return 0;
								}
								return -1;
							}
							return 1;
						}
						return num;
					}
					if (userScore != null)
					{
						return 1;
					}
					return -1;
				}
				MusicClearrankID clearRank = GameManager.GetClearRank((int)userScore.achivement);
				MusicClearrankID clearRank2 = GameManager.GetClearRank((int)userScore2.achivement);
				num = ((clearRank > clearRank2) ? 1 : ((clearRank < clearRank2) ? (-1) : 0));
				if (num == 0)
				{
					num = ((userScore.achivement < userScore2.achivement) ? (-1) : ((userScore.achivement > userScore2.achivement) ? 1 : 0));
					if (num == 0)
					{
						if (a.musicSelectData[(int)ScoreType].MusicData.priority <= b.musicSelectData[(int)ScoreType].MusicData.priority)
						{
							if (a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority)
							{
								return 0;
							}
							return -1;
						}
						return 1;
					}
				}
				return num;
			});
		}

		private void SetApFcSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			UserData data = Singleton<UserDataManager>.Instance.GetUserData(player);
			int difficulty = GetCurrentDifficulty(player);
			IsScoreSort = true;
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				UserScore userScore;
				UserScore userScore2;
				if (IsLevelCategory)
				{
					userScore = data.ScoreList[a.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore d) => d.id == a.GetID(ScoreType));
					userScore2 = data.ScoreList[b.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore f) => f.id == b.GetID(ScoreType));
				}
				else
				{
					int num = difficulty;
					int num2 = difficulty;
					if (difficulty == 4)
					{
						if (!IsRemasterEnableForMusicID(a.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							num = 3;
						}
						if (!IsRemasterEnableForMusicID(b.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							num2 = 3;
						}
					}
					userScore = data.ScoreList[num].Find((UserScore d) => d.id == a.GetID(ScoreType));
					userScore2 = data.ScoreList[num2].Find((UserScore f) => f.id == b.GetID(ScoreType));
				}
				int num3;
				if (userScore == null || userScore2 == null)
				{
					if (userScore == null && userScore2 == null)
					{
						num3 = ((!IsLevelCategory) ? ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? (-1) : 0)) : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? (-1) : 0)));
						if (num3 == 0)
						{
							if (a.musicSelectData[(int)ScoreType].MusicData.priority <= b.musicSelectData[(int)ScoreType].MusicData.priority)
							{
								if (a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority)
								{
									return 0;
								}
								return -1;
							}
							return 1;
						}
						return num3;
					}
					if (userScore != null)
					{
						return 1;
					}
					return -1;
				}
				int combo = (int)userScore.combo;
				int combo2 = (int)userScore2.combo;
				num3 = ((combo > combo2) ? 1 : ((combo < combo2) ? (-1) : 0));
				if (num3 == 0)
				{
					num3 = ((!IsLevelCategory) ? ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? (-1) : 0)) : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? (-1) : 0)));
					if (num3 == 0)
					{
						num3 = ((userScore.achivement < userScore2.achivement) ? (-1) : ((userScore.achivement > userScore2.achivement) ? 1 : 0));
						if (num3 == 0)
						{
							num3 = ((a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority < b.musicSelectData[(int)ScoreType].MusicData.priority) ? (-1) : 0));
						}
					}
				}
				return num3;
			});
		}

		private void SetSyncSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			UserData data = Singleton<UserDataManager>.Instance.GetUserData(player);
			int difficulty = GetCurrentDifficulty(player);
			IsScoreSort = true;
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				UserScore userScore;
				UserScore userScore2;
				if (IsLevelCategory)
				{
					userScore = data.ScoreList[a.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore d) => d.id == a.GetID(ScoreType));
					userScore2 = data.ScoreList[b.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore f) => f.id == b.GetID(ScoreType));
				}
				else
				{
					int num = difficulty;
					int num2 = difficulty;
					if (difficulty == 4)
					{
						if (!IsRemasterEnableForMusicID(a.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							num = 3;
						}
						if (!IsRemasterEnableForMusicID(b.musicSelectData[(int)ScoreType].MusicData.name.id))
						{
							num2 = 3;
						}
					}
					userScore = data.ScoreList[num].Find((UserScore d) => d.id == a.GetID(ScoreType));
					userScore2 = data.ScoreList[num2].Find((UserScore f) => f.id == b.GetID(ScoreType));
				}
				int num3;
				if (userScore == null || userScore2 == null)
				{
					if (userScore == null && userScore2 == null)
					{
						num3 = ((!IsLevelCategory) ? ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? (-1) : 0)) : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? (-1) : 0)));
						if (num3 == 0)
						{
							if (a.musicSelectData[(int)ScoreType].MusicData.priority <= b.musicSelectData[(int)ScoreType].MusicData.priority)
							{
								if (a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority)
								{
									return 0;
								}
								return -1;
							}
							return 1;
						}
						return num3;
					}
					if (userScore != null)
					{
						return 1;
					}
					return -1;
				}
				int sync = (int)userScore.sync;
				int sync2 = (int)userScore2.sync;
				num3 = ((sync > sync2) ? 1 : ((sync < sync2) ? (-1) : 0));
				if (num3 == 0)
				{
					num3 = ((!IsLevelCategory) ? ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID) ? (-1) : 0)) : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID > b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? 1 : ((a.musicSelectData[(int)ScoreType].ScoreData[a.musicSelectData[(int)ScoreType].Difficulty].musicLevelID < b.musicSelectData[(int)ScoreType].ScoreData[b.musicSelectData[(int)ScoreType].Difficulty].musicLevelID) ? (-1) : 0)));
					if (num3 == 0)
					{
						num3 = ((userScore.achivement < userScore2.achivement) ? (-1) : ((userScore.achivement > userScore2.achivement) ? 1 : 0));
						if (num3 == 0)
						{
							num3 = ((a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority < b.musicSelectData[(int)ScoreType].MusicData.priority) ? (-1) : 0));
						}
					}
				}
				return num3;
			});
		}

		private void SetNameSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			char hStart = SortMusicnameID.H_A_O.GetStartChar()[0];
			char hEnd = SortMusicnameID.H_WA_N.GetEndChar()[0];
			char aStart = SortMusicnameID.A_A_D.GetStartChar()[0];
			char aEnd = SortMusicnameID.A_T_Z.GetEndChar()[0];
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				string sortName = a.musicSelectData[(int)ScoreType].MusicData.sortName;
				string sortName2 = b.musicSelectData[(int)ScoreType].MusicData.sortName;
				int num = ((hStart <= sortName[0] && sortName[0] <= hEnd) ? 1 : ((aStart <= sortName[0] && sortName[0] <= aEnd) ? 2 : 3));
				int num2 = ((hStart <= sortName2[0] && sortName2[0] <= hEnd) ? 1 : ((aStart <= sortName2[0] && sortName2[0] <= aEnd) ? 2 : 3));
				int num3 = ((num < num2) ? (-1) : ((num > num2) ? 1 : 0));
				if (num3 == 0)
				{
					num3 = string.Compare(sortName, sortName2);
					if (num3 == 0)
					{
						num3 = ((a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority < b.musicSelectData[(int)ScoreType].MusicData.priority) ? (-1) : 0));
					}
				}
				return num3;
			});
		}

		private void SetVersionSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				int num = ((a.musicSelectData[(int)ScoreType].MusicData.version > b.musicSelectData[(int)ScoreType].MusicData.version) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.version < b.musicSelectData[(int)ScoreType].MusicData.version) ? (-1) : 0));
				if (num != 0)
				{
					return num;
				}
				return (a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority < b.musicSelectData[(int)ScoreType].MusicData.priority) ? (-1) : 0);
			});
		}

		private void SetDefaultSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort((CombineMusicSelectData a, CombineMusicSelectData b) => (a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority) ? ((a.musicSelectData[(int)ScoreType].Difficulty >= b.musicSelectData[(int)ScoreType].Difficulty) ? 1 : (-1)) : (-1)));
		}

		private void SetDefaultSortList_new(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort((CombineMusicSelectData a, CombineMusicSelectData b) => (a.musicSelectData[(int)ScoreType].MusicData.priority != b.musicSelectData[(int)ScoreType].MusicData.priority) ? ((a.musicSelectData[(int)ScoreType].MusicData.priority > b.musicSelectData[(int)ScoreType].MusicData.priority) ? 1 : ((a.musicSelectData[(int)ScoreType].MusicData.priority >= b.musicSelectData[(int)ScoreType].MusicData.priority) ? ((a.musicSelectData[(int)ScoreType].Difficulty >= b.musicSelectData[(int)ScoreType].Difficulty) ? ((a.musicSelectData[(int)ScoreType].Difficulty > b.musicSelectData[(int)ScoreType].Difficulty) ? 1 : 0) : (-1)) : (-1))) : 0);
		}

		private void SetRankingSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[a.GetID(ScoreType)];
				NotesWrapper notesWrapper2 = Singleton<NotesListManager>.Instance.GetNotesList()[b.GetID(ScoreType)];
				return (notesWrapper.Ranking >= notesWrapper2.Ranking) ? ((notesWrapper.Ranking > notesWrapper2.Ranking) ? 1 : 0) : (-1);
			});
		}

		private void SetRatingSortList(int player, ref List<CombineMusicSelectData> sortedList)
		{
			sortedList.Sort(delegate(CombineMusicSelectData a, CombineMusicSelectData b)
			{
				List<UserScore>[] scoreList = Singleton<UserDataManager>.Instance.GetUserData(player).ScoreList;
				UserScore userScore = scoreList[a.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore c) => c.id == a.GetID(ScoreType));
				UserScore userScore2 = scoreList[b.musicSelectData[(int)ScoreType].Difficulty].Find((UserScore d) => d.id == b.GetID(ScoreType));
				uint num = userScore?.achivement ?? 0;
				uint num2 = userScore2?.achivement ?? 0;
				uint num3 = userScore?.deluxscore ?? 0;
				uint num4 = userScore2?.deluxscore ?? 0;
				int difficulty = a.musicSelectData[(int)ScoreType].Difficulty;
				int difficulty2 = b.musicSelectData[(int)ScoreType].Difficulty;
				int musicLevelID = a.musicSelectData[(int)ScoreType].ScoreData[difficulty].musicLevelID;
				int musicLevelID2 = b.musicSelectData[(int)ScoreType].ScoreData[difficulty2].musicLevelID;
				if (num == 0 && num2 == 0)
				{
					if (musicLevelID == musicLevelID2)
					{
						return a.musicSelectData[(int)ScoreType].MusicData.priority - b.musicSelectData[(int)ScoreType].MusicData.priority;
					}
					return (musicLevelID > musicLevelID2) ? 1 : (-1);
				}
				return (num == num2) ? ((num3 == num4) ? ((musicLevelID == musicLevelID2) ? (a.musicSelectData[(int)ScoreType].MusicData.priority - b.musicSelectData[(int)ScoreType].MusicData.priority) : ((musicLevelID > musicLevelID2) ? 1 : (-1))) : ((num3 > num4) ? 1 : (-1))) : ((num >= num2) ? 1 : (-1));
			});
		}

		private void SetSortIndexToMaiList(CombineMusicSelectData data, int playingMusicID)
		{
			int num = 0;
			num = (data.isWaitConnectScore ? ((int)CurrentDifficulty[SortDecidePlayer]) : ((_beforeCategorySortSetting == SortTabID.Level) ? data.musicSelectData[(int)ScoreType].Difficulty : ((int)CurrentDifficulty[SortDecidePlayer])));
			int musicID = ((playingMusicID != 0) ? playingMusicID : data.GetID(ScoreType));
			SetSortIndexToMaiList(musicID, num);
		}

		private bool SetSortIndexToMaiList(int musicID, int difficultyID, bool isPosReset = true, bool isOtherTypeSearch = true)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			for (int i = 0; i < _combineMusicDataList.Count; i++)
			{
				ReadOnlyCollection<CombineMusicSelectData> readOnlyCollection = _combineMusicDataList[i];
				string categoryName = CategoryNameList[i];
				if (IsMaiList(categoryName))
				{
					num = 0;
					foreach (CombineMusicSelectData item in readOnlyCollection)
					{
						if (item.GetID(ScoreType) == musicID)
						{
							if (IsLevelCategory)
							{
								if (item.musicSelectData[(int)ScoreType].Difficulty == difficultyID)
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
						}
						if (flag)
						{
							break;
						}
						num++;
					}
				}
				if (flag)
				{
					break;
				}
				num2++;
			}
			if (!flag && isOtherTypeSearch)
			{
				int musicID2 = ((musicID < 10000) ? (musicID + 10000) : (musicID - 10000));
				if (SetSortIndexToMaiList(musicID2, difficultyID, isPosReset, isOtherTypeSearch: false))
				{
					return true;
				}
				if (SetSortIndexToMaiList(musicID, difficultyID - 1, isPosReset, isOtherTypeSearch: false))
				{
					return true;
				}
			}
			else if (!flag && IsLevelCategory && difficultyID == 4 && SetSortIndexToMaiList(musicID, difficultyID - 1, isPosReset, isOtherTypeSearch: false))
			{
				return true;
			}
			if (flag)
			{
				CurrentCategorySelect = num2;
				CurrentMusicSelect = num;
			}
			else if (isPosReset)
			{
				CurrentMusicSelect = 0;
			}
			return flag;
		}

		private bool SetSortIndexToExtraGenre(int musicID, string genreName)
		{
			int num = 0;
			CurrentMusicSelect = 0;
			SetGenreSortIndex(genreName);
			bool result = false;
			foreach (CombineMusicSelectData item in _combineMusicDataList[CurrentCategorySelect])
			{
				if (item.GetID(ScoreType) == musicID)
				{
					CurrentMusicSelect = num;
					return true;
				}
				num++;
			}
			return result;
		}

		private bool SetGenreSortIndex(string genreName)
		{
			bool result = false;
			for (int i = 0; i < CategoryNameList.Count; i++)
			{
				if (genreName == CategoryNameList[i])
				{
					CurrentCategorySelect = i;
					result = true;
					break;
				}
			}
			CurrentMusicSelect = 0;
			return result;
		}

		public void SetGhostJumpIndex()
		{
			if (IsMaiList())
			{
				int iD = _combineMusicDataList[CurrentCategorySelect][CurrentMusicSelect].GetID(ScoreType);
				SetSortIndexToExtraGenre(iD, Singleton<DataManager>.Instance.GetMusicGenre(12).genreName);
			}
			else
			{
				CombineMusicSelectData combineMusicSelectData = _combineMusicDataList[CurrentCategorySelect][CurrentMusicSelect];
				int iD2 = combineMusicSelectData.GetID(ScoreType);
				SetSortIndexToMaiList(combineMusicSelectData, iD2);
			}
			if (MonitorArray != null)
			{
				for (int i = 0; i < MonitorArray.Length; i++)
				{
					MonitorArray[i].SetDeployList(isAnimation: false);
				}
			}
		}

		private void CategoryMedal()
		{
			for (int i = 0; i < 2; i++)
			{
				_categoryMedalList[i].Clear();
			}
			foreach (ReadOnlyCollection<CombineMusicSelectData> combineMusicData in _combineMusicDataList)
			{
				for (int j = 0; j < 2; j++)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
					if (!userData.IsEntry)
					{
						continue;
					}
					MedalData[] array = new MedalData[6];
					for (MusicDifficultyID musicDifficultyID = MusicDifficultyID.Basic; musicDifficultyID < MusicDifficultyID.End; musicDifficultyID++)
					{
						bool flag = false;
						bool flag2 = false;
						MedalData medalData = new MedalData();
						foreach (CombineMusicSelectData item in combineMusicData)
						{
							int musicID = item.GetID(ScoreType);
							UserScore userScore = userData.ScoreList[(int)musicDifficultyID].Find((UserScore a) => a.id == musicID);
							if (userScore == null)
							{
								medalData.Failed();
								break;
							}
							MusicClearrankID clearRank = GameManager.GetClearRank((int)userScore.achivement);
							if (clearRank < MusicClearrankID.Rank_S)
							{
								flag = true;
								medalData.MinimumClearRank = MusicClearrankID.Rank_D;
							}
							else if (medalData.MinimumClearRank > clearRank)
							{
								medalData.MinimumClearRank = clearRank;
							}
							PlayComboflagID combo = userScore.combo;
							if (combo == PlayComboflagID.None)
							{
								flag2 = true;
								medalData.MinimumCombo = PlayComboflagID.None;
							}
							else if (medalData.MinimumCombo > combo)
							{
								medalData.MinimumCombo = combo;
							}
							if (flag && flag2)
							{
								medalData.Failed();
								break;
							}
						}
						array[(int)musicDifficultyID] = medalData;
					}
					_categoryMedalList[j].Add(new CategoryData
					{
						MedalDatas = array
					});
				}
			}
			for (int k = 0; k < _categoryMedalList.Length; k++)
			{
				for (int l = 0; l < _categoryMedalList[k].Count; l++)
				{
					for (int m = 0; m < _categoryMedalList[k][l].MedalDatas.Length; m++)
					{
					}
				}
			}
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabGenre(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> dataList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> inputMusicIdList = new HashSet<int>();
			for (int i = 0; i < musicList.Count; i++)
			{
				for (int j = 0; j < musicList[i].Count; j++)
				{
					if (musicList[i][j].musicSelectData[(int)ScoreType].Difficulty == 0)
					{
						SetSortMusicData(musicList[i][j].musicSelectData[(int)ScoreType].MusicData.genreName.id, musicList[i][j], Singleton<DataManager>.Instance.GetMusicGenre(musicList[i][j].musicSelectData[(int)ScoreType].MusicData.genreName.id).genreNameTwoLine, ref dataList, ref nameList, ref inputMusicIdList);
					}
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in dataList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>(dataList[key]);
				SetSortList(player, key + 1000, ref sortedList2);
				sortedList[key] = new List<CombineMusicSelectData>(sortedList2);
			}
			return sortedList;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabAll(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> dataList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> inputMusicIdList = new HashSet<int>();
			for (int i = 0; i < musicList.Count; i++)
			{
				for (int j = 0; j < musicList[i].Count; j++)
				{
					if (musicList[i][j].musicSelectData[(int)ScoreType].Difficulty == 0)
					{
						SetSortMusicData(0, musicList[i][j], Singleton<DataManager>.Instance.GetMusicGenre(199).genreNameTwoLine, ref dataList, ref nameList, ref inputMusicIdList);
					}
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in dataList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>(dataList[key]);
				SetSortList(player, key + 1000, ref sortedList2);
				sortedList[key] = new List<CombineMusicSelectData>(sortedList2);
			}
			return sortedList;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabVersion(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> dataList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> inputMusicIdList = new HashSet<int>();
			foreach (ReadOnlyCollection<CombineMusicSelectData> music in musicList)
			{
				foreach (CombineMusicSelectData item in music)
				{
					for (ConstParameter.ScoreKind scoreKind = ConstParameter.ScoreKind.Standard; scoreKind <= ConstParameter.ScoreKind.Deluxe; scoreKind++)
					{
						if (item.musicSelectData[(int)scoreKind].Difficulty == 0)
						{
							StringID addVersion = item.musicSelectData[(int)scoreKind].MusicData.AddVersion;
							SetVersionSortMusicData(scoreKind, item, addVersion.str, ref dataList, ref nameList, ref inputMusicIdList);
						}
					}
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in dataList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>(dataList[key]);
				SetSortList(player, key + 1000, ref sortedList2);
				sortedList[key] = new List<CombineMusicSelectData>(sortedList2);
			}
			return sortedList;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabLevel_old(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i <= 4; i++)
			{
				for (int j = 0; j < musicList.Count; j++)
				{
					for (int k = 0; k < musicList[j].Count; k++)
					{
						if (musicList[j][k].musicSelectData[(int)ScoreType].Difficulty != 0)
						{
							continue;
						}
						int musicLevelID = musicList[j][k].musicSelectData[(int)ScoreType].ScoreData[i].musicLevelID;
						int iD = musicList[j][k].GetID(ScoreType);
						if (!IsPlayEnableMusic(iD))
						{
							continue;
						}
						NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[iD];
						if (i >= 4 && !notesWrapper.IsEnable[4])
						{
							continue;
						}
						int item = iD * 100 + i;
						if (!hashSet.Contains(item))
						{
							MusicSelectDetailData msDetailData = _combineMusicDataList[j][k].msDetailData;
							msDetailData.difficultyId = i;
							int inputMusicId = -1;
							if (sortedList.ContainsKey(musicLevelID))
							{
								sortedList[musicLevelID].Add(new CombineMusicSelectData(msDetailData, ref inputMusicId));
							}
							else
							{
								nameList.Add(musicLevelID, "Lv." + ((MusicLevelID)musicLevelID).GetName());
								sortedList.Add(musicLevelID, new List<CombineMusicSelectData>
								{
									new CombineMusicSelectData(msDetailData, ref inputMusicId)
								});
							}
							if (inputMusicId != -1)
							{
								inputMusicId = inputMusicId * 100 + i;
								hashSet.Add(inputMusicId);
							}
						}
					}
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList2 = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in sortedList.Keys)
			{
				List<CombineMusicSelectData> sortedList3 = new List<CombineMusicSelectData>(sortedList[key]);
				SetSortList(player, key + 1000, ref sortedList3);
				sortedList2[key] = new List<CombineMusicSelectData>(sortedList3);
			}
			return sortedList2;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabLevel(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i <= 4; i++)
			{
				for (int j = 0; j < musicList.Count; j++)
				{
					for (int k = 0; k < musicList[j].Count; k++)
					{
						if (musicList[j][k].musicSelectData[(int)ScoreType].Difficulty != 0)
						{
							continue;
						}
						int musicLevelID = musicList[j][k].musicSelectData[(int)ScoreType].ScoreData[i].musicLevelID;
						int iD = musicList[j][k].GetID(ScoreType);
						NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[iD];
						if (i >= 4 && !notesWrapper.IsEnable[4])
						{
							continue;
						}
						int item = iD * 100 + i;
						if (!hashSet.Contains(item))
						{
							MusicSelectDetailData msDetailData = _combineMusicDataList[j][k].msDetailData;
							msDetailData.difficultyId = i;
							int inputMusicId = -1;
							if (sortedList.ContainsKey(musicLevelID))
							{
								sortedList[musicLevelID].Add(new CombineMusicSelectData(msDetailData, ref inputMusicId));
							}
							else
							{
								nameList.Add(musicLevelID, "Lv." + ((MusicLevelID)musicLevelID).GetName());
								sortedList.Add(musicLevelID, new List<CombineMusicSelectData>
								{
									new CombineMusicSelectData(msDetailData, ref inputMusicId)
								});
							}
							if (inputMusicId != -1)
							{
								inputMusicId = inputMusicId * 100 + i;
								hashSet.Add(inputMusicId);
							}
						}
					}
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList2 = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in sortedList.Keys)
			{
				List<CombineMusicSelectData> sortedList3 = new List<CombineMusicSelectData>(sortedList[key]);
				SetSortList(player, key + 1000, ref sortedList3);
				sortedList2[key] = new List<CombineMusicSelectData>(sortedList3);
			}
			return sortedList2;
		}

		private SortedList<int, List<CombineMusicSelectData>> CagegoryTabMusicName(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			List<CombineMusicSelectData> list = new List<CombineMusicSelectData>();
			for (int i = 0; i < musicList.Count; i++)
			{
				list.AddRange(musicList[i]);
			}
			list.Sort((CombineMusicSelectData a, CombineMusicSelectData b) => string.Compare(a.musicSelectData[(int)ScoreType].MusicData.sortName, b.musicSelectData[(int)ScoreType].MusicData.sortName));
			SortedList<int, List<CombineMusicSelectData>> dataList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> inputMusicIdList = new HashSet<int>();
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].musicSelectData[(int)ScoreType].Difficulty > 0 || list[j].musicSelectData[(int)ScoreType].MusicData.sortName.Length == 0)
				{
					continue;
				}
				char c = list[j].musicSelectData[(int)ScoreType].MusicData.sortName[0];
				bool flag = false;
				for (int k = 0; k < 15; k++)
				{
					if (((SortMusicnameID)k).GetStartChar()[0] <= c && c <= ((SortMusicnameID)k).GetEndChar()[0])
					{
						SetSortMusicData(k, list[j], ((SortMusicnameID)k).GetName(), ref dataList, ref nameList, ref inputMusicIdList);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					SetSortMusicData(15, list[j], SortMusicnameID.NumSymbole.GetName(), ref dataList, ref nameList, ref inputMusicIdList);
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in dataList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>(dataList[key]);
				SetSortList(player, key + 1000, ref sortedList2);
				sortedList[key] = new List<CombineMusicSelectData>(sortedList2);
			}
			return sortedList;
		}

		private SortedList<int, List<CombineMusicSelectData>> CategoryTabRank(int player, ref List<ReadOnlyCollection<CombineMusicSelectData>> musicList, out SortedList<int, string> nameList)
		{
			SortedList<int, List<CombineMusicSelectData>> dataList = new SortedList<int, List<CombineMusicSelectData>>();
			nameList = new SortedList<int, string>();
			HashSet<int> inputMusicIdList = new HashSet<int>();
			foreach (ReadOnlyCollection<CombineMusicSelectData> music in musicList)
			{
				foreach (CombineMusicSelectData item in music)
				{
					_ = Singleton<UserDataManager>.Instance.GetUserData(player).ScoreList;
					int clearRank = (int)GameManager.GetClearRank((int)(GetUserScore(player, (int)CurrentDifficulty[player], item.GetID(ScoreType))?.achivement ?? 0));
					SetSortMusicData(clearRank, item, clearRank.ToString(), ref dataList, ref nameList, ref inputMusicIdList);
				}
			}
			SortedList<int, List<CombineMusicSelectData>> sortedList = new SortedList<int, List<CombineMusicSelectData>>();
			foreach (int key in dataList.Keys)
			{
				List<CombineMusicSelectData> sortedList2 = new List<CombineMusicSelectData>(dataList[key]);
				SetSortList(player, key + 1000, ref sortedList2);
				sortedList[key] = new List<CombineMusicSelectData>(sortedList2);
			}
			return sortedList;
		}

		private void SetSortMusicData(int id, CombineMusicSelectData data, string tabName, ref SortedList<int, List<CombineMusicSelectData>> dataList, ref SortedList<int, string> nameList, ref HashSet<int> inputMusicIdList)
		{
			if (IsPlayEnableMusic(data.GetID(ScoreType)) && !inputMusicIdList.Contains(data.GetID(ScoreType)))
			{
				int inputMusicId = -1;
				if (dataList.ContainsKey(id))
				{
					dataList[id].Add(new CombineMusicSelectData(data.msDetailData, ref inputMusicId));
				}
				else
				{
					nameList.Add(id, tabName);
					dataList.Add(id, new List<CombineMusicSelectData>
					{
						new CombineMusicSelectData(data.msDetailData, ref inputMusicId)
					});
				}
				if (inputMusicId != -1)
				{
					inputMusicIdList.Add(inputMusicId);
				}
			}
		}

		private void SetVersionSortMusicData(ConstParameter.ScoreKind type, CombineMusicSelectData data, string tabName, ref SortedList<int, List<CombineMusicSelectData>> dataList, ref SortedList<int, string> nameList, ref HashSet<int> inputMusicIdList)
		{
			if (IsPlayEnableMusic(data.GetID(type)) && !inputMusicIdList.Contains(data.GetID(type)))
			{
				int id = data.musicSelectData[(int)type].MusicData.AddVersion.id;
				int inputMusicId = -1;
				data.msDetailData.musicId = data.GetID(type);
				if (dataList.ContainsKey(id))
				{
					dataList[id].Add(new CombineMusicSelectData(data.msDetailData, ref inputMusicId, isOtherTypeInput: false));
				}
				else
				{
					nameList.Add(id, tabName);
					dataList.Add(id, new List<CombineMusicSelectData>
					{
						new CombineMusicSelectData(data.msDetailData, ref inputMusicId, isOtherTypeInput: false)
					});
				}
				if (inputMusicId == -1)
				{
					inputMusicIdList.Add(data.GetID(type));
				}
			}
		}

		private bool IsPlayEnableMusic(int musicID)
		{
			if (Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(musicID) && Singleton<NotesListManager>.Instance.GetNotesList()[musicID].IsPlayable)
			{
				return true;
			}
			return false;
		}

		public MusicSelectData GetMusic(int diffIndex)
		{
			int num = CurrentMusicSelect + diffIndex;
			int num2 = CurrentCategorySelect;
			while (num >= _combineMusicDataList[num2].Count)
			{
				num -= _combineMusicDataList[num2].Count;
				num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
			}
			while (num < 0)
			{
				num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
				num = _combineMusicDataList[num2].Count + num;
			}
			return _combineMusicDataList[num2][num].musicSelectData[(int)ScoreType];
		}

		public CombineMusicSelectData GetCombineMusic(int diffIndex)
		{
			int num = CurrentMusicSelect + diffIndex;
			int num2 = CurrentCategorySelect;
			while (num >= _combineMusicDataList[num2].Count)
			{
				num -= _combineMusicDataList[num2].Count;
				num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
			}
			while (num < 0)
			{
				num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
				num = _combineMusicDataList[num2].Count + num;
			}
			return _combineMusicDataList[num2][num];
		}

		public GenreSelectData GetGenreSelectData(int player, int categoryIndex)
		{
			int num = CurrentCategorySelect + categoryIndex;
			if (num >= 0 && num < _genreSelectDataList[player].Count)
			{
				return _genreSelectDataList[player][num];
			}
			return null;
		}

		public GenreSelectData GetGenreSelectDataForMusicIndex(int player, int musicIndex)
		{
			int num = CurrentMusicSelect + musicIndex;
			int num2 = CurrentCategorySelect;
			while (num >= _combineMusicDataList[num2].Count)
			{
				num -= _combineMusicDataList[num2].Count;
				num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
			}
			while (num < 0)
			{
				num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
				num = _combineMusicDataList[num2].Count + num;
			}
			if (num2 >= 0 && num2 < _genreSelectDataList[player].Count)
			{
				return _genreSelectDataList[player][num2];
			}
			return null;
		}

		public void ReCalcGenreSelectData()
		{
			OnSortChange(SortDecidePlayer, deployList: false);
		}

		public void ReCalcLevelSortData()
		{
			OnSortChange(SortDecidePlayer);
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				if (IsLevelTab())
				{
					CombineMusicSelectData combineMusic = GetCombineMusic(0);
					if (combineMusic != null)
					{
						MonitorArray[i].ChangeDifficulty((MusicDifficultyID)combineMusic.musicSelectData[(int)ScoreType].Difficulty);
					}
				}
				else
				{
					MonitorArray[i].ChangeDifficulty(CurrentDifficulty[i]);
				}
			}
		}

		public bool IsMusicBoundary(int diffIndex, out int overCount)
		{
			int num = CurrentMusicSelect + diffIndex;
			int num2 = CurrentCategorySelect;
			overCount = 0;
			if (num >= CombineMusicDataList[num2].Count)
			{
				while (num >= _combineMusicDataList[num2].Count)
				{
					num = num - _combineMusicDataList[num2].Count - 1;
					num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
					overCount++;
				}
			}
			else if (num < -1)
			{
				while (num < 0)
				{
					overCount--;
					num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
					num = _combineMusicDataList[num2].Count + num + 1;
				}
			}
			if (num != _combineMusicDataList[num2].Count)
			{
				return num == -1;
			}
			return true;
		}

		public bool IsGenreBoundary(int localIndex, out int overCount)
		{
			int num = CurrentCategorySelect + localIndex;
			overCount = 0;
			if (num >= _combineMusicDataList.Count)
			{
				while (num >= _combineMusicDataList.Count)
				{
					num = ((num + 1 < _combineMusicDataList.Count) ? (num + 1) : 0);
					num = num - _combineMusicDataList.Count - 1;
					overCount++;
				}
			}
			else if (num < -1)
			{
				while (num < 0)
				{
					overCount--;
					num = _combineMusicDataList.Count + num + 1;
				}
			}
			if (num != _combineMusicDataList.Count)
			{
				return num == -1;
			}
			return true;
		}

		public int GetDifficulty(int playerIndex, int musicIndex = 0)
		{
			if (IsLevelTab(musicIndex))
			{
				return GetDifficultyByLevel(musicIndex);
			}
			return GetCurrentDifficulty(playerIndex);
		}

		public int GetDifficultyByLevel(int diffIndex)
		{
			int num = CurrentMusicSelect + diffIndex;
			int num2 = CurrentCategorySelect;
			while (num >= _combineMusicDataList[num2].Count)
			{
				num -= _combineMusicDataList[num2].Count;
				num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
			}
			while (num < 0)
			{
				num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
				num = _combineMusicDataList[num2].Count + num;
			}
			if (_combineMusicDataList[num2][num].musicSelectData[(int)ScoreType] == null)
			{
				return 0;
			}
			return _combineMusicDataList[num2][num].musicSelectData[(int)ScoreType].Difficulty;
		}

		public LevelCategoryData GetLevelToListPositoin(int musicId, int difficulty)
		{
			return _levelCategoryPositionList[musicId][difficulty];
		}

		public int GetCurrentDifficulty(int monitorIndex)
		{
			return (int)CurrentDifficulty[monitorIndex];
		}

		public bool IsExtraFolder(int diffIndex)
		{
			string musicCategoryNameFromMusicIndex = GetMusicCategoryNameFromMusicIndex(diffIndex);
			return IsExtraFolder(musicCategoryNameFromMusicIndex);
		}

		public bool IsExtraFolder(string categoryName)
		{
			if (!categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(12).genreName))
			{
				return categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(14).genreName);
			}
			return true;
		}

		public bool IsMaiList(int musicIndex = 0)
		{
			string musicCategoryNameFromMusicIndex = GetMusicCategoryNameFromMusicIndex(musicIndex);
			return IsMaiList(musicCategoryNameFromMusicIndex);
		}

		public bool IsMaiList(string categoryName)
		{
			return !((categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(1).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(2).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(195).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(196).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(197).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(10).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(12).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(14).genreName) || categoryName.Contains(Singleton<DataManager>.Instance.GetMusicGenre(198).genreName)) | IsTournamentFolder(categoryName));
		}

		public bool IsMaiListFromCategoryIndex(int categoryIndex = 0)
		{
			int num = CurrentCategorySelect + categoryIndex;
			if (num < 0 || num >= CategoryNameList.Count)
			{
				return false;
			}
			string categoryName = CategoryNameList[num];
			return IsMaiList(categoryName);
		}

		public bool IsSortEnableCategory(int musicIndex = 0)
		{
			return !GetMusicCategoryNameFromMusicIndex(musicIndex).Contains(Singleton<DataManager>.Instance.GetMusicGenre(198).genreName);
		}

		public bool IsLevelTab(int musicIndex = 0)
		{
			if (IsLevelCategory)
			{
				return IsMaiList(musicIndex);
			}
			return false;
		}

		public int GetExtraPlayer(int diffIndex)
		{
			GhostManager.GhostTarget ghostTarget = GetCombineMusic(diffIndex).musicSelectData[(int)ScoreType].GhostTarget;
			int num = 0;
			if ((GhostManager.GhostTarget.MapGhost1_1P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.VsGhost6_1P) || ghostTarget <= GhostManager.GhostTarget.BossGhost_1P)
			{
				return 0;
			}
			if ((GhostManager.GhostTarget.MapGhost1_2P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.VsGhost6_2P) || ghostTarget <= GhostManager.GhostTarget.BossGhost_2P)
			{
				return 1;
			}
			return -1;
		}

		public GhostManager.GhostType GetGhostType(int musicIndex = 0)
		{
			GhostManager.GhostTarget ghostTarget = GetCombineMusic(musicIndex).musicSelectData[(int)ScoreType].GhostTarget;
			GhostManager.GhostType ghostType = GhostManager.GhostType.Invalid;
			if ((GhostManager.GhostTarget.MapGhost1_1P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.MapGhost3_1P) || (GhostManager.GhostTarget.MapGhost1_2P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.MapGhost3_2P))
			{
				return GhostManager.GhostType.Map;
			}
			if ((GhostManager.GhostTarget.VsGhost1_1P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.VsGhost6_1P) || (GhostManager.GhostTarget.VsGhost1_2P <= ghostTarget && ghostTarget <= GhostManager.GhostTarget.VsGhost6_2P))
			{
				return GhostManager.GhostType.Global;
			}
			if (ghostTarget == GhostManager.GhostTarget.BossGhost_1P || ghostTarget == GhostManager.GhostTarget.BossGhost_2P)
			{
				return GhostManager.GhostType.Boss;
			}
			return GhostManager.GhostType.Invalid;
		}

		public bool IsGhostFolder(int diffIndex)
		{
			return GetMusicCategoryNameFromMusicIndex(diffIndex).Contains(Singleton<DataManager>.Instance.GetMusicGenre(12).genreName);
		}

		public bool IsConnectionFolder(int index = 0)
		{
			return GetMusicCategoryNameFromMusicIndex(index).Contains(Singleton<DataManager>.Instance.GetMusicGenre(198).genreName);
		}

		public bool IsRatingFolder(int index = 0)
		{
			return GetMusicCategoryNameFromMusicIndex(index).Contains(Singleton<DataManager>.Instance.GetMusicGenre(14).genreName);
		}

		public bool IsMapTaskFolder(int index = 0)
		{
			return GetMusicCategoryNameFromMusicIndex(index).Contains(Singleton<DataManager>.Instance.GetMusicGenre(195).genreName);
		}

		public bool IsChallengeFolder(int index = 0)
		{
			return GetMusicCategoryNameFromMusicIndex(index).Contains(Singleton<DataManager>.Instance.GetMusicGenre(196).genreName);
		}

		public bool IsTournamentFolder(int index = 0)
		{
			string musicCategoryNameFromMusicIndex = GetMusicCategoryNameFromMusicIndex(index);
			return IsTournamentFolder(musicCategoryNameFromMusicIndex);
		}

		private bool IsTournamentFolder(string categoryName)
		{
			foreach (KeyValuePair<int, ReadOnlyCollection<CombineMusicSelectData>> combineTournamentDataList in _combineTournamentDataListList)
			{
				int key = combineTournamentDataList.Key;
				if (Singleton<ScoreRankingManager>.Instance.ScoreRankings.ContainsKey(key))
				{
					string genreName = Singleton<ScoreRankingManager>.Instance.ScoreRankings[key].GenreName;
					if (categoryName.Contains(genreName))
					{
						return true;
					}
				}
			}
			return false;
		}

		public int GetTournamentRankingID(int index = 0)
		{
			string musicCategoryNameFromMusicIndex = GetMusicCategoryNameFromMusicIndex(index);
			foreach (KeyValuePair<int, ReadOnlyCollection<CombineMusicSelectData>> combineTournamentDataList in _combineTournamentDataListList)
			{
				int key = combineTournamentDataList.Key;
				if (Singleton<ScoreRankingManager>.Instance.ScoreRankings.ContainsKey(key))
				{
					string genreName = Singleton<ScoreRankingManager>.Instance.ScoreRankings[key].GenreName;
					if (musicCategoryNameFromMusicIndex.Contains(genreName))
					{
						return key;
					}
				}
			}
			return -1;
		}

		private bool IsTournamentFolderForCategoryID(int categoryID)
		{
			int key = categoryID - 10000;
			return Singleton<ScoreRankingManager>.Instance.ScoreRankings.ContainsKey(key);
		}

		public bool IsOptionBoundary(int playerIndex, int diffIndex, out int overCount)
		{
			return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).IsOptionBoundary(diffIndex, out overCount);
		}

		public string GetOptionName(int playerIndex, int diffIndex, out OptionCategoryID category, out string value, out string detail, out string valueDetails, out string spriteKey, out bool isLeftButtonActive, out bool isRightButtonActive)
		{
			return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).GetCategory(diffIndex, out category, out value, out detail, out valueDetails, out spriteKey, out isLeftButtonActive, out isRightButtonActive);
		}

		public string GetOptionCategoryName(int playerIndex, int diffIndex)
		{
			return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).GetCategoryName(diffIndex);
		}

		public SortData GetSortData(SortRootID root)
		{
			SortData result = default(SortData);
			result.TargetName = root.GetName();
			result.SortTypeName = GetSortName(root);
			result.DetailName = GetSortDatails(root);
			return result;
		}

		public string GetCategoryName(int playerIndex, int diff)
		{
			string text = "";
			SubSequence subSequence = _currentPlayerSubSequence[playerIndex];
			if (subSequence == SubSequence.Option)
			{
				return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).GetCategoryName(diff);
			}
			return GetMusicCategoryNameFromGenreIndex(diff);
		}

		public string GetMusicCategoryNameFromMusicIndex(int musicIndex)
		{
			int num = ((musicIndex != 0) ? (CurrentMusicSelect + musicIndex) : 0);
			int num2 = CurrentCategorySelect;
			if (_combineMusicDataList.Count == 0)
			{
				return "";
			}
			while (num >= _combineMusicDataList[num2].Count)
			{
				num -= _combineMusicDataList[num2].Count;
				num2 = ((num2 + 1 < _combineMusicDataList.Count) ? (num2 + 1) : 0);
			}
			while (num < 0)
			{
				num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_combineMusicDataList.Count - 1));
				num = _combineMusicDataList[num2].Count + num;
			}
			return CategoryNameList[num2];
		}

		public string GetMusicCategoryNameFromGenreIndex(int genreIndex)
		{
			int num = CurrentCategorySelect + genreIndex;
			if (_combineMusicDataList.Count <= 0)
			{
				return CategoryNameList[num];
			}
			while (num >= _combineMusicDataList.Count)
			{
				num -= _combineMusicDataList.Count;
			}
			while (num < 0)
			{
				num = _combineMusicDataList.Count + num;
			}
			return CategoryNameList[num];
		}

		public bool ScoreKindMove()
		{
			if (IsMaiList() && IsVersionCategory)
			{
				int iD = GetCombineMusic(0).GetID(ScoreType);
				int num = 0;
				num = ((iD >= 10000) ? (iD - 10000) : (iD + 10000));
				return SetSortIndexToMaiList(num, 0, isPosReset: false, isOtherTypeSearch: false);
			}
			return false;
		}

		public MedalData GetCategoryMedalData(int playerIndex, int diff)
		{
			int num;
			for (num = CurrentCategorySelect + diff; num >= CategoryNameList.Count; num -= CategoryNameList.Count)
			{
			}
			while (num < 0)
			{
				num = CategoryNameList.Count + num;
			}
			int currentDifficulty = GetCurrentDifficulty(playerIndex);
			return _categoryMedalList[playerIndex][num].MedalDatas[currentDifficulty];
		}

		public int GetCategoryGenruColor(int playerIndex, int diff)
		{
			int result = -1;
			if (_currentPlayerSubSequence[playerIndex] == SubSequence.Music && _categorySortSetting == SortTabID.Genre)
			{
				int num = CurrentCategorySelect + diff;
				if (CategoryNameList.Count > 0)
				{
					while (num >= CategoryNameList.Count)
					{
						num -= CategoryNameList.Count;
					}
					while (num < 0)
					{
						num = CategoryNameList.Count + num;
					}
				}
				result = _combineMusicDataList[num][0].musicSelectData[(int)ScoreType].MusicData.genreName.id;
			}
			return result;
		}

		public int GetCurrentMusicListMax()
		{
			return _combineMusicDataList[CurrentCategorySelect].Count;
		}

		public int GetCurrentListIndex(int playerIndex)
		{
			SubSequence subSequence = _currentPlayerSubSequence[playerIndex];
			if (subSequence == SubSequence.Option)
			{
				return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).CurrentOption;
			}
			return CurrentMusicSelect;
		}

		public int GetCurrentCategoryMax(int playerIndex)
		{
			SubSequence subSequence = _currentPlayerSubSequence[playerIndex];
			if (subSequence == SubSequence.Option)
			{
				return ((OptionSelectSequence)_subSequenceArray[playerIndex][5]).CurrentOptionCategory.GetCategoryMax();
			}
			return GetCurrentMusicListMax();
		}

		public Sprite GetTabSprite(GenreSelectData data)
		{
			return MonitorArray[0].GetTabSprite(data);
		}

		public Sprite[] GetCompIconSprite(MusicClearrankID rank, MusicSelectMonitor.FcapIconEnum fcap)
		{
			return MonitorArray[0].GetCompIconSprite(rank, fcap);
		}

		public int GetCurrentMenu(int playerIndex)
		{
			return (int)CurrentSelectMenu[playerIndex];
		}

		public float GetVolumeAmount(int playerIndex)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Option.HeadPhoneVolume.GetValue();
		}

		public UserData GetUserData(int playerIndex)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
		}

		public UserScore GetUserScore(int playerIndex, int difficulty, int musicId)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex).ScoreList[difficulty].Find((UserScore a) => a.id == musicId);
		}

		public MusicData GetMusicNotes(int musicID)
		{
			return Singleton<DataManager>.Instance.GetMusic(musicID);
		}

		public void CallWaitMessage(int playerIndex)
		{
			container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait);
		}

		public void CallCancelMessage(int playerIndex)
		{
			if (playerIndex != -1)
			{
				playerIndex = ((playerIndex == 0) ? 1 : 0);
				if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).IsEntry)
				{
					container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationCancel);
					IsPreparationCompletes[0] = (IsPreparationCompletes[1] = false);
					_isShowCancelMessage = true;
				}
				return;
			}
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).IsEntry)
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationCancel);
					IsPreparationCompletes[0] = (IsPreparationCompletes[1] = false);
					_isShowCancelMessage = true;
				}
			}
		}

		public void CallMessage(int playerIndex, WindowMessageID id)
		{
			container.processManager.EnqueueMessage(playerIndex, id);
		}

		public void CallMessage(WindowMessageID id)
		{
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				if (IsEntry(i))
				{
					container.processManager.EnqueueMessage(i, id);
				}
			}
		}

		public void CloseWindow(int playerIndex = -1)
		{
			if (playerIndex == -1)
			{
				for (int i = 0; i < MonitorArray.Length; i++)
				{
					container.processManager.ForcedCloseWindow(i);
				}
			}
			else
			{
				container.processManager.ForcedCloseWindow(playerIndex);
			}
		}

		public Sprite GetOptionValueSprite(string key)
		{
			if (!_optionValueSprites.ContainsKey(key))
			{
				return _optionValueSprites["UI_OPT_00_00"];
			}
			return _optionValueSprites[key];
		}

		public Sprite GetSortValueSprite(SortRootID root)
		{
			string key = "";
			switch (root)
			{
			case SortRootID.Tab:
				key = _categorySortSetting.GetFilePath();
				break;
			case SortRootID.Music:
				key = _musicSortSetting.GetFilePath();
				break;
			}
			if (!_sortValueSprites.ContainsKey(key))
			{
				return _optionValueSprites["UI_OPT_00_00"];
			}
			return _sortValueSprites[key];
		}

		public Texture2D[] GetGenreTextureList(int index = 0)
		{
			Texture2D[] array = new Texture2D[8];
			int num = _combineMusicDataList[CurrentCategorySelect].Count();
			if (num != 0)
			{
				for (int i = 0; i < 8; i++)
				{
					if (!_combineMusicDataList[CurrentCategorySelect][(i + index) % num].isWaitConnectScore)
					{
						string thumbnailName = _combineMusicDataList[CurrentCategorySelect][(i + index) % num].musicSelectData[(int)ScoreType].MusicData.thumbnailName;
						array[i] = container.assetManager.GetJacketThumbTexture2D(thumbnailName);
					}
				}
			}
			return array;
		}

		public Texture2D GetGenreTexture(int index)
		{
			Texture2D result = null;
			int num = _combineMusicDataList[CurrentCategorySelect].Count();
			if (num != 0 && !_combineMusicDataList[CurrentCategorySelect][(index + 8 - 1) % num].isWaitConnectScore)
			{
				string thumbnailName = _combineMusicDataList[CurrentCategorySelect][(index + 8 - 1) % num].musicSelectData[(int)ScoreType].MusicData.thumbnailName;
				result = container.assetManager.GetJacketThumbTexture2D(thumbnailName);
			}
			return result;
		}

		public SubSequence GetBeforeSubSeq(int playerIndex = -1)
		{
			int num = playerIndex;
			if (num == -1)
			{
				for (int i = 0; i < MonitorArray.Length; i++)
				{
					if (IsEntry(i))
					{
						num = i;
						break;
					}
				}
			}
			return _beforePlayerSubSequence[num];
		}

		public bool IsRemasterEnable(int index = 0)
		{
			int iD = GetCombineMusic(index).GetID(ScoreType);
			NotesWrapper notesWrapper = (Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(iD) ? Singleton<NotesListManager>.Instance.GetNotesList()[iD] : null);
			if (notesWrapper == null)
			{
				return true;
			}
			if (notesWrapper.IsEnable[4])
			{
				return true;
			}
			return false;
		}

		private bool IsRemasterEnableForMusicID(int musicID)
		{
			if (!Singleton<NotesListManager>.Instance.GetNotesList().ContainsKey(musicID))
			{
				return true;
			}
			return Singleton<NotesListManager>.Instance.GetNotesList()[musicID]?.IsEnable[4] ?? false;
		}

		public bool IsMusicSeq(int player = -1)
		{
			if (player == -1)
			{
				for (int i = 0; i < _currentPlayerSubSequence.Length; i++)
				{
					if (IsEntry(i))
					{
						player = i;
						break;
					}
				}
			}
			if (player >= 0 && player < _currentPlayerSubSequence.Length && _currentPlayerSubSequence[player] == SubSequence.Music)
			{
				return true;
			}
			return false;
		}

		public bool[] IsPlayableMusic(int musicID)
		{
			bool[] array = new bool[6];
			NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[musicID];
			for (MusicDifficultyID musicDifficultyID = MusicDifficultyID.Basic; musicDifficultyID <= MusicDifficultyID.ReMaster; musicDifficultyID++)
			{
				if (IsChallengeFolder() || IsMapTaskFolder())
				{
					array[(int)musicDifficultyID] = notesWrapper.IsEnable[(int)musicDifficultyID];
				}
				else
				{
					array[(int)musicDifficultyID] = notesWrapper.IsEnable[(int)musicDifficultyID] && notesWrapper.IsUnlock[(int)musicDifficultyID];
				}
			}
			return array;
		}

		public bool IsInputLocking(int monitorId)
		{
			return IsInputLock(monitorId);
		}

		public bool IsEntry(int playerIndex)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex).IsEntry;
		}

		public string GetSortName(SortRootID root)
		{
			string result = string.Empty;
			switch (root)
			{
			case SortRootID.Tab:
				result = _categorySortSetting.GetName();
				break;
			case SortRootID.Music:
				result = _musicSortSetting.GetName();
				break;
			}
			return result;
		}

		public string GetSortDatails(SortRootID root)
		{
			string result = string.Empty;
			switch (root)
			{
			case SortRootID.Tab:
				result = _categorySortSetting.GetDetail();
				break;
			case SortRootID.Music:
				result = _musicSortSetting.GetDetail();
				break;
			}
			return result;
		}

		public int GetSortValueIndex(SortRootID root)
		{
			int result = -1;
			switch (root)
			{
			case SortRootID.Tab:
				result = (int)_categorySortSetting;
				break;
			case SortRootID.Music:
				result = (int)_musicSortSetting;
				break;
			}
			return result;
		}

		public int GetSortMax(SortRootID root)
		{
			int result = -1;
			switch (root)
			{
			case SortRootID.Tab:
				result = SortTabID.End.GetEnd();
				break;
			case SortRootID.Music:
				result = SortMusicID.End.GetEnd();
				break;
			}
			return result;
		}

		public void AddSort(SortRootID root)
		{
			switch (root)
			{
			case SortRootID.Tab:
				_categorySortSetting = ((_categorySortSetting + 1 >= SortTabID.End) ? SortTabID.Name : (_categorySortSetting + 1));
				break;
			case SortRootID.Music:
				_musicSortSetting = ((_musicSortSetting + 1 >= SortMusicID.End) ? SortMusicID.Name : (_musicSortSetting + 1));
				break;
			}
		}

		public void SubSort(SortRootID root)
		{
			switch (root)
			{
			case SortRootID.Tab:
				_categorySortSetting = ((_categorySortSetting - 1 >= SortTabID.Genre) ? (_categorySortSetting - 1) : SortTabID.Genre);
				break;
			case SortRootID.Music:
				_musicSortSetting = ((_musicSortSetting - 1 >= SortMusicID.ID) ? (_musicSortSetting - 1) : SortMusicID.ID);
				break;
			}
		}

		public void CharacterSelectReset(int playerIndex)
		{
			_characterSelectProces?.Reset(playerIndex);
		}

		public void NotificationCharacterSelectProcess()
		{
			for (int i = 0; i < 2; i++)
			{
				if (_currentPlayerSubSequence[i] == SubSequence.Character)
				{
					_characterSelectProces.Escape(i);
					MonitorArray[i].RetrunCharacterSelect();
				}
			}
		}

		public bool IsUseCharacterSelect(int monitorId)
		{
			return _isUseCharacterSelect[monitorId];
		}

		public void ChangeBGM()
		{
			if (CurrentCategorySelect < _combineMusicDataList.Count && CurrentMusicSelect < _combineMusicDataList[CurrentCategorySelect].Count)
			{
				if (_combineMusicDataList[CurrentCategorySelect][CurrentMusicSelect].GetID(ScoreType) == 2)
				{
					SoundManager.PreviewEnd();
					SoundManager.PlayBGM(Cue.BGM_ENTRY, 2);
				}
				else if (_combineMusicDataList[CurrentCategorySelect][CurrentMusicSelect].GetID(ScoreType) != 0)
				{
					SoundManager.StopBGM(2);
					CombineMusicSelectData combineMusicSelectData = _combineMusicDataList[CurrentCategorySelect][CurrentMusicSelect];
					PlayingScoreID = combineMusicSelectData.GetID(ScoreType) * 100 + combineMusicSelectData.musicSelectData[(int)ScoreType].Difficulty;
					SoundManager.PreviewPlay(combineMusicSelectData.musicSelectData[(int)ScoreType].MusicData.cueName.id);
				}
			}
		}

		private void SetConnectData()
		{
			_connectCombineMusicDataList.Clear();
			IsConnectCategoryEnable = false;
			if (RecruitData != null)
			{
				int musicID = RecruitData.MusicID;
				CombineMusicSelectData combineMusicSelectData = new CombineMusicSelectData();
				MusicData music = Singleton<DataManager>.Instance.GetMusic(musicID);
				List<Notes> notesList = Singleton<NotesListManager>.Instance.GetNotesList()[musicID].NotesList;
				if (musicID < 10000)
				{
					combineMusicSelectData.existStandardScore = true;
				}
				else if (10000 < musicID && musicID < 20000)
				{
					combineMusicSelectData.existDeluxeScore = true;
				}
				for (int i = 0; i < 2; i++)
				{
					combineMusicSelectData.musicSelectData.Add(new MusicSelectData(music, notesList, 0));
				}
				_connectCombineMusicDataList.Add(combineMusicSelectData);
				string thumbnailName = music.thumbnailName;
				for (int j = 0; j < MonitorArray.Length; j++)
				{
					if (IsEntry(j))
					{
						MonitorArray[j].SetRecruitInfo(thumbnailName);
						SoundManager.PlaySE(Cue.SE_INFO_NORMAL, j);
					}
				}
				IsConnectingMusic = true;
			}
			else
			{
				CombineMusicSelectData combineMusicSelectData2 = new CombineMusicSelectData();
				for (int k = 0; k < 2; k++)
				{
					combineMusicSelectData2.musicSelectData.Add(null);
				}
				combineMusicSelectData2.isWaitConnectScore = true;
				_connectCombineMusicDataList.Add(combineMusicSelectData2);
				IsConnectingMusic = false;
			}
			if (MonitorArray == null)
			{
				return;
			}
			for (int l = 0; l < MonitorArray.Length; l++)
			{
				if (_currentPlayerSubSequence[l] != 0)
				{
					continue;
				}
				MonitorArray[l].SetDeployList(isAnimation: false);
				if (IsConnectionFolder())
				{
					ChangeBGM();
					if (IsEntry(l))
					{
						MonitorArray[l].SetVisibleButton(IsConnectingMusic, InputManager.ButtonSetting.Button04);
					}
				}
			}
		}

		private void reinputConnectCombineData()
		{
			_combineMusicDataList.RemoveAt(_combineMusicDataList.Count - 1);
			for (int i = 0; i < MonitorArray.Length; i++)
			{
				_genreSelectDataList[i].RemoveAt(_genreSelectDataList[i].Count - 1);
			}
			InputGenreSelectData(new ReadOnlyCollection<CombineMusicSelectData>(_connectCombineMusicDataList), Singleton<DataManager>.Instance.GetMusicGenre(198).name.id, isExtra: true);
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(_connectCombineMusicDataList));
		}

		private bool IsConnectStart()
		{
			if (!IsConnectingMusic)
			{
				RecruitInfo recruitInfo = Manager.Party.Party.Party.Get().GetRecruitListWithoutMe().FirstOrDefault();
				if (recruitInfo != null)
				{
					RecruitData = new RecruitInfo(recruitInfo);
					SetConnectData();
					return true;
				}
			}
			else
			{
				RecruitInfo recruitInfo2 = Manager.Party.Party.Party.Get().GetRecruitListWithoutMe().FirstOrDefault();
				if (recruitInfo2 != null && RecruitData.MechaInfo.GetIpAddress() != recruitInfo2.MechaInfo.GetIpAddress())
				{
					RecruitData = new RecruitInfo(recruitInfo2);
					SetConnectData();
					return true;
				}
			}
			return false;
		}

		private bool IsConnectStop()
		{
			if (IsConnectingMusic && Manager.Party.Party.Party.Get().GetRecruitListWithoutMe().FirstOrDefault() == null)
			{
				RecruitData = null;
				SetConnectData();
				return true;
			}
			return false;
		}

		private void PartyExec()
		{
			if (!Manager.Party.Party.Party.Get().IsHost() && !Manager.Party.Party.Party.Get().IsClient())
			{
				if (Manager.Party.Party.Party.Get().IsRequest() || Manager.Party.Party.Party.Get().IsConnect())
				{
					return;
				}
				if (IsConnectStop() || IsConnectStart())
				{
					reinputConnectCombineData();
				}
				if (RecruitData != null && JoinActive)
				{
					UserData[] array = new UserData[2]
					{
						new UserData(),
						new UserData()
					};
					for (int i = 0; i < 2; i++)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
						array[i].IsEntry = userData.IsEntry;
						array[i].Detail.UserID = userData.Detail.UserID;
						array[i].Detail.UserName = userData.Detail.UserName;
						array[i].Detail.EquipIconID = userData.Detail.EquipIconID;
					}
					MusicDifficultyID[] fumenDifs = new MusicDifficultyID[2]
					{
						CurrentDifficulty[0],
						CurrentDifficulty[1]
					};
					Manager.Party.Party.Party.Get().SelectMusic();
					Manager.Party.Party.Party.Get().StartJoin(RecruitData.IpAddress, new MechaInfo(array, fumenDifs, GetCombineMusic(0).GetID(ScoreType)));
					JoinActive = false;
				}
				if (RecruitActive)
				{
					UserData[] array2 = new UserData[2]
					{
						new UserData(),
						new UserData()
					};
					for (int j = 0; j < 2; j++)
					{
						UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(j);
						array2[j].IsEntry = userData2.IsEntry;
						array2[j].Detail.UserID = userData2.Detail.UserID;
						array2[j].Detail.UserName = userData2.Detail.UserName;
						array2[j].Detail.EquipIconID = userData2.Detail.EquipIconID;
					}
					MusicDifficultyID[] fumenDifs2 = new MusicDifficultyID[2]
					{
						(MusicDifficultyID)DifficultySelectIndex[0],
						(MusicDifficultyID)DifficultySelectIndex[1]
					};
					Manager.Party.Party.Party.Get().StartRecruit(new MechaInfo(array2, fumenDifs2, GetCombineMusic(0).GetID(ScoreType)), RecruitStance.OnlyFriend);
					RecruitActive = false;
				}
				if (PrepareFinish)
				{
					ConnectMusicAllDecide = true;
				}
			}
			else if (Manager.Party.Party.Party.Get().IsHost())
			{
				if (IsConnectStop() || IsConnectStart())
				{
					reinputConnectCombineData();
				}
				if (RecruitCancel)
				{
					Manager.Party.Party.Party.Get().CancelBothRecruitJoin();
					RecruitCancel = false;
					_clientFinishDeploy = false;
					_connectNum = 0;
					return;
				}
				if (PrepareFinish)
				{
					if (IsActiveJoined())
					{
						if (IsClientFinishSetting())
						{
							Manager.Party.Party.Party.Get().GoToTrackStart();
						}
						else
						{
							Manager.Party.Party.Party.Get().GoToTrackStart();
						}
					}
					else
					{
						Manager.Party.Party.Party.Get().Wait();
					}
					ConnectMusicAllDecide = true;
				}
				if (!_clientFinishDeploy && IsClientFinishSetting())
				{
					for (int k = 0; k < MonitorArray.Length; k++)
					{
						if (_currentPlayerSubSequence[k] == SubSequence.Menu && !IsPreparationCompletes[k])
						{
							MonitorArray[k].SetMenuReset();
						}
					}
					_clientFinishDeploy = true;
				}
				if (_connectNum != Manager.Party.Party.Party.Get().GetEntryNumber())
				{
					for (int l = 0; l < MonitorArray.Length; l++)
					{
						if (_currentPlayerSubSequence[l] == SubSequence.Menu && !IsPreparationCompletes[l])
						{
							MonitorArray[l].SetMenuReset();
						}
					}
					IEnumerable<MechaInfo> joinMembersWithoutMe = Manager.Party.Party.Party.Get().GetPartyMemberInfo().GetJoinMembersWithoutMe();
					if (_connectNum < Manager.Party.Party.Party.Get().GetEntryNumber())
					{
						foreach (MechaInfo item in joinMembersWithoutMe)
						{
							int num = 0;
							Texture2D[] array3 = new Texture2D[MonitorArray.Length];
							string[] array4 = new string[MonitorArray.Length];
							string[] array5 = new string[MonitorArray.Length];
							for (int m = 0; m < MonitorArray.Length; m++)
							{
								if (item.Entrys[num])
								{
									array3[m] = container.assetManager.GetIconTexture2D(num + 2, item.IconIDs[num]);
									array4[m] = num + 2 + 1 + "P";
									array5[m] = item.UserNames[num];
								}
								else
								{
									array3[m] = null;
									array4[m] = "";
									array5[m] = "";
								}
								num++;
							}
							for (int n = 0; n < MonitorArray.Length; n++)
							{
								if (IsEntry(n))
								{
									MonitorArray[n].SetConnectingInfo(array3, array4, array5);
									SoundManager.PlaySE(Cue.SE_INFO_NORMAL, n);
								}
							}
						}
						_clientFinishDeploy = false;
					}
					else
					{
						for (int num2 = 0; num2 < MonitorArray.Length; num2++)
						{
							if (IsEntry(num2) && !ConnectMusicAllDecide)
							{
								MonitorArray[num2].SetCancelInfo();
								SoundManager.PlaySE(Cue.SE_INFO_NORMAL, num2);
							}
						}
					}
					_connectNum = Manager.Party.Party.Party.Get().GetEntryNumber();
				}
				UpdateChangeInfo();
			}
			else
			{
				if (!Manager.Party.Party.Party.Get().IsClient() || Manager.Party.Party.Party.Get().IsRequest())
				{
					return;
				}
				if (IsHostGameStart())
				{
					ConnectMusicAllDecide = true;
					if (!PrepareFinish)
					{
						OnGameStart();
						ChangeUserInfo();
						if (!_isHostWait)
						{
							CallMessage(WindowMessageID.MusicSelectForceTrackStart);
						}
						else
						{
							CloseWindow();
						}
					}
					PrepareFinish = false;
					RecruitCancel = false;
				}
				else if (IsHostDisconnect())
				{
					Manager.Party.Party.Party.Get().CancelBothRecruitJoin();
					PrepareFinish = false;
					RecruitCancel = false;
					_isHostWait = false;
					CallMessage(WindowMessageID.MusicSelectCanceRecruit);
					IsForceMusicBack = true;
					IsPreparationCompletes[0] = (IsPreparationCompletes[1] = false);
					IsForceMusicBackConfirm[0] = (IsForceMusicBackConfirm[1] = false);
					MonitorArray[0].CloseConnectCancelWarning();
					MonitorArray[1].CloseConnectCancelWarning();
					container.processManager.SetTimerSecurity(10, 0, TimerCountUp);
				}
				else if (_isTimeUpConnect)
				{
					Manager.Party.Party.Party.Get().FinishSetting();
					PrepareFinish = false;
					_isHostWait = true;
					_isTimeUpConnect = false;
					CallMessage(WindowMessageID.MusicSelectWaitToHost);
					container.processManager.SetTimerSecurity(0, 0, TimerCountUp);
				}
				else if (PrepareFinish)
				{
					Manager.Party.Party.Party.Get().FinishSetting();
					PrepareFinish = false;
					_isHostWait = true;
					CallMessage(WindowMessageID.MusicSelectWaitToHost);
				}
				else if (RecruitCancel)
				{
					Manager.Party.Party.Party.Get().CancelBothRecruitJoin();
					RecruitCancel = false;
				}
				UpdateChangeInfo();
			}
		}

		private void UpdateChangeInfo()
		{
			IEnumerable<MechaInfo> joinMembersWithoutMe = Manager.Party.Party.Party.Get().GetPartyMemberInfo().GetJoinMembersWithoutMe();
			bool flag = false;
			foreach (MechaInfo item in joinMembersWithoutMe)
			{
				for (int i = 0; i < item.Entrys.Length; i++)
				{
					if (item.Entrys[i] && _otherDifficulty[i] != (MusicDifficultyID)item.FumenDifs[i])
					{
						_otherDifficulty[i] = (MusicDifficultyID)item.FumenDifs[i];
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			for (int j = 0; j < MonitorArray.Length; j++)
			{
				if (_currentPlayerSubSequence[j] == SubSequence.Menu && !IsPreparationCompletes[j])
				{
					MonitorArray[j].SetMenuReset();
				}
			}
		}

		public void ChangeUserInfo()
		{
			UserData[] array = new UserData[2]
			{
				new UserData(),
				new UserData()
			};
			for (int i = 0; i < 2; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				array[i].IsEntry = userData.IsEntry;
				array[i].Detail.UserID = userData.Detail.UserID;
				array[i].Detail.UserName = userData.Detail.UserName;
				array[i].Detail.EquipIconID = userData.Detail.EquipIconID;
			}
			MusicDifficultyID[] fumenDifs = new MusicDifficultyID[2]
			{
				(MusicDifficultyID)DifficultySelectIndex[0],
				(MusicDifficultyID)DifficultySelectIndex[1]
			};
			Manager.Party.Party.Party.Get().SendMechaInfo(new MechaInfo(array, fumenDifs, GetCombineMusic(0).GetID(ScoreType)));
		}

		private bool IsJoinedFromClient()
		{
			bool result = false;
			foreach (MechaInfo item in Manager.Party.Party.Party.Get().GetPartyMemberInfo().GetJoinMembersWithoutMe())
			{
				_ = item;
				result = true;
			}
			return result;
		}

		public bool IsClientFinishSetting()
		{
			int entryNumber = Manager.Party.Party.Party.Get().GetEntryNumber();
			int num = 0;
			PartyMemberState partyMemberState = Manager.Party.Party.Party.Get().GetPartyMemberState();
			for (int i = 0; i < partyMemberState.StateList.Length; i++)
			{
				if (partyMemberState.GetState(i) != 0 && partyMemberState.GetState(i) == PartyPartyClientStateID.FinishSetting)
				{
					num++;
				}
			}
			return entryNumber - 1 <= num;
		}

		public bool IsActiveJoined()
		{
			return Manager.Party.Party.Party.Get().GetEntryNumber() > 1;
		}

		private bool IsHostGameStart()
		{
			if (Manager.Party.Party.Party.Get().IsClient())
			{
				return Manager.Party.Party.Party.Get().IsClientToReady();
			}
			return false;
		}

		private bool IsHostDisconnect()
		{
			return !Manager.Party.Party.Party.Get().IsJoinAndActive();
		}

		private bool IsJoinConnect()
		{
			return Manager.Party.Party.Party.Get().GetClientStateID() == PartyPartyClientStateID.Connect;
		}

		private bool IsJoinRequest()
		{
			return Manager.Party.Party.Party.Get().GetClientStateID() == PartyPartyClientStateID.Request;
		}

		private void DebugTest()
		{
			_combineMusicDataList.Clear();
			CategoryNameList.Clear();
			List<CombineMusicSelectData> list = new List<CombineMusicSelectData>();
			MusicData music = Singleton<DataManager>.Instance.GetMusic(381);
			list.Add(new CombineMusicSelectData(music.GetID(), 3));
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(list));
			list = new List<CombineMusicSelectData>();
			music = Singleton<DataManager>.Instance.GetMusic(125);
			list.Add(new CombineMusicSelectData(music.GetID(), 3));
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(list));
			list = new List<CombineMusicSelectData>();
			music = Singleton<DataManager>.Instance.GetMusic(792);
			list.Add(new CombineMusicSelectData(music.GetID(), 3));
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(list));
			list = new List<CombineMusicSelectData>();
			music = Singleton<DataManager>.Instance.GetMusic(125);
			list.Add(new CombineMusicSelectData(music.GetID(), 3));
			_combineMusicDataList.Add(new ReadOnlyCollection<CombineMusicSelectData>(list));
			string[] array = new string[4] { "ケース１", "ケース２", "ケース３", "ケース４" };
			foreach (string item in array)
			{
				CategoryNameList.Add(item);
			}
			CategoryMedal();
		}
	}
}
