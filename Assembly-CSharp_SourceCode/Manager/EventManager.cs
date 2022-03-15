using System.Collections.Generic;
using MAI2.Util;
using Manager.MaiStudio;
using Net.VO.Mai2;
using Util;

namespace Manager
{
	public class EventManager : Singleton<EventManager>
	{
		private List<OpenEventData> eventData = new List<OpenEventData>();

		private const long NewEventDays = 2419200L;

		public void UpdateEvent()
		{
			TimeManager.GetNowUnixTime();
			this.eventData.Clear();
			this.eventData.Add(new OpenEventData(1)
			{
				StartUnixTime = 0L,
				EndUnixTime = -1L,
				IsNew = false,
				IsEndless = true,
				EventType = EventInfoType.Normal
			});
			Safe.ReadonlySortedDictionary<int, EventData> events = Singleton<DataManager>.Instance.GetEvents();
			GameEvent[] eventDataList = Singleton<OperationManager>.Instance.GetEventDataList();
			for (int i = 0; i < eventDataList.Length; i++)
			{
				GameEvent gameEvent = eventDataList[i];
				long unixTime = TimeManager.GetUnixTime(gameEvent.startDate);
				long unixTime2 = TimeManager.GetUnixTime(gameEvent.endDate);
				try
				{
					EventData eventData = events[gameEvent.id];
					if (unixTime < TimeManager.PlayBaseTime && TimeManager.PlayBaseTime < unixTime2)
					{
						this.eventData.Add(new OpenEventData(gameEvent.id)
						{
							StartUnixTime = unixTime,
							EndUnixTime = unixTime2,
							IsNew = (unixTime + 2419200 > TimeManager.PlayBaseTime),
							IsEndless = eventData.alwaysOpen,
							EventType = eventData.infoType
						});
					}
				}
				catch
				{
				}
			}
			this.eventData.Sort();
		}

		public bool IsOpenEvent(int eventId)
		{
			return eventData.BinarySearch(new OpenEventData(eventId)) >= 0;
		}

		public bool IsNewEvent(int eventId)
		{
			int num = eventData.BinarySearch(new OpenEventData(eventId));
			if (num >= 0)
			{
				return eventData[num].IsNew;
			}
			return false;
		}

		public OpenEventData GetEventData(int eventId)
		{
			int num = eventData.BinarySearch(new OpenEventData(eventId));
			if (num >= 0)
			{
				return eventData[num];
			}
			return default(OpenEventData);
		}

		public long GetEventStartUnixTime(int eventId)
		{
			return GetEventData(eventId).StartUnixTime;
		}

		public long GetEventEndUnixTime(int eventId)
		{
			return GetEventData(eventId).EndUnixTime;
		}

		public bool IsDateNewAndOpen(int eventId, long time)
		{
			int num = eventData.BinarySearch(new OpenEventData(eventId));
			if (num >= 0)
			{
				if (eventData[num].StartUnixTime >= time)
				{
					return time <= eventData[num].EndUnixTime;
				}
				return false;
			}
			return false;
		}
	}
}
