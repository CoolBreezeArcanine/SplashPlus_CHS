using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class UdemaeBossData : SerializeBase, ISerialize
	{
		public StringID name;

		public StringID notesDesigner;

		public StringID music;

		public StringID difficulty;

		public int achieve;

		public int achieveUnder;

		public int rating;

		public StringID daniId;

		public StringID silhouette;

		public bool isTransmission;

		public StringID transmissionRelaxEventId;

		public string dataName;

		public UdemaeBossData()
		{
			name = new StringID();
			notesDesigner = new StringID();
			music = new StringID();
			difficulty = new StringID();
			achieve = 0;
			achieveUnder = 0;
			rating = 0;
			daniId = new StringID();
			silhouette = new StringID();
			isTransmission = false;
			transmissionRelaxEventId = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.UdemaeBossData(UdemaeBossData sz)
		{
			Manager.MaiStudio.UdemaeBossData udemaeBossData = new Manager.MaiStudio.UdemaeBossData();
			udemaeBossData.Init(sz);
			return udemaeBossData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			notesDesigner.AddPath(parentPath);
			music.AddPath(parentPath);
			difficulty.AddPath(parentPath);
			daniId.AddPath(parentPath);
			silhouette.AddPath(parentPath);
			transmissionRelaxEventId.AddPath(parentPath);
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
