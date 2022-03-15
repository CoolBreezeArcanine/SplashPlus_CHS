using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class HolidayData : SerializeBase, ISerialize
	{
		public StringID name;

		public DateTime Day;

		public string dataName;

		public HolidayData()
		{
			name = new StringID();
			Day = default(DateTime);
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.HolidayData(HolidayData sz)
		{
			Manager.MaiStudio.HolidayData holidayData = new Manager.MaiStudio.HolidayData();
			holidayData.Init(sz);
			return holidayData;
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
