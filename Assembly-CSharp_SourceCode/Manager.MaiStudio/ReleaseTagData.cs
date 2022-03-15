using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ReleaseTagData : AccessorBase
	{
		public StringID name { get; private set; }

		public string dataName { get; private set; }

		public ReleaseTagData()
		{
			name = new StringID();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.ReleaseTagData sz)
		{
			name = (StringID)sz.name;
			dataName = sz.dataName;
		}
	}
}
