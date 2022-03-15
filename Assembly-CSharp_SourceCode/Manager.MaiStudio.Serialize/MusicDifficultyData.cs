using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicDifficultyData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public Color24 Color;

		public Color24 SubColor;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicDifficultyData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			Color = new Color24();
			SubColor = new Color24();
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicDifficultyData(MusicDifficultyData sz)
		{
			Manager.MaiStudio.MusicDifficultyData musicDifficultyData = new Manager.MaiStudio.MusicDifficultyData();
			musicDifficultyData.Init(sz);
			return musicDifficultyData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			Color.AddPath(parentPath);
			SubColor.AddPath(parentPath);
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
