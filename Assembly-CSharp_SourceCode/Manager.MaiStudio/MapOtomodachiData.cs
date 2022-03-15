using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class MapOtomodachiData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID BonusMusic { get; private set; }

		public int Rate { get; private set; }

		public int WinOdds { get; private set; }

		public int LoseOdds { get; private set; }

		public StringID Silhouette { get; private set; }

		public StringID Title { get; private set; }

		public string dataName { get; private set; }

		public MapOtomodachiData()
		{
			name = new StringID();
			BonusMusic = new StringID();
			Rate = 0;
			WinOdds = 0;
			LoseOdds = 0;
			Silhouette = new StringID();
			Title = new StringID();
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.MapOtomodachiData sz)
		{
			name = (StringID)sz.name;
			BonusMusic = (StringID)sz.BonusMusic;
			Rate = sz.Rate;
			WinOdds = sz.WinOdds;
			LoseOdds = sz.LoseOdds;
			Silhouette = (StringID)sz.Silhouette;
			Title = (StringID)sz.Title;
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
