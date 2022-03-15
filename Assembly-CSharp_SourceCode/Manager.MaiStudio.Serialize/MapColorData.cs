using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapColorData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID ColorGroupId;

		public Color24 Color;

		public Color24 ColorDark;

		public string dataName;

		public MapColorData()
		{
			name = new StringID();
			ColorGroupId = new StringID();
			Color = new Color24();
			ColorDark = new Color24();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapColorData(MapColorData sz)
		{
			Manager.MaiStudio.MapColorData mapColorData = new Manager.MaiStudio.MapColorData();
			mapColorData.Init(sz);
			return mapColorData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			ColorGroupId.AddPath(parentPath);
			Color.AddPath(parentPath);
			ColorDark.AddPath(parentPath);
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
