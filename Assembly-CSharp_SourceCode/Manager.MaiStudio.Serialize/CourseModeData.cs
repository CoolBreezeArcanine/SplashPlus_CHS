using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class CourseModeData : SerializeBase, ISerialize
	{
		public StringID name;

		public bool isForceFail;

		public string detail;

		public string modefile;

		public string categoryfile;

		public string tabfile;

		public string dataName;

		public CourseModeData()
		{
			name = new StringID();
			isForceFail = false;
			detail = "";
			modefile = "";
			categoryfile = "";
			tabfile = "";
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.CourseModeData(CourseModeData sz)
		{
			Manager.MaiStudio.CourseModeData courseModeData = new Manager.MaiStudio.CourseModeData();
			courseModeData.Init(sz);
			return courseModeData;
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
