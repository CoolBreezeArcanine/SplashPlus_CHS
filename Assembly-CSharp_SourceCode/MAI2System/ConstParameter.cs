using DB;
using Mai2.Voice_000001;
using Manager;
using UnityEngine;

namespace MAI2System
{
	public static class ConstParameter
	{
		public enum ErrorID
		{
			None = 0,
			CameraNotFill = 3100,
			CodeReader_InitializeFailed = 3101,
			PhotoCamera_NotFound = 3102,
			GroupRole_DuplicateParent = 3201,
			TouchPanel_Left_OpenError = 3300,
			TouchPanel_Left_InitError = 3301,
			TouchPanel_Right_OpenError = 3302,
			TouchPanel_Right_InitError = 3303,
			GraphicBoard_RenderError = 912,
			AimeConnectError = 6501,
			AimeFiruUpdateError = 6506
		}

		public enum ScoreKind
		{
			Standard,
			Deluxe,
			Max
		}

		public const float ScreenOneWidth = 1080f;

		public const float ScreenWidth = 2160f;

		public const float ScreenHeight = 1920f;

		public static readonly Vector2 ScreenSize = new Vector2(2160f, 1920f);

		public const long GuestUserIDBase = 281474976710657L;

		public const long ItemKindConvert = 10000000000L;

		public static int PresentKindConvert = 1000000;

		public const uint GameID = 5915972u;

		public const string GameIDStr = "SDEZ";

		public static string DllHashDev = "f4efc2f7f9f33f8e969c0330179f9c0e7c623de38b23db3545a742ac5425c896";

		public static string DllHash = "085a1a629338e5ab2a13d669293bcd33702fe550891dcb579866acfc5dd3438d";

		public const uint CardVersionID = 4u;

		public const uint NowGameVersion = 21500u;

		public const int MaxCardRequestCount = 50;

		public const int MaxCharacterRequestCount = 50;

		public const int MaxMusicRequestCount = 50;

		public static readonly Vector2Int PhotoSize = new Vector2Int(1056, 594);

		public const int PhotoQuality = 100;

		public const int GamePhotoJpgQuality = 100;

		public const int IconJpgQuality = 50;

		public const int MaxFavoriteCount = 20;

		public static readonly string TestString_Good = CommonMessageID.SystemGood.GetName();

		public static readonly string TestString_Bad = CommonMessageID.SystemBad.GetName();

		public static readonly string TestString_Check = CommonMessageID.SystemCheck.GetName();

		public static readonly string TestString_NA = CommonMessageID.SystemNa.GetName();

		public static readonly string TestString_Warn = CommonMessageID.SystemWarn.GetName();

		public const string AnimIdleName = "Idle";

		public const string AnimInName = "In";

		public const string AnimLoopName = "Loop";

		public const string AnimOutName = "Out";

		public const int Login_Error = -1;

		public const int Login_None = 0;

		public const int Login_Success = 1;

		public const int MusicIdEx_NewTutorial = 0;

		public const int MusicIdEx_BasicTutorial = 1;

		public const int MusicIdEx_WaitConnect = 2;

		public const int MusicIdEx_End = 4;

		public const int DxRatingActivity = 1000;

		public const int ScoreBaseID = 0;

		public const int ScoreConstNum = 10000;

		public const int DxScoreBaseID = 10000;

		public const int StrongScoreBaseID = 20000;

		public const int EndScoreBaseID = 30000;

		public const int DefaultCharaID = 101;

		public const MusicDifficultyID NormalDifficultyMax = MusicDifficultyID.ReMaster;

		public const int GenreBaseUp = 1000;

		public const int TournamentBaseUp = 10000;

		public const Cue CommonDialogVoice = Cue.VO_000228;

		public const int RhythmTestMusicID = 854;

		public const int RhythmTestMusicDifficulty = 3;

		public const OptionDispjudgeID RhythmTestDispJudgeType = OptionDispjudgeID.Type2A;
	}
}
