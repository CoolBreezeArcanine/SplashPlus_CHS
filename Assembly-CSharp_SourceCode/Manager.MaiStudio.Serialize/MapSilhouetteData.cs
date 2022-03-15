using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class MapSilhouetteData : SerializeBase, ISerialize
	{
		public StringID name;

		public string fileName;

		public string dataName;

		public MapSilhouetteData()
		{
			name = new StringID();
			fileName = "";
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.MapSilhouetteData(MapSilhouetteData sz)
		{
			Manager.MaiStudio.MapSilhouetteData mapSilhouetteData = new Manager.MaiStudio.MapSilhouetteData();
			mapSilhouetteData.Init(sz);
			return mapSilhouetteData;
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
