using System;
using Manager.MaiStudio;

namespace Manager
{
	public struct OpenEventData : IComparable
	{
		private const long NewDays = 28L;

		public int Id;

		public bool IsNew;

		public bool IsEndless;

		public EventInfoType EventType;

		public long StartUnixTime;

		public long EndUnixTime;

		public OpenEventData(int id)
		{
			Id = id;
			IsNew = false;
			IsEndless = false;
			StartUnixTime = 0L;
			EndUnixTime = 0L;
			EventType = EventInfoType.Normal;
		}

		public static bool operator ==(OpenEventData a, OpenEventData b)
		{
			return a.Id == b.Id;
		}

		public static bool operator !=(OpenEventData a, OpenEventData b)
		{
			return a.Id != b.Id;
		}

		public override bool Equals(object obj)
		{
			OpenEventData openEventData = this;
			OpenEventData openEventData2 = (OpenEventData)obj;
			return openEventData == openEventData2;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public int CompareTo(OpenEventData other)
		{
			return CompareTo(other);
		}

		public int CompareTo(object obj)
		{
			OpenEventData openEventData = this;
			OpenEventData openEventData2 = (OpenEventData)obj;
			return openEventData.Id.CompareTo(openEventData2.Id);
		}
	}
}
