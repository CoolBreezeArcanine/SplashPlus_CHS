namespace Net
{
	public static class NetConfig
	{
		public const int DefaultMaxRetry = 3;

		public static int TimeOutInMSec { get; set; } = 30000;


		public static int MaxRetry { get; set; } = 3;


		public static float RetryWaitInSec { get; set; } = 3f;


		public static string ClientId { get; set; } = string.Empty;

	}
}
