namespace Manager
{
	public struct RivalData
	{
		public long RivalID;

		public string RivalName;

		public int Difficuty;

		public RivalData(int rivalID, string name, int difficulty)
		{
			RivalID = rivalID;
			RivalName = name;
			Difficuty = difficulty;
		}
	}
}
