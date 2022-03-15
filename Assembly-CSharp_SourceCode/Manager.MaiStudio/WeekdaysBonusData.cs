using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class WeekdaysBonusData : AccessorBase
	{
		public StringID name { get; private set; }

		public DateTime StartDay { get; private set; }

		public DateTime EndDay { get; private set; }

		public ReadOnlyCollection<DateTime> IgnoreDay { get; private set; }

		public string dataName { get; private set; }

		public WeekdaysBonusData()
		{
			name = new StringID();
			StartDay = default(DateTime);
			EndDay = default(DateTime);
			IgnoreDay = new ReadOnlyCollection<DateTime>(new List<DateTime>());
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.WeekdaysBonusData sz)
		{
			name = (StringID)sz.name;
			StartDay = sz.StartDay;
			EndDay = sz.EndDay;
			List<DateTime> list = new List<DateTime>();
			foreach (DateTime item in sz.IgnoreDay)
			{
				list.Add(item);
			}
			IgnoreDay = new ReadOnlyCollection<DateTime>(list);
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
