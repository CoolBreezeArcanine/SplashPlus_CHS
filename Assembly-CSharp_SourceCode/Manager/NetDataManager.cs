using System;
using MAI2.Util;
using Net.VO.Mai2;

namespace Manager
{
	public class NetDataManager : Singleton<NetDataManager>
	{
		private readonly NetUserData[] _netUserData;

		private readonly UserLoginResponseVO[] _loginVOs;

		public NetDataManager()
		{
			_netUserData = new NetUserData[2]
			{
				new NetUserData(),
				new NetUserData()
			};
			_loginVOs = new UserLoginResponseVO[2];
		}

		public void Initialize()
		{
			NetUserData[] netUserData = _netUserData;
			for (int i = 0; i < netUserData.Length; i++)
			{
				netUserData[i].Clear();
			}
			for (int j = 0; j < _loginVOs.Length; j++)
			{
				_loginVOs[j] = null;
			}
		}

		public NetUserData GetNetUserData(int index)
		{
			if (index < 0 || index >= _netUserData.Length)
			{
				return null;
			}
			return _netUserData[index];
		}

		public UserCourse[] GetUserCourses(int index)
		{
			return ((UserCourse[])GetNetUserData(index)?.CourseList.Clone()) ?? Array.Empty<UserCourse>();
		}

		public UserCharge[] GetUserCharges(int index)
		{
			return ((UserCharge[])GetNetUserData(index)?.ChargeList.Clone()) ?? Array.Empty<UserCharge>();
		}

		public UserMap[] GetUserMaps(int index)
		{
			return ((UserMap[])GetNetUserData(index)?.MapList.Clone()) ?? Array.Empty<UserMap>();
		}

		public Net.VO.Mai2.UserLoginBonus[] GetUserLoginBonuses(int index)
		{
			return ((Net.VO.Mai2.UserLoginBonus[])GetNetUserData(index)?.LoginBonusList.Clone()) ?? Array.Empty<Net.VO.Mai2.UserLoginBonus>();
		}

		public UserCharacter[] GetUserCharacters(int index)
		{
			return ((UserCharacter[])GetNetUserData(index)?.CharacterList.Clone()) ?? Array.Empty<UserCharacter>();
		}

		public UserGhost[] GetUserGhosts(int index)
		{
			return ((UserGhost[])GetNetUserData(index)?.GhostList.Clone()) ?? Array.Empty<UserGhost>();
		}

		public UserRating GetUserRating(int index)
		{
			return GetNetUserData(index)?.Rating.Clone() ?? default(UserRating);
		}

		public UserActivity GetUserActivity(int index)
		{
			return GetNetUserData(index)?.Activity.Clone() ?? default(UserActivity);
		}

		public UserRegion[] GetUserRegions(int index)
		{
			return ((UserRegion[])GetNetUserData(index)?.RegionList.Clone()) ?? Array.Empty<UserRegion>();
		}

		public UserScoreRanking[] GetUserScoreRankings(int index)
		{
			return ((UserScoreRanking[])GetNetUserData(index)?.ScoreRanking.Clone()) ?? Array.Empty<UserScoreRanking>();
		}

		public void SetLoginVO(int index, UserLoginResponseVO vo)
		{
			_loginVOs[index] = (UserLoginResponseVO)vo.Clone();
		}

		public UserLoginResponseVO GetLoginVO(int index)
		{
			return _loginVOs[index];
		}
	}
}
