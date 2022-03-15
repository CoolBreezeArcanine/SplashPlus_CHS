using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserCard
	{
		public int cardId;

		public int cardTypeId;

		public int charaId;

		public int mapId;

		public long startDate;

		public long endDataDate;

		public UserCard()
		{
			cardId = 0;
			cardTypeId = 0;
			charaId = 0;
			mapId = 0;
			startDate = 0L;
			endDataDate = 0L;
		}
	}
}
