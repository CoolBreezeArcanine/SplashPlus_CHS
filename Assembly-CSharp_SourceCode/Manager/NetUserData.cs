using System;
using System.Collections.Generic;
using System.Linq;
using Net.VO.Mai2;

namespace Manager
{
	public class NetUserData
	{
		public ulong UserId;

		public UserDetail Detail;

		public UserCard[] CardList;

		public UserCharacter[] CharacterList;

		public readonly Dictionary<ItemKind, UserItem[]> ItemLists;

		public readonly Dictionary<FavoriteKind, UserFavorite> FavoriteList;

		public UserMap[] MapList;

		public Net.VO.Mai2.UserLoginBonus[] LoginBonusList;

		public UserOption Option;

		public UserExtend Extend;

		public UserRating Rating;

		public UserMusicDetail[] MusicDetailList;

		public UserCourse[] CourseList;

		public UserCharge[] ChargeList;

		public UserActivity Activity;

		public UserScoreRanking[] ScoreRanking;

		public UserGhost[] GhostList;

		public UserRegion[] RegionList;

		public NetUserData()
		{
			ItemLists = new Dictionary<ItemKind, UserItem[]>();
			FavoriteList = new Dictionary<FavoriteKind, UserFavorite>();
			Clear();
		}

		public void Clear()
		{
			UserId = 0uL;
			CardList = Array.Empty<UserCard>();
			CharacterList = Array.Empty<UserCharacter>();
			ItemLists.Clear();
			FavoriteList.Clear();
			GhostList = Array.Empty<UserGhost>();
			MapList = Array.Empty<UserMap>();
			LoginBonusList = Array.Empty<Net.VO.Mai2.UserLoginBonus>();
			MusicDetailList = Array.Empty<UserMusicDetail>();
			CourseList = Array.Empty<UserCourse>();
			ChargeList = Array.Empty<UserCharge>();
			RegionList = Array.Empty<UserRegion>();
			ScoreRanking = Array.Empty<UserScoreRanking>();
			Detail = default(UserDetail);
			Option = default(UserOption);
			Extend = default(UserExtend);
			Rating = default(UserRating);
			Activity = default(UserActivity);
		}

		public UserItem[] ExportUserItems()
		{
			return ItemLists.SelectMany((KeyValuePair<ItemKind, UserItem[]> d) => d.Value).ToArray();
		}

		public UserFavorite[] ExportUserFavorites()
		{
			return FavoriteList.Select((KeyValuePair<FavoriteKind, UserFavorite> d) => d.Value).ToArray();
		}
	}
}
