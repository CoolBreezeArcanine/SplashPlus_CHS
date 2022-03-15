namespace Monitor.MapResult
{
	public class BounusIconDispInfo
	{
		public enum MiniIconType
		{
			MiniIconChara,
			MiniIconMusic,
			MiniIconMax
		}

		public enum BigIconType
		{
			BigIconChara,
			BigIconCenter,
			BigIconEnd,
			BigIconStack,
			BigIconMax
		}

		public enum BonusKind
		{
			BonusKindChara,
			BonusKindCleaRank,
			BonusKindPlayBonusMusic,
			BonusKindOtomodachi,
			BonusKindTicketGrade,
			BonusKindMax
		}

		public enum RankKind
		{
			RankKindNone,
			RankKindFullSyncDX,
			RankKindFullSyncDXPlus,
			RankKindFullSync,
			RankKindFullSyncPlus,
			RankKindFullCombo,
			RankKindFullComboPlus,
			RankKindAllPerfect,
			RankKindAllPerfectPlus,
			RankKindSyncPlay
		}

		public int ID;

		public MiniIconType miniIconType;

		public BigIconType bigIconType;

		public BonusKind bonusKind;

		public uint point;

		public uint addPoint;

		public BounusIconDispInfo()
		{
			ID = 0;
			miniIconType = MiniIconType.MiniIconChara;
			bigIconType = BigIconType.BigIconChara;
			bonusKind = BonusKind.BonusKindChara;
			point = 0u;
			addPoint = 0u;
		}
	}
}
