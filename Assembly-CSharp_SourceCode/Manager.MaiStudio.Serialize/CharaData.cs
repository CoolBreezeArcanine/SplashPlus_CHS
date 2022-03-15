using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CharaData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID color;

		public StringID genre;

		public bool isCopyright;

		public bool disable;

		public string imageFile;

		public string thumbnailName;

		public int priority;

		public string dataName;

		public CharaData()
		{
			name = new StringID();
			color = new StringID();
			genre = new StringID();
			isCopyright = false;
			disable = false;
			imageFile = "";
			thumbnailName = "";
			priority = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CharaData(CharaData sz)
		{
			Manager.MaiStudio.CharaData charaData = new Manager.MaiStudio.CharaData();
			charaData.Init(sz);
			return charaData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			color.AddPath(parentPath);
			genre.AddPath(parentPath);
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
