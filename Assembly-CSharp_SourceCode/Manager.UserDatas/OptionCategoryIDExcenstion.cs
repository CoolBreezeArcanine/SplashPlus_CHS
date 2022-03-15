using DB;

namespace Manager.UserDatas
{
	internal static class OptionCategoryIDExcenstion
	{
		public static int GetCategoryMax(this OptionCategoryID category)
		{
			return category switch
			{
				OptionCategoryID.SpeedSetting => 3, 
				OptionCategoryID.DesignSetting => 5, 
				OptionCategoryID.GameSetting => 7, 
				OptionCategoryID.JudgeSetting => 8, 
				OptionCategoryID.SoundSetting => 11, 
				_ => 0, 
			};
		}
	}
}
