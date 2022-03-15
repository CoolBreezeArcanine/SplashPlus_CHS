using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CourseModeData : AccessorBase
	{
		public StringID name { get; private set; }

		public bool isForceFail { get; private set; }

		public string detail { get; private set; }

		public string modefile { get; private set; }

		public string categoryfile { get; private set; }

		public string tabfile { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.CourseModeData sz)
		{
			name = (StringID)sz.name;
			isForceFail = sz.isForceFail;
			detail = sz.detail;
			modefile = sz.modefile;
			categoryfile = sz.categoryfile;
			tabfile = sz.tabfile;
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
