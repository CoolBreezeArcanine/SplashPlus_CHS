namespace Note
{
	internal static class Static
	{
		public static JudgeTimingParam[] judgeTimingParam = new JudgeTimingParam[9]
		{
			new JudgeTimingParam(0, isFinish: false, Judge.Begin, Timing.Just),
			new JudgeTimingParam(3, isFinish: true, Judge.Begin, Timing.Begin),
			new JudgeTimingParam(5, isFinish: true, Judge.Good, Timing.Begin),
			new JudgeTimingParam(7, isFinish: true, Judge.Great, Timing.Begin),
			new JudgeTimingParam(8, isFinish: true, Judge.Perfect, Timing.Just),
			new JudgeTimingParam(6, isFinish: true, Judge.Great, Timing.Late),
			new JudgeTimingParam(4, isFinish: true, Judge.Good, Timing.Late),
			new JudgeTimingParam(2, isFinish: true, Judge.Begin, Timing.Late),
			new JudgeTimingParam(1, isFinish: true, Judge.Begin, Timing.Late)
		};

		public static int judgeTimingParamNum => judgeTimingParam.Length;
	}
}
