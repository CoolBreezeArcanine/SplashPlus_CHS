using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ScoreRankingData : AccessorBase
	{
		public StringID name { get; private set; }

		public bool isSpecial { get; private set; }

		public StringID eventName { get; private set; }

		public string eventText { get; private set; }

		public StringID netOpenName { get; private set; }

		public StringsCollection MusicIds { get; private set; }

		public string genreName { get; private set; }

		public string genreNameTwoLine { get; private set; }

		public Color24 Color { get; private set; }

		public string FileName { get; private set; }

		public string dataName { get; private set; }

		public ScoreRankingData()
		{
			name = new StringID();
			isSpecial = false;
			eventName = new StringID();
			eventText = "";
			netOpenName = new StringID();
			MusicIds = new StringsCollection();
			genreName = "";
			genreNameTwoLine = "";
			Color = new Color24();
			FileName = "";
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.ScoreRankingData sz)
		{
			name = (StringID)sz.name;
			isSpecial = sz.isSpecial;
			eventName = (StringID)sz.eventName;
			eventText = sz.eventText;
			netOpenName = (StringID)sz.netOpenName;
			MusicIds = (StringsCollection)sz.MusicIds;
			genreName = sz.genreName;
			genreNameTwoLine = sz.genreNameTwoLine;
			Color = (Color24)sz.Color;
			FileName = sz.FileName;
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
