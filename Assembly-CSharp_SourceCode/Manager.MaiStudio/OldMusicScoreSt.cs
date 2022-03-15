using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class OldMusicScoreSt : AccessorBase
	{
		public int TheoryScore { get; private set; }

		public int Score { get; private set; }

		public OldMusicScoreSt()
		{
			TheoryScore = 0;
			Score = 0;
		}

		public void Init(Manager.MaiStudio.Serialize.OldMusicScoreSt sz)
		{
			TheoryScore = sz.TheoryScore;
			Score = sz.Score;
		}
	}
}
