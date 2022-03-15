using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DB;
using MAI2.Util;
using MAI2System;
using UnityEngine;
using Util;

namespace Manager
{
	public static class GameManager
	{
		public enum TargetID
		{
			Left,
			Right,
			End
		}

		public enum PlayerID
		{
			ThisLeft,
			ThisRight,
			OtherLeft,
			OtherRight,
			End
		}

		public enum SpecialKind
		{
			None,
			Tap,
			Hold,
			Slide,
			Break,
			ExTap,
			Touch,
			TouchHold
		}

		public enum TutorialEnum
		{
			NotPlay,
			BasicPlay,
			NewPlay
		}

		public enum AutoPlayMode
		{
			None,
			Critical,
			Perfect,
			Great,
			Good,
			Random
		}

		public const int SpecialMax = 67;

		public const int MaxRivalNum = 3;

		public const int MaxButtonNum = 8;

		public const int MaxCharaSlotNum = 5;

		public const int MaxMapNpcNum = 3;

		public const int MaxUserGhostNum = 10;

		public const int MaxCardLogNum = 5;

		public const uint FREEDOM_MODE_TIME_MSEC = 600000u;

		public const uint FREEDOM_CARD_ADDTIME_MSEC = 120000u;

		public const long GuestUserIDBase = 281474976710657L;

		public const uint MaxPlayTrackCount = 4u;

		public const uint MaxNormalPlayTrackCount = 3u;

		public const uint MaxNewFreePlayTrackCount = 1u;

		public const uint MaxCourseTrackCount = 4u;

		public static uint TempMaxTrackCount = 1u;

		public const int RecvItemDivNum = 100;

		public const int RecvMusicDivNum = 50;

		public const int RecvActivityDivNum = 100;

		public const int RecvCourseDivNum = 50;

		public const int TrnsItemDivNum = 40;

		public const int TrnsMusicDivNum = 20;

		public const int TrnsFriendDivNum = 20;

		public const int TrnsRankGradeDivNum = 20;

		public static string StreamingAssetsPath;

		public static bool IsGotoSystemTest = false;

		public static bool IsGotoReboot = false;

		public static bool IsException = false;

		public static bool IsGotoSystemError = false;

		public static bool IsErrorMode = false;

		public static bool IsGameProcMode = false;

		public static bool IsInitializeEnd = false;

		private static readonly Stopwatch GameMasterTimer = Stopwatch.StartNew();

		private static long _gameMasterPreTime;

		private static long _gameMasterTime;

		private static float _gameMasterFrame;

		private static float _gameMasterAddFrame;

		private static long _gameMasterPreRealTime;

		private static long _freedomTime = 600000L;

		private static double _gameMasterTimeD;

		private static double _gameMasterPreTimeD;

		public const int FreedomModeMusicSelectTimerShowTime = 100000;

		public static readonly float MainMonitorSize = 1080f;

		public static readonly float SubMonitorHeight = 478f;

		public const decimal TheoryMaxAchive = 101.0m;

		public const decimal TheoryAchive = 100.0m;

		public static readonly float TheoryRateBorderNum = 1010000f * (float)RatingTableID.Rate_22.GetOffset() * 100000f;

		private static bool _init = false;

		public static uint MusicTrackNumber = 1u;

		public static readonly float Speed = 1f;

		private static readonly ReadOnlyCollection<NoteJudge.ETiming> RandTiming = Array.AsReadOnly(new NoteJudge.ETiming[4]
		{
			NoteJudge.ETiming.TooLate,
			NoteJudge.ETiming.LateGood,
			NoteJudge.ETiming.LateGreat,
			NoteJudge.ETiming.Critical
		});

		private static bool[] _selectResultDetails = new bool[2];

		public const int AchiveDeimalToIntParam = 10000;

		public const decimal AchiveIntToDecimalParam = 10000m;

		public static bool IsNoteCheckMode;

		public static string NoteCheckPath;

