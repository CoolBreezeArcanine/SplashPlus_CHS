using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class OldMusicScoreSt : SerializeBase
	{
		public int TheoryScore;

		public int Score;

		public OldMusicScoreSt()
		{
			TheoryScore = 0;
			Score = 0;
		}

		public static explicit operator Manager.MaiStudio.OldMusicScoreSt(OldMusicScoreSt sz)
		{
			Manager.MaiStudio.OldMusicScoreSt oldMusicScoreSt = new Manager.MaiStudio.OldMusicScoreSt();
			oldMusicScoreSt.Init(sz);
			return oldMusicScoreSt;
		}

		public override void AddPath(string parentPath)
		{
		}
	}
}
