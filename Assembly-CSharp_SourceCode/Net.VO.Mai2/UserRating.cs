using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserRating
	{
		public int rating;

		public UserRate[] ratingList;

		public UserRate[] newRatingList;

		public UserRate[] nextRatingList;

		public UserRate[] nextNewRatingList;

		public UserUdemae udemae;
	}
}
