using System;

namespace Manager
{
	public class TimeManager
	{
		public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.f";

		public static readonly TimeSpan JpTime = new TimeSpan(9, 0, 0);

		public static long PlayBaseTime { get; private set; }

		public static DateTime GetNowTime()
		{
			return DateTime.UtcNow;
		}

		public static long GetNowUnixTime()
		{
			return ((DateTimeOffset)GetNowTime()).ToUnixTimeSeconds();
		}

		public static void MarkGameStartTime()
		{
			PlayBaseTime = GetNowUnixTime();
		}

		public static void Reset()
		{
			PlayBaseTime = 0L;
		}

		public static DateTime GetDateTime(long unixTime)
		{
			try
			{
				return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime.Add(JpTime);
			}
			catch (Exception)
			{
				return DateTime.MinValue;
			}
		}

		public static DateTimeOffset GetDateTimeOffset(long unixTime)
		{
			return new DateTimeOffset(GetDateTime(unixTime), JpTime);
		}

		public static long GetUnixTime(string timeJp)
		{
			try
			{
				return new DateTimeOffset(DateTime.Parse(timeJp), JpTime).ToUnixTimeSeconds();
			}
			catch (Exception)
			{
				return 0L;
			}
		}

		public static string GetNowDateString()
		{
			return GetDateString(GetNowUnixTime());
		}

		public static string GetDateString(long unixTime)
		{
			return GetDateTime(unixTime).ToString("yyyy-MM-dd HH:mm:ss.f");
		}

		public static string GetLogDateString(long unixTime)
		{
			return ToLogDateTime(GetDateTime(unixTime)).ToString("yyyy-MM-dd");
		}

		public static DateTime ToLogDateTime(DateTime dateTime, int hour = 0)
		{
			DateTime result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0);
			if (7 > dateTime.Hour)
			{
				return result.AddDays(-1.0);
			}
			return result;
		}

		public static bool IsAnotherLogDay(long unix1, long unix2)
		{
			return ToLogDateTime(GetDateTime(unix1).Date) != ToLogDateTime(GetDateTime(unix2).Date);
		}
	}
}
