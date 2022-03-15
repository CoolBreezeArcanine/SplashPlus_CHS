using System.Diagnostics;

public static class Console
{
	[Conditional("APP_DEBUG")]
	public static void Initialize()
	{
	}

	[Conditional("APP_DEBUG")]
	public static void Terminate()
	{
	}

	[Conditional("APP_DEBUG")]
	public static void Log(string msg)
	{
	}

	[Conditional("APP_DEBUG")]
	public static void LogFormat(string msg, params object[] args)
	{
	}
}
