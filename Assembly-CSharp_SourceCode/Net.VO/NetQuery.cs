namespace Net.VO
{
	public class NetQuery<T0, T1> : INetQuery where T0 : VOSerializer, new()where T1 : VOSerializer, new()
	{
		public string Api { get; }

		public ulong UserId { get; }

		public T0 Request { get; }

		public T1 Response { get; }

		public NetQuery(string api, ulong userId = 0uL)
		{
			Api = api;
			UserId = userId;
			Request = new T0();
			Response = new T1();
		}

		public string GetRequest()
		{
			return Request.Serialize();
		}

		public bool SetResponse(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			Response.Deserialize(str);
			return true;
		}
	}
}
