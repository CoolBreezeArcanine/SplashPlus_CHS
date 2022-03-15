namespace Manager
{
	public class NotesComposition
	{
		public BPMChangeDataList _bpmList = new BPMChangeDataList();

		public MeterChangeDataList _meterList = new MeterChangeDataList();

		public SoflanDataList _soflanList = new SoflanDataList();

		public ClickDataList _clickList = new ClickDataList();

		public ClickDataList _clickSeList = new ClickDataList();

		public BarDataList[] _barList = new BarDataList[4];

		public NotesTime _startGameTime = new NotesTime(0);

		public NotesTime _startNotesTime = new NotesTime(0);

		public NotesTime _endGameTime = new NotesTime(0);

		public NotesTime _endNotesTime = new NotesTime(0);

		public NotesComposition()
		{
			for (int i = 0; i < 4; i++)
			{
				_barList[i] = new BarDataList();
			}
			clear();
		}

		public void clear()
		{
			_bpmList.Clear();
			_meterList.Clear();
			_soflanList.Clear();
			_clickList.Clear();
			_clickSeList.Clear();
			BarDataList[] barList = _barList;
			for (int i = 0; i < barList.Length; i++)
			{
				barList[i].Clear();
			}
			_startGameTime.clear();
			_startNotesTime.clear();
			_endGameTime.clear();
			_endNotesTime.clear();
		}
	}
}