		public static bool IsMovieCheckMode;

		public static int DebugAchievement;

		public static bool IsEventMode => SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode;

		public static bool IsCourseMode { get; set; }

		public static bool IsFreedomMode { get; set; }

		public static bool IsFreedomCountDown { get; private set; }

		public static bool IsFreedomTimerPause { get; private set; }

		public static bool IsFreedomTimeUp { get; set; }

		public static bool IsInGame { get; set; }

		public static bool IsCardFreedomAddTime { get; private set; }

		public static bool IsCardOpenMaster { get; private set; }

		public static bool[] IsCardOpenRating { get; private set; } = new bool[4];


		public static SpecialKind SpecialKindNum { get; set; } = SpecialKind.None;


		public static int[] SelectMusicID { get; set; } = new int[4];


		public static int[] SelectDifficultyID { get; set; } = new int[4];


		public static GhostManager.GhostTarget[] SelectGhostID { get; set; } = new GhostManager.GhostTarget[2];


		public static GhostManager.GhostTarget SelectedDeleteGhostID { get; set; } = GhostManager.GhostTarget.End;


		public static bool[] EncountNewNpcGhost { get; set; } = new bool[2];


		public static bool IsPerfectChallenge { get; set; } = false;


		public static bool IsMaxTrack { get; set; } = false;


		public static bool IsPhotoAgree { get; set; } = false;


		public static bool IsGotoPhotoShoot { get; set; }

		public static bool IsGotoCodeRead { get; set; } = false;


		public static bool IsGotoCharacterSelect { get; set; }

		public static TutorialEnum TutorialPlayed { get; set; } = TutorialEnum.NotPlay;


		public static bool[] IsSelectContinue { get; set; } = new bool[2];


		public static AutoPlayMode AutoPlay { get; set; }

		public static int CategoryIndex { get; set; }

		public static int MusicIndex { get; set; }

		public static int ExtraFlag { get; set; }

		public static int SelectScoreType { get; set; }

		public static bool IsForceChangeMusic { get; set; }

		public static bool[] IsSelectResultDetails
		{
			get
			{
				return _selectResultDetails;
			}
			set
			{
				_selectResultDetails = value;
			}
		}

		public static SortTabID SortCategorySetting { get; set; }

		public static SortMusicID SortMusicSetting { get; set; }

		public static Texture2D[] FaceIconTexture { get; set; } = new Texture2D[2];


		public static bool[] IsTakeFaceIcon { get; set; } = new bool[2];


		public static bool[] IsUploadPhoto { get; set; } = new bool[2];


		public static int[] PhotoTrackNo { get; set; } = new int[2];


		public static Texture2D[] PhotoTexture { get; set; } = new Texture2D[2];


		public static int[] RandomSeed { get; set; } = new int[4];


		public static bool IsTutorial { get; set; } = false;


		public static bool IsAdvDemo { get; set; } = false;


		public static bool NextMapSelect { get; set; } = false;


		public static bool NextCharaSelect { get; set; } = false;


		private static double GetGameMicroSec()
		{
			return GetGameMilliSec() * 1000.0;
		}

		private static double GetGameMilliSec()
		{
			return (double)GameMasterTimer.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0;
		}

		public static void UpdateGameTimer()
		{
			_gameMasterPreTime = _gameMasterTime;
			_gameMasterTime = GameMasterTimer.ElapsedMilliseconds;
			_gameMasterFrame = (float)GetGameMSec() / 16.666666f;
			_gameMasterAddFrame = (float)GetGameMSecAdd() / 16.666666f;
			_gameMasterPreRealTime = GameMasterTimer.ElapsedMilliseconds;
			_gameMasterPreTimeD = _gameMasterTimeD;
			_gameMasterTimeD = GetGameMilliSec();
			if (_freedomTime <= 0)
			{
				IsFreedomCountDown = false;
			}
			if (IsFreedomMode && IsFreedomCountDown && !IsFreedomTimerPause)
			{
				_freedomTime -= GetGameMSecAdd();
				if (DebugInput.GetKey(KeyCode.F4))
				{
					_freedomTime -= GetGameMSecAdd() * 50;
				}
			}
		}

