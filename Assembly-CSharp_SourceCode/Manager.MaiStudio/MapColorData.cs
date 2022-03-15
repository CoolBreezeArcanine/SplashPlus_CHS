using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapColorData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID ColorGroupId { get; private set; }

		public Color24 Color { get; private set; }

		public Color24 ColorDark { get; private set; }

		public string dataName { get; private set; }

		public MapColorData()
		{
			name = new StringID();
			ColorGroupId = new StringID();
			Color = new Color24();
			ColorDark = new Color24();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapColorData sz)
		{
			name = (StringID)sz.name;
			ColorGroupId = (StringID)sz.ColorGroupId;
			Color = (Color24)sz.Color;
			ColorDark = (Color24)sz.ColorDark;
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
