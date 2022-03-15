using MAI2.Util;

namespace Manager
{
	public static class MusicClearrankIDEnum
	{
		public static bool IsValid(this MusicClearrankID self)
		{
			if (self >= MusicClearrankID.Rank_D)
			{
				return self < MusicClearrankID.End;
			}
			return false;
		}

		public static string GetName(this MusicClearrankID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicClearRank((int)self).genreName;
			}
			return "";
		}

		public static int GetAchvement(this MusicClearrankID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicClearRank((int)self).achieve;
			}
			return 0;
		}
	}
}
