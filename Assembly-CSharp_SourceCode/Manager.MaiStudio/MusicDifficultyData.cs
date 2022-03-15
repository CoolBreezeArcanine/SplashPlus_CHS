using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MusicDifficultyData : AccessorBase
	{
		public StringID name { get; private set; }

		public string genreName { get; private set; }

		public string genreNameTwoLine { get; private set; }

		public Color24 Color { get; private set; }

		public Color24 SubColor { get; private set; }

		public int priority { get; private set; }

		public bool disable { get; private set; }

		public string dataName { get; private set; }

		public MusicDifficultyData()
		{
			name = new StringID();
			genreName = "";
			genreNameTwoLine = "";
			Color = new Color24();
			SubColor = new Color24();
			priority = 0;
			disable = false;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MusicDifficultyData sz)
		{
			name = (StringID)sz.name;
			genreName = sz.genreName;
			genreNameTwoLine = sz.genreNameTwoLine;
			Color = (Color24)sz.Color;
			SubColor = (Color24)sz.SubColor;
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
