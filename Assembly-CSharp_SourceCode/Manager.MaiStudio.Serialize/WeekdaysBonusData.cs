using System;
using System.Collections.Generic;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class WeekdaysBonusData : SerializeBase, ISerialize
	{
		public StringID name;

		public DateTime StartDay;

		public DateTime EndDay;

		public List<DateTime> IgnoreDay;

		public string dataName;

		public WeekdaysBonusData()
		{
			name = new StringID();
			StartDay = default(DateTime);
			EndDay = default(DateTime);
			IgnoreDay = new List<DateTime>();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.WeekdaysBonusData(WeekdaysBonusData sz)
		{
			Manager.MaiStudio.WeekdaysBonusData weekdaysBonusData = new Manager.MaiStudio.WeekdaysBonusData();
			weekdaysBonusData.Init(sz);
			return weekdaysBonusData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
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
