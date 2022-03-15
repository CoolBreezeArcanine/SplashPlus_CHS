using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public static class MusicDifficultyIDEnum
	{
		public static bool IsValid(this MusicDifficultyID self)
		{
			if (self >= MusicDifficultyID.Basic)
			{
				return self < MusicDifficultyID.End;
			}
			return false;
		}

		public static string GetName(this MusicDifficultyID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicDifficulty((int)self).name.str;
			}
			return "";
		}

		public static Color24 GetMainColor(this MusicDifficultyID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicDifficulty((int)self).Color;
			}
			return new Color24();
		}

		public static Color24 GetSubColor(this MusicDifficultyID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicDifficulty((int)self).SubColor;
			}
			return new Color24();
		}
	}
}
