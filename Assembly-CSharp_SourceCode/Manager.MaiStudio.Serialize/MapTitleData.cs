using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapTitleData : SerializeBase, ISerialize
	{
		public StringID name;

		public string dataName;

		public MapTitleData()
		{
			name = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapTitleData(MapTitleData sz)
		{
			Manager.MaiStudio.MapTitleData mapTitleData = new Manager.MaiStudio.MapTitleData();
			mapTitleData.Init(sz);
			return mapTitleData;
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
