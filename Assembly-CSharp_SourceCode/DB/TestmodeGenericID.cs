namespace DB
{
	public enum TestmodeGenericID
	{
		OpTypeSelect = 0,
		OpTypeTextExit = 1,
		OpTypeTestServiceExit = 2,
		OpTypeTextContinue = 3,
		OpTypeServiceAbort = 4,
		Begin = 0,
		End = 5,
		Invalid = -1
	}
}
