using DB;
using MAI2.Util;

namespace Manager.Achieve
{
	public class AchieveTrackData
	{
		public int MusicId;

		public PlayComboflagID ComboType;

		public int PlayerNum;

		public MusicClearrankID Rank;

		public uint ComboNum;

		public uint ChainNum;

		public PlaySyncflagID SyncType;

		public MusicDifficultyID Difficulty;

		public int EquipTitle;

		public int EquipPlate;

		public int EquipIcon;

		public int EquipFrame;

		public int[] CharaSlot;

		public int Satellite;

		public uint MissNum;

		public OptionNotespeedID NoteSpeed;

		public OptionTouchspeedID TouchSpeed;

		public OptionSlidespeedID SlideSpeed;

		public uint Rating;

		public uint GradeRating;

		public uint MusicRating;

		public uint VsRank;

		public UdemaeID ClassID;

		public ulong OtomodatiID;

		public bool OtomodatiVsWin;

		public static AchieveTrackData Create(int index)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(index);
			GameScoreList ghostScore = Singleton<GamePlayManager>.Instance.GetGhostScore();
			return new AchieveTrackData
			{
				MusicId = gameScore.SessionInfo.musicId,
				ComboType = gameScore.ComboType,
				PlayerNum = Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum(),
				Rank = GameManager.GetClearRank(gameScore.Achivement),
				ComboNum = gameScore.Combo,
				ChainNum = gameScore.Chain,
				SyncType = gameScore.SyncType,
				Difficulty = (MusicDifficultyID)gameScore.SessionInfo.difficulty,
				EquipTitle = userData.Detail.EquipTitleID,
				EquipPlate = userData.Detail.EquipPlateID,
				EquipIcon = userData.Detail.EquipIconID,
				EquipFrame = userData.Detail.EquipFrameID,
				CharaSlot = userData.Detail.CharaSlot,
				Satellite = index,
				MissNum = gameScore.MissNum,
				NoteSpeed = gameScore.UserOption.GetNoteSpeed,
				TouchSpeed = gameScore.UserOption.GetTouchSpeed,
				SlideSpeed = gameScore.UserOption.SlideSpeed,
				Rating = userData.Detail.Rating,
				GradeRating = userData.Detail.GradeRating,
				MusicRating = userData.Detail.MusicRating,
				VsRank = gameScore.VsRank,
				ClassID = userData.RatingList.Udemae.ClassID,
				OtomodatiID = ((ghostScore == null) ? 0 : (ghostScore.IsNpc() ? ghostScore.UserID : 0)),
				OtomodatiVsWin = Singleton<GamePlayManager>.Instance.IsMapNpcWin(index)
			};
		}
	}
}
