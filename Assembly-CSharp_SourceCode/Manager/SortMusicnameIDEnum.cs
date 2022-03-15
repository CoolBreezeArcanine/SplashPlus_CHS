using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public static class SortMusicnameIDEnum
	{
		public static bool IsValid(this SortMusicnameID self)
		{
			if (self >= SortMusicnameID.H_A_O)
			{
				return self < SortMusicnameID.End;
			}
			return false;
		}

		public static string GetName(this SortMusicnameID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicNameSort((int)self).name.str;
			}
			return "";
		}

		public static string GetStartChar(this SortMusicnameID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicNameSort((int)self).startChara;
			}
			return "";
		}

		public static string GetEndChar(this SortMusicnameID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicNameSort((int)self).endChara;
			}
			return "";
		}

		public static Color24 GetMainColor(this SortMusicnameID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicNameSort((int)self).Color;
			}
			return new Color24();
		}

		public static string GetFilePath(this SortMusicnameID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetMusicNameSort((int)self).FileName;
			}
			return "";
		}
	}
}
