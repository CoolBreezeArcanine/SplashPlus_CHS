using DB;

namespace Manager.UserDatas
{
	public class UserSort
	{
		private SortTabID sortTab;

		private SortMusicID sortMusic;

		public SortTabID SortTab
		{
			get
			{
				return sortTab;
			}
			set
			{
				sortTab = value;
			}
		}

		public SortMusicID SortMusic
		{
			get
			{
				return sortMusic;
			}
			set
			{
				sortMusic = value;
			}
		}

		public void Initialize()
		{
			SortTab = SortTabID.Genre;
			SortMusic = SortMusicID.ID;
		}
	}
}
