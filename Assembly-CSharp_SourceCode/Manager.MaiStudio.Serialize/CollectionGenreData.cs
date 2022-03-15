using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CollectionGenreData : SerializeBase, ISerialize
	{
		public StringID name;

		public string genreName;

		public string genreNameTwoLine;

		public Color24 Color;

		public string FileName;

		public int priority;

		public bool disable;

		public string dataName;

		public CollectionGenreData()
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

		public static explicit operator Manager.MaiStudio.CollectionGenreData(CollectionGenreData sz)
		{
			Manager.MaiStudio.CollectionGenreData collectionGenreData = new Manager.MaiStudio.CollectionGenreData();
			collectionGenreData.Init(sz);
			return collectionGenreData;
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
