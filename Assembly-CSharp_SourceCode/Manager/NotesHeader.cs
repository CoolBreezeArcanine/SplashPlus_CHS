namespace Manager
{
	public class NotesHeader
	{
		public FormatType _format = FormatType.FORMAT_INVALID;

		public string _notesName;

		public Version[] _version = new Version[2];

		public string _creator;

		public BPMInfo _bpmInfo = new BPMInfo();

		public MeterInfo _metInfo = new MeterInfo();

		public int _resolutionTime;

		public int _resolutionX;

		public int _clickFirst;

		public float _progJudgeBPM;

		public bool _isTutorial;

		public bool _isFes;

		public int _touchNum;

		public int _maxNotes;

		public NotesHeader()
		{
			for (int i = 0; i < 2; i++)
			{
				_version[i] = new Version();
			}
			clear();
		}

		public void clear()
		{
			_notesName = "";
			Version[] version = _version;
			for (int i = 0; i < version.Length; i++)
			{
				version[i].init();
			}
			_creator = "";
			_bpmInfo.init();
			_metInfo.init();
			_resolutionTime = 1920;
			_clickFirst = 0;
			_progJudgeBPM = 240f;
			_isTutorial = false;
			_isFes = false;
			_touchNum = 0;
			_maxNotes = 0;
		}

		public ClickDataList getClickFirstList(BarDataList barList_Beat04)
		{
			ClickDataList clickDataList = new ClickDataList();
			NotesTime notesTime = new NotesTime(_clickFirst);
			foreach (BarData item in barList_Beat04)
			{
				if (item.time < notesTime)
				{
					ClickData clickData = new ClickData();
					clickData.time = item.time;
					clickDataList.Add(clickData);
					continue;
				}
				return clickDataList;
			}
			return clickDataList;
		}
	}
}
