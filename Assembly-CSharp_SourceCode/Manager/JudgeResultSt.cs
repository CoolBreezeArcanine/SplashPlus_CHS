namespace Manager
{
	public struct JudgeResultSt
	{
		public NoteScore.EScoreType Type;

		public int Timing;

		public uint Score;

		public uint ScoreBonus;

		public uint TheoryScore;

		public uint TheoryScoreBonus;

		public bool Judged;

		public uint Deluxe;

		public bool IsBreak;

		public void UpdateScore(int monitorIndex, NoteScore.EScoreType type, NoteJudge.ETiming timing)
		{
			Judged = true;
			Type = type;
			Timing = (int)timing;
			switch (Type)
			{
			case NoteScore.EScoreType.Tap:
			case NoteScore.EScoreType.Touch:
				IsBreak = false;
				Score = NoteScore.GetJudgeScore(timing);
				ScoreBonus = 0u;
				TheoryScoreBonus = 0u;
				TheoryScore = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical);
				break;
			case NoteScore.EScoreType.Hold:
				IsBreak = false;
				Score = NoteScore.GetJudgeScore(timing, NoteScore.EScoreType.Hold);
				ScoreBonus = 0u;
				TheoryScoreBonus = 0u;
				TheoryScore = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Hold);
				break;
			case NoteScore.EScoreType.Slide:
				IsBreak = false;
				Score = NoteScore.GetJudgeScore(timing, NoteScore.EScoreType.Slide);
				ScoreBonus = 0u;
				TheoryScoreBonus = 0u;
				TheoryScore = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Slide);
				break;
			case NoteScore.EScoreType.Break:
				IsBreak = true;
				Score = NoteScore.GetJudgeScore(timing, NoteScore.EScoreType.Break);
				ScoreBonus = NoteScore.GetJudgeScore(timing, NoteScore.EScoreType.BreakBonus);
				TheoryScore = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.Break);
				TheoryScoreBonus = NoteScore.GetJudgeScore(NoteJudge.ETiming.Critical, NoteScore.EScoreType.BreakBonus);
				break;
			}
			switch (NoteJudge.ConvertJudge(timing))
			{
			default:
				Deluxe = 0u;
				break;
			case NoteJudge.JudgeBox.Great:
				Deluxe = 1u;
				break;
			case NoteJudge.JudgeBox.Perfect:
				Deluxe = 2u;
				break;
			case NoteJudge.JudgeBox.Critical:
				Deluxe = 3u;
				break;
			}
		}

		public static bool operator ==(JudgeResultSt a, JudgeResultSt b)
		{
			if (!a.Judged)
			{
				return false;
			}
			if (a.Timing == b.Timing && a.Timing != 0 && a.Timing != 14)
			{
				return true;
			}
			return false;
		}

		public static bool operator !=(JudgeResultSt a, JudgeResultSt b)
		{
			return !(a == b);
		}

		public override bool Equals(object o)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
