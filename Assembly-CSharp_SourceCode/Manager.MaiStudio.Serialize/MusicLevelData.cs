using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicLevelData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public int value;

		public string levelNum;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicLevelData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			value = 0;
			levelNum = "";
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicLevelData(MusicLevelData sz)
		{
			Manager.MaiStudio.MusicLevelData musicLevelData = new Manager.MaiStudio.MusicLevelData();
			musicLevelData.Init(sz);
			return musicLevelData;
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
