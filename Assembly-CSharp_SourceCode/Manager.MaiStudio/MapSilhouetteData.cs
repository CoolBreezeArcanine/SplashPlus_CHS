using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapSilhouetteData : AccessorBase
	{
		public StringID name { get; private set; }

		public string fileName { get; private set; }

		public string dataName { get; private set; }

		public MapSilhouetteData()
		{
			name = new StringID();
			fileName = "";
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapSilhouetteData sz)
		{
			name = (StringID)sz.name;
			fileName = sz.fileName;
			dataName = sz.dataName;
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
