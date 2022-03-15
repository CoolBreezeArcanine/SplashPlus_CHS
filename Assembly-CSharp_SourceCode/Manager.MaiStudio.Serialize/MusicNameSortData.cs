using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MusicNameSortData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public string startChara;

		public string endChara;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public MusicNameSortData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			startChara = "";
			endChara = "";
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MusicNameSortData(MusicNameSortData sz)
		{
			Manager.MaiStudio.MusicNameSortData musicNameSortData = new Manager.MaiStudio.MusicNameSortData();
			musicNameSortData.Init(sz);
			return musicNameSortData;
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
