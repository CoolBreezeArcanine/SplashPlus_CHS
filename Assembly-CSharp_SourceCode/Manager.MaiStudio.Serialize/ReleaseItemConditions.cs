using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ReleaseItemConditions : SerializeBase
	{
		public ReleaseConditionTrackKind kindTrack;

		public ReleaseConditionCreditKind kindCredit;

		public ReleaseConditionTotalKind kindTotal;

		public StringID titleId;

		public StringID plateId;

		public StringID iconId;

		public StringID charaId;

		public StringID mapId;

		public StringID musicId;

		public StringID musicGroupId;

		public StringID otomodachiId;

		public HiSpeedTapTouchID hiSpeedTapTouchId;

		public HiSpeedSlideID hiSpeedSlideId;

		public GradeID gradeId;

		public RankID rankId;

		public SatelliteID satelliteId;

		public DifficultyID difficultyId;

		public TodohukenID todohukenId;

		public int param;

		public FcID fcId;

		public SyncID syncId;

		public ReleaseItemConditions()
		{
			kindTrack = ReleaseConditionTrackKind.None;
			kindCredit = ReleaseConditionCreditKind.None;
			kindTotal = ReleaseConditionTotalKind.None;
			titleId = new StringID();
			plateId = new StringID();
			iconId = new StringID();
			charaId = new StringID();
			mapId = new StringID();
			musicId = new StringID();
			musicGroupId = new StringID();
			otomodachiId = new StringID();
			hiSpeedTapTouchId = HiSpeedTapTouchID.SPEED_10;
			hiSpeedSlideId = HiSpeedSlideID.SPEED_m10;
			gradeId = GradeID.SHOSINSHA;
			rankId = RankID.D;
			satelliteId = SatelliteID.LEFT;
			difficultyId = DifficultyID.ANY;
			todohukenId = TodohukenID.AICHI;
			param = 0;
			fcId = FcID.None;
			syncId = SyncID.None;
		}

		public static explicit operator Manager.MaiStudio.ReleaseItemConditions(ReleaseItemConditions sz)
		{
			Manager.MaiStudio.ReleaseItemConditions releaseItemConditions = new Manager.MaiStudio.ReleaseItemConditions();
			releaseItemConditions.Init(sz);
			return releaseItemConditions;
		}

		public override void AddPath(string parentPath)
		{
			titleId.AddPath(parentPath);
			plateId.AddPath(parentPath);
			iconId.AddPath(parentPath);
			charaId.AddPath(parentPath);
			mapId.AddPath(parentPath);
			musicId.AddPath(parentPath);
			musicGroupId.AddPath(parentPath);
			otomodachiId.AddPath(parentPath);
		}
	}
}
