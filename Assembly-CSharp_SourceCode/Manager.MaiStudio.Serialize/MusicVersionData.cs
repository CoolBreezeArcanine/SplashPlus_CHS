using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicVersionData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public int version;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicVersionData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			version = 0;
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicVersionData(MusicVersionData sz)
		{
			Manager.MaiStudio.MusicVersionData musicVersionData = new Manager.MaiStudio.MusicVersionData();
			musicVersionData.Init(sz);
			return musicVersionData;
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
