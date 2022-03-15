using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MusicLevelData : AccessorBase
	{
		public StringID name { get; private set; }

		public string genreName { get; private set; }

		public string genreNameTwoLine { get; private set; }

		public int value { get; private set; }

		public string levelNum { get; private set; }

		public Color24 Color { get; private set; }

		public string FileName { get; private set; }

		public int priority { get; private set; }

		public bool disable { get; private set; }

		public string dataName { get; private set; }

		public MusicLevelData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			value = 0;
			levelNum = "";
			Color = new Color24();
			FileName = "";
			priority = 0;
			disable = false;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MusicLevelData sz)
		{
			name = (StringID)sz.name;
			genreName = sz.genreName;
			genreNameTwoLine = sz.genreNameTwoLine;
			value = sz.value;
			levelNum = sz.levelNum;
			Color = (Color24)sz.Color;
			FileName = sz.FileName;
			priority = sz.priority;
			disable = sz.disable;
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