		public static long GetGameRealMSec()
		{
			return GameMasterTimer.ElapsedMilliseconds;
		}

		public static long GetGameRealAddMSec()
		{
			return GetGameRealMSec() - _gameMasterPreRealTime;
		}

		public static long GetGameMSec()
		{
			return _gameMasterTime;
		}

		public static float GetGameFrame()
		{
			return _gameMasterFrame;
		}

		public static double GetGameMSecD()
		{
			return _gameMasterTimeD;
		}

		public static double GetGameMSecAddD()
		{
			return _gameMasterTimeD - _gameMasterPreTimeD;
		}

		public static long GetGameMSecAdd()
		{
			return _gameMasterTime - _gameMasterPreTime;
		}

		public static float GetGameFrameAdd()
		{
			return _gameMasterAddFrame;
		}

		public static long GetFreedomStartTime()
		{
			return (uint)(IsCardFreedomAddTime ? 720000 : 600000);
		}

		public static long GetFreedomModeMSec()
		{
			return _freedomTime;
		}

		public static bool IsFreedomMapSkip()
		{
			if (!IsFreedomMode)
			{
				return false;
			}
			return true;
		}

		public static bool IsGotoGameOver()
		{
			return MusicTrackNumber >= GetMaxTrackCount();
		}

		public static bool IsFinalTrack(uint track)
		{
			return track == GetMaxTrackCount();
		}

		public static uint GetMaxTrackCount()
		{
			if (IsFreedomMode)
			{
				return MusicTrackNumber;
			}
			return TempMaxTrackCount;
		}

		public static void SetMaxTrack()
		{
			if (IsEventMode)
			{
				TempMaxTrackCount = (uint)SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.EventModeTrack.GetTrack();
			}
			else if (IsCourseMode)
			{
				TempMaxTrackCount = 4u;
			}
			else if (IsMaxTrack)
			{
				TempMaxTrackCount = 4u;
			}
			else
			{
				bool flag = (Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(0L).UserType == UserData.UserIDType.New) || (Singleton<UserDataManager>.Instance.GetUserData(1L).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(1L).UserType == UserData.UserIDType.New);
				if (Singleton<UserDataManager>.Instance.IsSingleUser() && flag)
				{
					TempMaxTrackCount = 1u;
				}
				else
				{
					TempMaxTrackCount = 3u;
				}
			}
			if (Singleton<SystemConfig>.Instance.config.MaxTrack > 0)
			{
				TempMaxTrackCount = (uint)Singleton<SystemConfig>.Instance.config.MaxTrack;
			}
		}

		public static void StartFreedomModeTimer(long startTimeSec)
		{
			IsFreedomCountDown = true;
			IsFreedomTimerPause = false;
			IsFreedomTimeUp = false;
			_freedomTime = startTimeSec;
		}

		public static void PauseFreedomModeTimer(bool isPause)
		{
			IsFreedomTimerPause = isPause;
		}

		public static void ForcedTerminationForFreedomMode()
		{
			_freedomTime = 0L;
		}

		public static void Initialize()
		{
			if (!_init)
			{
				_init = true;
			}
			Clear();
		}

