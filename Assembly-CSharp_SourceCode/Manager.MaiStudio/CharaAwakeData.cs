using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CharaAwakeData : AccessorBase
	{
		public StringID name { get; private set; }

		public int awakeLevel { get; private set; }

		public string dataName { get; private set; }

		public CharaAwakeData()
		{
			name = new StringID();
			awakeLevel = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.CharaAwakeData sz)
		{
			name = (StringID)sz.name;
			awakeLevel = sz.awakeLevel;
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
