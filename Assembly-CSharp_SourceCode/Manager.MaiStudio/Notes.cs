using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class Notes : AccessorBase
	{
		public FilePath file { get; private set; }

		public int level { get; private set; }

		public int levelDecimal { get; private set; }

		public StringID notesDesigner { get; private set; }

		public int notesType { get; private set; }

		public int musicLevelID { get; private set; }

		public int maxNotes { get; private set; }

		public bool isEnable { get; private set; }

		public Notes()
		{
			file = new FilePath();
			level = 0;
			levelDecimal = 0;
			notesDesigner = new StringID();
			notesType = 0;
			musicLevelID = 0;
			maxNotes = 0;
			isEnable = false;
		}

		public void Init(Manager.MaiStudio.Serialize.Notes sz)
		{
			file = (FilePath)sz.file;
			level = sz.level;
			levelDecimal = sz.levelDecimal;
			notesDesigner = (StringID)sz.notesDesigner;
			notesType = sz.notesType;
			musicLevelID = sz.musicLevelID;
			maxNotes = sz.maxNotes;
			isEnable = sz.isEnable;
		}
	}
}
