using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserItem
	{
		public int itemId;

		public int stock;

		public bool isValid;

		public UserItem()
		{
			itemId = 0;
			stock = 0;
			isValid = false;
		}

		public UserItem(int id)
		{
			itemId = id;
			stock = 1;
			isValid = true;
		}
	}
}
