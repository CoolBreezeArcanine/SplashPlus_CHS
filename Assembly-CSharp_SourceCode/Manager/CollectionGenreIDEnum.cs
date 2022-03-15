using MAI2.Util;
using Manager.MaiStudio;

namespace Manager
{
	public static class CollectionGenreIDEnum
	{
		public static bool IsValid(this CollectionGenreID self)
		{
			if (self >= CollectionGenreID.Title)
			{
				return self < CollectionGenreID.End;
			}
			return false;
		}

		public static string GetName(this CollectionGenreID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetCollectionnType((int)self).name.str;
			}
			return "";
		}

		public static string GetGenreName(this CollectionGenreID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetCollectionnType((int)self).genreName;
			}
			return "";
		}

		public static Color24 GetMainColor(this CollectionGenreID self)
		{
			if (self.IsValid())
			{
				return Singleton<DataManager>.Instance.GetCollectionnType((int)self).Color;
			}
			return new Color24();
		}
	}
}
