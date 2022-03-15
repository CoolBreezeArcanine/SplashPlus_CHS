namespace Manager
{
	public class ProgJudgeData : TimingBase
	{
		public enum ProgType
		{
			Prog_Hold = 0,
			Prog_HoldEnd = 1,
			Prog_MAX = 2,
			Prog_Invalid = -1
		}

		public ProgType type;

		public bool enable;

		public bool isUsed;

		public ProgJudgeData(ProgJudgeData pj)
		{
			type = pj.type;
			enable = pj.enable;
			isUsed = pj.isUsed;
		}

		public ProgJudgeData()
		{
			clear();
		}

		public void clear()
		{
			type = ProgType.Prog_Invalid;
			enable = false;
			isUsed = false;
		}
	}
}
