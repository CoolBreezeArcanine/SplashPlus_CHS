using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CharaData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID color { get; private set; }

		public StringID genre { get; private set; }

		public bool isCopyright { get; private set; }

		public bool disable { get; private set; }

		public string imageFile { get; private set; }

		public string thumbnailName { get; private set; }

		public int priority { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.CharaData sz)
		{
			name = (StringID)sz.name;
			color = (StringID)sz.color;
			genre = (StringID)sz.genre;
			isCopyright = sz.isCopyright;
			disable = sz.disable;
			imageFile = sz.imageFile;
			thumbnailName = sz.thumbnailName;
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
