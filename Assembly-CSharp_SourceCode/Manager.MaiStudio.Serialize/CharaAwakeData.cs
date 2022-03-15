using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CharaAwakeData : SerializeBase, ISerialize
	{
		public StringID name;

		public int awakeLevel;

		public string dataName;

		public CharaAwakeData()
		{
			name = new StringID();
			awakeLevel = 0;
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CharaAwakeData(CharaAwakeData sz)
		{
			Manager.MaiStudio.CharaAwakeData charaAwakeData = new Manager.MaiStudio.CharaAwakeData();
			charaAwakeData.Init(sz);
			return charaAwakeData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
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
