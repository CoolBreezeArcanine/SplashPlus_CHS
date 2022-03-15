namespace Manager.UserDatas
{
	public class UserRegion
	{
		public int RegionId;

		public int PlayCount;

		public string Created;

		public UserRegion()
		{
			Clear();
		}

		public void Clear()
		{
			RegionId = 0;
			PlayCount = 0;
			Created = "";
		}
	}
}
