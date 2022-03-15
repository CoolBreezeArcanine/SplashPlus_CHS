using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class Notes : SerializeBase
	{
		public FilePath file;

		public int level;

		public int levelDecimal;

		public StringID notesDesigner;

		public int notesType;

		public int musicLevelID;

		public int maxNotes;

		public bool isEnable;

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

		public static explicit operator Manager.MaiStudio.Notes(Notes sz)
		{
			Manager.MaiStudio.Notes notes = new Manager.MaiStudio.Notes();
			notes.Init(sz);
			return notes;
		}

		public override void AddPath(string parentPath)
		{
			file.AddPath(parentPath);
			notesDesigner.AddPath(parentPath);
		}
	}
}
