namespace Net
{
	public interface INetQuery
	{
		string Api { get; }

		ulong UserId { get; }

		string GetRequest();

		bool SetResponse(string str);
	}
}
