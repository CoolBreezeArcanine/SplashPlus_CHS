using Manager.MaiStudio;

public class CollectionData
{
	private StringID _netOpenName = new StringID();

	private StringID _releaseTagName = new StringID();

	private bool _disable;

	private StringID _name = new StringID();

	private StringID _genre = new StringID();

	private bool _isDefault;

	private string _normText = "";

	private TrophyRareType _rareType = TrophyRareType.Bronze;

	private string _fileName = "";

	private bool _isNew;

	private bool _isHave;

	private bool _isDisp;

	private int _priority;

	private string _dataName = "";

	private int _id = -1;

	private string _nameStr = "";

	public StringID NetOpenName
	{
		get
		{
			return _netOpenName;
		}
		set
		{
			_netOpenName = value;
		}
	}

	public StringID ReleaseTagName
	{
		get
		{
			return _releaseTagName;
		}
		set
		{
			_releaseTagName = value;
		}
	}

	public bool Disable
	{
		get
		{
			return _disable;
		}
		set
		{
			_disable = value;
		}
	}

	public StringID Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public StringID Genre
	{
		get
		{
			return _genre;
		}
		set
		{
			_genre = value;
		}
	}

	public bool IsDefault
	{
		get
		{
			return _isDefault;
		}
		set
		{
			_isDefault = value;
		}
	}

	public bool IsRandom => _id == 2;

	public string NormText
	{
		get
		{
			return _normText;
		}
		set
		{
			_normText = value;
		}
	}

	public TrophyRareType TrophyRareType
	{
		get
		{
			return _rareType;
		}
		set
		{
			_rareType = value;
		}
	}

	public string FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
		}
	}

	public bool IsNew
	{
		get
		{
			return _isNew;
		}
		set
		{
			_isNew = value;
		}
	}

	public bool IsHave
	{
		get
		{
			return _isHave;
		}
		set
		{
			_isHave = value;
		}
	}

	public bool IsDisp
	{
		get
		{
			return _isDisp;
		}
		set
		{
			_isDisp = value;
		}
	}

	public int Priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
		}
	}

	public string DataName
	{
		get
		{
			return _dataName;
		}
		set
		{
			_dataName = value;
		}
	}

	public int ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public string NameStr
	{
		get
		{
			return _nameStr;
		}
		set
		{
			_nameStr = value;
		}
	}

	public int GetID()
	{
		return _id;
	}
}
