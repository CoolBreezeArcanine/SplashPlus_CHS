using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicClearRankData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public int achieve;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicClearRankData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			achieve = 0;
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicClearRankData(MusicClearRankData sz)
		{
			Manager.MaiStudio.MusicClearRankData musicClearRankData = new Manager.MaiStudio.MusicClearRankData();
			musicClearRankData.Init(sz);
			return musicClearRankData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			Color.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
			priority = pri;
		}

		public bool IsDisable()
		{
			return disable;
		}
	}
}
