using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public static class MusicLevelIDEnum
	{
		public static bool IsValid(this MusicLevelID self)
		{
			if (self >= MusicLevelID.None)
			{
				return self < MusicLevelID.End;
			}
			return false;
		}

		public static string GetName(this MusicLevelID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicLevel((int)self).name.str;
			}
			return "";
		}

		public static int GetValue(this MusicLevelID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicLevel((int)self).value;
			}
			return 0;
		}

		public static Color24 GetMainColor(this MusicLevelID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicLevel((int)self).Color;
			}
			return new Color24();
		}

		public static string GetFilePath(this MusicLevelID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicLevel((int)self).FileName;
			}
			return "";
		}

		public static string GetLevelNum(this MusicLevelID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicLevel((int)self).levelNum;
			}
			return "";
		}
	}
}
