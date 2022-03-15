using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class InformationData : SerializeBase, ISerialize
	{
		public bool disable;

		public StringID name;

		public StringID eventName;

		public string infoText;

		public FilePath fileName;

		public InformationKind infoKind;

		public string dataName;

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

		public static explicit operator Manager.MaiStudio.InformationData(InformationData sz)
		{
			Manager.MaiStudio.InformationData informationData = new Manager.MaiStudio.InformationData();
			informationData.Init(sz);
			return informationData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			eventName.AddPath(parentPath);
			fileName.AddPath(parentPath);
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
