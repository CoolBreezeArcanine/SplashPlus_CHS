namespace Note
{
	internal class JudgeTimingParam
	{
		public int priority;

		public bool isFinish;

		public Judge result;

		public Timing timing;

		public JudgeTimingParam(int priority, bool isFinish, Judge result, Timing timing)
		{
			this.priority = priority;
			this.isFinish = isFinish;
			this.result = result;
			this.timing = timing;
		}
	}
}
