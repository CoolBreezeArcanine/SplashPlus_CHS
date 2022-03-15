using System;

namespace Net.VO
{
	[Serializable]
	public class UpsertResponseVO : VOSerializer
	{
		public int returnCode;

		public string apiName;
	}
}
