using System.Collections.Generic;

namespace Manager.UserDatas
{
	public class UserRating
	{
		public const int RatingListMax = 35;

		public const int RatingNewListMax = 15;

		public const int NextRatingListMax = 10;

		public const int NextRatingNewListMax = 10;

		public int Rating;

		public List<UserRate> RatingList { get; set; } = new List<UserRate>();


		public List<UserRate> NewRatingList { get; set; } = new List<UserRate>();


		public List<UserRate> NextRatingList { get; set; } = new List<UserRate>();


		public List<UserRate> NextNewRatingList { get; set; } = new List<UserRate>();


		public UserUdemae Udemae { get; set; } = new UserUdemae();


		public UserRating()
		{
			Clear();
		}

		public void Clear()
		{
			Rating = 0;
			RatingList.Clear();
			NewRatingList.Clear();
			NextRatingList.Clear();
			NextNewRatingList.Clear();
			Udemae.Initialize();
		}

		public void UpdateScore(int musicid, int difficulty, uint achive, uint romVersion)
		{
			UserRate tmpRate = new UserRate(musicid, difficulty, achive, romVersion);
			if (tmpRate.OldFlag)
			{
				UpdateScore(RatingList, tmpRate, 35);
			}
			else
			{
				UpdateScore(NewRatingList, tmpRate, 15);
			}
		}

		private void UpdateScore(List<UserRate> targetList, UserRate tmpRate, int maxCount)
		{
			int num = targetList.IndexOf(tmpRate);
			if (0 <= num)
			{
				if (targetList[num] < tmpRate)
				{
					targetList[num] = tmpRate;
				}
			}
			else
			{
				bool flag = false;
				if (targetList.Count > 0 && targetList[targetList.Count - 1] < tmpRate)
				{
					for (int i = 0; i < targetList.Count; i++)
					{
						if (targetList[i] < tmpRate)
						{
							targetList.Insert(i, tmpRate);
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					targetList.Add(tmpRate);
				}
			}
			targetList.Sort();
			targetList.Reverse();
			if (targetList.Count > maxCount)
			{
				targetList.RemoveRange(maxCount, targetList.Count - maxCount);
			}
		}

		public bool Contains(int musicid, int difficulty)
		{
			if (!RatingList.Contains(new UserRate(musicid, difficulty, 0u, 0u)))
			{
				return NewRatingList.Contains(new UserRate(musicid, difficulty, 0u, 0u));
			}
			return true;
		}
	}
}
