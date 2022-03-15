using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ReleaseItemConditions : AccessorBase
	{
		public ReleaseConditionTrackKind kindTrack { get; private set; }

		public ReleaseConditionCreditKind kindCredit { get; private set; }

		public ReleaseConditionTotalKind kindTotal { get; private set; }

		public StringID titleId { get; private set; }

		public StringID plateId { get; private set; }

		public StringID iconId { get; private set; }

		public StringID charaId { get; private set; }

		public StringID mapId { get; private set; }

		public StringID musicId { get; private set; }

		public StringID musicGroupId { get; private set; }

		public StringID otomodachiId { get; private set; }

		public HiSpeedTapTouchID hiSpeedTapTouchId { get; private set; }

		public HiSpeedSlideID hiSpeedSlideId { get; private set; }

		public GradeID gradeId { get; private set; }

		public RankID rankId { get; private set; }

		public SatelliteID satelliteId { get; private set; }

		public DifficultyID difficultyId { get; private set; }

		public TodohukenID todohukenId { get; private set; }

		public int param { get; private set; }

		public FcID fcId { get; private set; }

		public SyncID syncId { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.ReleaseItemConditions sz)
		{
			kindTrack = sz.kindTrack;
			kindCredit = sz.kindCredit;
			kindTotal = sz.kindTotal;
			titleId = (StringID)sz.titleId;
			plateId = (StringID)sz.plateId;
			iconId = (StringID)sz.iconId;
			charaId = (StringID)sz.charaId;
			mapId = (StringID)sz.mapId;
			musicId = (StringID)sz.musicId;
			musicGroupId = (StringID)sz.musicGroupId;
			otomodachiId = (StringID)sz.otomodachiId;
			hiSpeedTapTouchId = sz.hiSpeedTapTouchId;
			hiSpeedSlideId = sz.hiSpeedSlideId;
			gradeId = sz.gradeId;
			rankId = sz.rankId;
			satelliteId = sz.satelliteId;
			difficultyId = sz.difficultyId;
			todohukenId = sz.todohukenId;
			param = sz.param;
			fcId = sz.fcId;
			syncId = sz.syncId;
		}
	}
}
