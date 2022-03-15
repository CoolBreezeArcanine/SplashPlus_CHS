using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class UdemaeBossData : AccessorBase
	{
		public StringID name { get; private set; }

		public StringID notesDesigner { get; private set; }

		public StringID music { get; private set; }

		public StringID difficulty { get; private set; }

		public int achieve { get; private set; }

		public int achieveUnder { get; private set; }

		public int rating { get; private set; }

		public StringID daniId { get; private set; }

		public StringID silhouette { get; private set; }

		public bool isTransmission { get; private set; }

		public StringID transmissionRelaxEventId { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.UdemaeBossData sz)
		{
			name = (StringID)sz.name;
			notesDesigner = (StringID)sz.notesDesigner;
			music = (StringID)sz.music;
			difficulty = (StringID)sz.difficulty;
			achieve = sz.achieve;
			achieveUnder = sz.achieveUnder;
			rating = sz.rating;
			daniId = (StringID)sz.daniId;
			silhouette = (StringID)sz.silhouette;
			isTransmission = sz.isTransmission;
			transmissionRelaxEventId = (StringID)sz.transmissionRelaxEventId;
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
