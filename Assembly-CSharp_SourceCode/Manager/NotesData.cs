using System.Collections.Generic;

namespace Manager
{
	public class NotesData
	{
		public NoteDataList _noteData = new NoteDataList();

		public List<TouchChainList> _touchChainList = new List<TouchChainList>();

		public NotesData()
		{
			clear();
		}

		public void clear()
		{
			_noteData.Clear();
			_touchChainList.Clear();
		}
	}
}
