using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class UserAll
	{
		public UserDetail[] userData;

		public UserExtend[] userExtend;

		public UserOption[] userOption;

		public UserCharacter[] userCharacterList;

		public UserGhost[] userGhost;

		public UserMap[] userMapList;

		public UserLoginBonus[] userLoginBonusList;

		public UserRating[] userRatingList;

		public UserItem[] userItemList;

		public UserMusicDetail[] userMusicDetailList;

		public UserCourse[] userCourseList;

		public UserCharge[] userChargeList;

		public UserFavorite[] userFavoriteList;

		public UserActivity[] userActivityList;

		public UserGamePlaylog[] userGamePlaylogList;

		public string isNewCharacterList;

		public string isNewMapList;

		public string isNewLoginBonusList;

		public string isNewItemList;

		public string isNewMusicDetailList;

		public string isNewCourseList;

		public string isNewFavoriteList;
	}
}
