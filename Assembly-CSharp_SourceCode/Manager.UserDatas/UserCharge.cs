using System;

namespace Manager.UserDatas
{
	[Serializable]
	public class UserCharge
	{
		public int chargeId;

		public int stock;

		public string purchaseDate;

		public string validDate;

		public UserCharge(int id, int stockNum, string purchase, string valid)
		{
			chargeId = id;
			stock = stockNum;
			purchaseDate = purchase;
			validDate = valid;
		}
	}
}
