using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ScoreRankingData : SerializeBase, ISerialize
	{
		public StringID name;

		public bool isSpecial;

		public StringID eventName;

		public string eventText;

		public StringID netOpenName;

		public StringsCollection MusicIds;

		public string genreName;

		public string genreNameTwoLine;

		public Color24 Color;

		public string FileName;

		public string dataName;

		public ScoreRankingData()
		{
			name = new StringID();
			isSpecial = false;
			eventName = new StringID();
			eventText = "";
			netOpenName = new StringID();
			MusicIds = new StringsCollection();
			genreName = "";
			genreNameTwoLine = "";
			Color = new Color24();
			FileName = "";
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.ScoreRankingData(ScoreRankingData sz)
		{
			Manager.MaiStudio.ScoreRankingData scoreRankingData = new Manager.MaiStudio.ScoreRankingData();
			scoreRankingData.Init(sz);
			return scoreRankingData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			eventName.AddPath(parentPath);
			netOpenName.AddPath(parentPath);
			MusicIds.AddPath(parentPath);
			Color.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
