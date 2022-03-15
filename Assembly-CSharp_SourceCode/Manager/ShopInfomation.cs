namespace Manager
{
	public class ShopInfomation
	{
		public string ShopName;

		public string ShopNickName;

		public uint LocationId;

		public int RegionCode;

		public string CountryCode;

		public ShopInfomation()
		{
			ShopName = string.Empty;
			ShopNickName = string.Empty;
			LocationId = 0u;
			RegionCode = 0;
			CountryCode = string.Empty;
		}
	}
}
