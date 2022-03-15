using System;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class HolidayData : AccessorBase
	{
		public StringID name { get; private set; }

		public DateTime Day { get; private set; }

		public string dataName { get; private set; }

		public HolidayData()
		{
			name = new StringID();
			Day = default(DateTime);
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.HolidayData sz)
		{
			name = (StringID)sz.name;
			Day = sz.Day;
			dataName = sz.dataName;
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
