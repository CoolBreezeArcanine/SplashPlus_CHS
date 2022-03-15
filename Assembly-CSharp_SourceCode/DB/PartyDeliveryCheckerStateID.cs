namespace DB
{
	public enum PartyDeliveryCheckerStateID
	{
		First = 0,
		ServerCheck = 1,
		ServerActive = 2,
		ClientCheck = 3,
		ClientActive = 4,
		Error = 5,
		Finish = 6,
		Begin = 0,
		End = 7,
		Invalid = -1
	}
}
