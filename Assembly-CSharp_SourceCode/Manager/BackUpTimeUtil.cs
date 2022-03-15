using System;

namespace Manager
{
	public static class BackUpTimeUtil
	{
		public const int DailyChangeHour = 7;

		public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.f";

		public const string LogDateFormat = "yyyy-MM-dd";

		public static readonly DateTime PosixOriginDateTime = new DateTime(1970, 1, 1, 9, 0, 0);

		public static string correct(string str)
		{
			if (str != null)
			{
				return str;
			}
			return string.Empty;
		}

		public static DateTime toDateTime(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return DateTime.MinValue;
			}
			DateTime result = default(DateTime);
			DateTime.TryParse(str, out result);
			return result;
		}

		public static string toString(DateTime dateTime)
		{
			if (DateTime.MinValue == dateTime)
			{
				return DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss.f");
			}
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss.f");
		}

		public static DateTime toLogDateTime(DateTime dateTime, int hour = 0)
		{
			DateTime result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0);
			if (7 > dateTime.Hour)
			{
				return result.AddDays(-1.0);
			}
			return result;
		}

		public static string toLogDateString(DateTime dateTime)
		{
			return toLogDateTime(dateTime).ToString("yyyy-MM-dd");
		}

		public static TimeSpan toPosixDateTime(DateTime dateTime)
		{
			return dateTime - PosixOriginDateTime;
		}

		public static int toSortNumber(DateTime loginDateTime)
		{
			return (int)(toPosixDateTime(loginDateTime).Ticks / 10000000);
		}
	}
}
