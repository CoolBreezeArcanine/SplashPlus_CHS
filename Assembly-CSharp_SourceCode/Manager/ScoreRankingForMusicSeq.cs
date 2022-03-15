using System.Collections.Generic;
using Manager.MaiStudio;

namespace Manager
{
	public class ScoreRankingForMusicSeq
	{
		public int Id;

		public string FileName;

		public Color24 GenreColor;

		public string GenreName;

		public List<ScoreRankingMusicInfo> MusicInfoList;

		public ScoreRankingForMusicSeq(int id)
		{
			Id = id;
			FileName = "";
			GenreColor = new Color24();
			GenreName = "";
			MusicInfoList = new List<ScoreRankingMusicInfo>();
		}
	}
}
