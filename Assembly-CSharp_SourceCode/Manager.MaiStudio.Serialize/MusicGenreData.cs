using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicGenreData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicGenreData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicGenreData(MusicGenreData sz)
		{
			Manager.MaiStudio.MusicGenreData musicGenreData = new Manager.MaiStudio.MusicGenreData();
			musicGenreData.Init(sz);
			return musicGenreData;
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
