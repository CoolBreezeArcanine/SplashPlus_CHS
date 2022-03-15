using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class InformationData : AccessorBase
	{
		public bool disable { get; private set; }

		public StringID name { get; private set; }

		public StringID eventName { get; private set; }

		public string infoText { get; private set; }

		public FilePath fileName { get; private set; }

		public InformationKind infoKind { get; private set; }

		public string dataName { get; private set; }

		public InformationData()
		{
			disable = false;
			name = new StringID();
			eventName = new StringID();
			infoText = "";
			fileName = new FilePath();
			infoKind = InformationKind.Info;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.InformationData sz)
		{
			disable = sz.disable;
			name = (StringID)sz.name;
			eventName = (StringID)sz.eventName;
			infoText = sz.infoText;
			fileName = (FilePath)sz.fileName;
			infoKind = sz.infoKind;
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
			return disable;
		}
	}
}