		public static void Clear()
		{
			for (int i = 0; i < 4; i++)
			{
				SelectMusicID[i] = 0;
				SelectDifficultyID[i] = 0;
			}
			for (int j = 0; j < 2; j++)
			{
				IsSelectContinue[j] = false;
				IsSelectResultDetails[j] = false;
				SelectGhostID[j] = GhostManager.GhostTarget.End;
				FaceIconTexture[j] = null;
				IsTakeFaceIcon[j] = false;
				EncountNewNpcGhost[j] = false;
				IsUploadPhoto[j] = false;
				PhotoTrackNo[j] = 0;
				PhotoTexture[j] = null;
				IsCardOpenRating[j] = false;
			}
			SelectedDeleteGhostID = GhostManager.GhostTarget.End;
			IsPerfectChallenge = false;
			CategoryIndex = 0;
			MusicIndex = 0;
			ExtraFlag = 0;
			SelectScoreType = 1;
			IsForceChangeMusic = false;
			SortCategorySetting = SortTabID.Genre;
			SortMusicSetting = SortMusicID.ID;
			MusicTrackNumber = 1u;
			IsCourseMode = false;
			SpecialKindNum = SpecialKind.None;
			IsFreedomMode = false;
			IsMaxTrack = false;
			IsFreedomCountDown = false;
			IsFreedomTimerPause = false;
			IsFreedomTimeUp = false;
			IsPhotoAgree = false;
			TutorialPlayed = TutorialEnum.NotPlay;
			IsCardFreedomAddTime = false;
			IsCardOpenMaster = false;
			NextMapSelect = false;
			NextCharaSelect = false;
			IsInGame = false;
			Singleton<GamePlayManager>.Instance.ClaerLog();
			TimeManager.Reset();
			Singleton<MapMaster>.Instance.IsCreateMapData = false;
			Singleton<MapMaster>.Instance.ClearConvertMapDistance();
			IsGotoPhotoShoot = false;
			IsGotoCodeRead = false;
			IsGotoCharacterSelect = false;
			IsTutorial = false;
			IsAdvDemo = false;
			IsNoteCheckMode = false;
			IsMovieCheckMode = false;
			NoteCheckPath = "";
			DebugAchievement = 0;
		}

		public static float GetNoteSpeedForBeat(int index)
		{
			return 1000f / (GetNoteSpeed(index) / 60f);
		}

		public static float GetTouchSpeedForBeat(int index)
		{
			return 1000f / (GetTouchSpeed(index) / 60f);
		}

		public static float GetNoteSpeed(int index)
		{
			return ((OptionNotespeedID)index).GetValue();
		}

		public static float GetTouchSpeed(int index)
		{
			return ((OptionTouchspeedID)index).GetValue();
		}

		public static bool IsAutoPlay()
		{
			return AutoPlay != AutoPlayMode.None;
		}

		public static NoteJudge.ETiming AutoJudge()
		{
			return AutoPlay switch
			{
				AutoPlayMode.Critical => NoteJudge.ETiming.Critical, 
				AutoPlayMode.Perfect => NoteJudge.ETiming.LatePerfect2nd, 
				AutoPlayMode.Great => NoteJudge.ETiming.LateGreat, 
				AutoPlayMode.Good => NoteJudge.ETiming.LateGood, 
				AutoPlayMode.Random => RandTiming[UnityEngine.Random.Range(0, RandTiming.Count)], 
				_ => NoteJudge.ETiming.TooFast, 
			};
		}

		public static MusicClearrankID GetClearRank(int achievement)
		{
			for (int i = 0; i < 13; i++)
			{
				if (achievement < Singleton<DataManager>.Instance.GetMusicClearRank(i + 1).achieve)
				{
					return (MusicClearrankID)i;
				}
			}
			return MusicClearrankID.Rank_SSSP;
		}

		public static MusicClearrankID GetClearRank(uint achievement)
		{
			return GetClearRank((int)achievement);
		}

		public static MusicClearrankID GetClearRank(decimal achievement)
		{
			return GetClearRank(ConvAchiveDecimalToInt(achievement));
		}

