using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class PhotoFrameData : AccessorBase
	{
		public StringID name { get; private set; }

		public bool disable { get; private set; }

		public StringID eventName { get; private set; }

		public string imageFile { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

		public PhotoFrameData()
		{
			name = new StringID();
			disable = false;
			eventName = new StringID();
			imageFile = "";
			priority = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.PhotoFrameData sz)
		{
			name = (StringID)sz.name;
			disable = sz.disable;
			eventName = (StringID)sz.eventName;
			imageFile = sz.imageFile;
			priority = sz.priority;
			dataName = sz.dataName;
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
