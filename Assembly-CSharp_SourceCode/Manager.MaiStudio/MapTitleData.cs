using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapTitleData : AccessorBase
	{
		public StringID name { get; private set; }

		public string dataName { get; private set; }

		public MapTitleData()
		{
			name = new StringID();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapTitleData sz)
		{
			name = (StringID)sz.name;
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