		public static MusicLevelID GetMusicLevelID(int levelx10)
		{
			if (levelx10 < 10)
			{
				return MusicLevelID.None;
			}
			if (levelx10 < 20)
			{
				return MusicLevelID.Level1;
			}
			if (levelx10 < 30)
			{
				return MusicLevelID.Level2;
			}
			if (levelx10 < 40)
			{
				return MusicLevelID.Level3;
			}
			if (levelx10 < 50)
			{
				return MusicLevelID.Level4;
			}
			if (levelx10 < 60)
			{
				return MusicLevelID.Level5;
			}
			if (levelx10 < 70)
			{
				return MusicLevelID.Level6;
			}
			if (levelx10 < 77)
			{
				return MusicLevelID.Level7;
			}
			if (levelx10 < 80)
			{
				return MusicLevelID.Level7P;
			}
			if (levelx10 < 87)
			{
				return MusicLevelID.Level8;
			}
			if (levelx10 < 90)
			{
				return MusicLevelID.Level8P;
			}
			if (levelx10 < 97)
			{
				return MusicLevelID.Level9;
			}
			if (levelx10 < 100)
			{
				return MusicLevelID.Level9P;
			}
			if (levelx10 < 107)
			{
				return MusicLevelID.Level10;
			}
			if (levelx10 < 110)
			{
				return MusicLevelID.Level10P;
			}
			if (levelx10 < 117)
			{
				return MusicLevelID.Level11;
			}
			if (levelx10 < 120)
			{
				return MusicLevelID.Level11P;
			}
			if (levelx10 < 127)
			{
				return MusicLevelID.Level12;
			}
			if (levelx10 < 130)
			{
				return MusicLevelID.Level12P;
			}
			if (levelx10 < 137)
			{
				return MusicLevelID.Level13;
			}
			if (levelx10 < 140)
			{
				return MusicLevelID.Level13P;
			}
			if (levelx10 < 147)
			{
				return MusicLevelID.Level14;
			}
			if (levelx10 < 150)
			{
				return MusicLevelID.Level14P;
			}
			if (levelx10 < 157)
			{
				return MusicLevelID.Level15;
			}
			return MusicLevelID.None;
		}

		public static int ConvAchiveDecimalToInt(decimal achive)
		{
			return (int)(achive * 10000m);
		}

		public static decimal ConvAchiveIntToDecimal(int achive)
		{
			return (decimal)achive / 10000m;
		}

		public static ConstParameter.ScoreKind GetScoreKind(int musicID)
		{
			if (musicID < 10000)
			{
				return ConstParameter.ScoreKind.Standard;
			}
			if (musicID < 20000)
			{
				return ConstParameter.ScoreKind.Deluxe;
			}
			return ConstParameter.ScoreKind.Max;
		}

		public static DeluxcorerankrateID GetDeluxcoreRank(int percent)
		{
			for (DeluxcorerankrateID deluxcorerankrateID = DeluxcorerankrateID.Rank_00; deluxcorerankrateID < DeluxcorerankrateID.End; deluxcorerankrateID++)
			{
				if (percent < deluxcorerankrateID.GetAchieve())
				{
					return deluxcorerankrateID - 1;
				}
			}
			return DeluxcorerankrateID.Rank_05;
		}

		public static bool IsRhythmTestMusicID(int monIndex)
		{
			return SelectMusicID[monIndex] == 854;
		}

		public static bool ForceHideNote(int monIndex)
		{
			if (SelectMusicID[monIndex] == 854)
			{
				return SelectDifficultyID[monIndex] >= 3;
			}
			return false;
		}

		public static void UpdateRandom()
		{
			for (int i = 0; i < 4; i++)
			{
				RandomSeed[i] = UnityEngine.Random.Range(0, int.MaxValue);
			}
		}

		public static void SetCardEffect(int cardBit, int playerIndex)
		{
			if (((uint)cardBit & (true ? 1u : 0u)) != 0)
			{
				IsCardOpenMaster = true;
			}
			if (((uint)cardBit & 2u) != 0)
			{
				IsCardOpenRating[playerIndex] = true;
			}
			if (((uint)cardBit & 4u) != 0)
			{
				IsCardFreedomAddTime = true;
			}
		}
	}
}
