using DB;

public class PlayerNameInfo
{
	public enum InputType : byte
	{
		Large,
		Num,
		Small,
		Symbol
	}

	private int _listIndex;

	private InputType _inputType;

	private int _nameFieldIndex;

	private bool _isFieldEnd;

	private PseudoStringList _name;

	public InputType Type
	{
		get
		{
			return _inputType;
		}
		set
		{
			_inputType = value;
		}
	}

	public int NameFieldIndex
	{
		get
		{
			return _nameFieldIndex;
		}
		set
		{
			_nameFieldIndex = value;
		}
	}

	public bool IsFieldEnd
	{
		get
		{
			return _isFieldEnd;
		}
		set
		{
			_isFieldEnd = value;
		}
	}

	public string CurrentName()
	{
		return _name.Name();
	}

	public void Initialize(string userName)
	{
		_listIndex = 0;
		_inputType = InputType.Large;
		_nameFieldIndex = ((!(userName == "")) ? userName.Length : 0);
		if (8 <= _nameFieldIndex)
		{
			_nameFieldIndex--;
		}
		_isFieldEnd = false;
		_name = new PseudoStringList();
		if (userName != "")
		{
			for (int i = 0; i < userName.Length; i++)
			{
				_name.Append(userName[i].ToString());
			}
		}
	}

	public bool ChangeInputType(InputType type)
	{
		if (_inputType == type)
		{
			return false;
		}
		_inputType = type;
		_listIndex = 0;
		return true;
	}

	public void ChangeEnd()
	{
		_listIndex = GetCurrentListNum(_inputType) - 1;
	}

	private int GetLoadIndex(int diff)
	{
		int num = _listIndex + diff;
		int currentListNum = GetCurrentListNum(_inputType);
		while (num >= currentListNum)
		{
			num -= currentListNum;
		}
		while (num < 0)
		{
			num = currentListNum + num;
		}
		return num;
	}

	private string GetName(int loadIndex)
	{
		string text = "";
		switch (_inputType)
		{
		case InputType.Large:
			text = ((CharlistAbcLargeID)loadIndex).GetName();
			break;
		case InputType.Num:
			text = ((CharlistNumID)loadIndex).GetName();
			break;
		case InputType.Small:
			text = ((CharlistAbcSmallID)loadIndex).GetName();
			break;
		case InputType.Symbol:
			text = ((CharlistSymboleID)loadIndex).GetName();
			break;
		}
		if (text == "\u3000")
		{
			text = "␣";
		}
		return text;
	}

	public void SetInputTypeAndListIndex()
	{
		string text = _name.EndMaxStr();
		int end = CharlistAbcLargeIDEnum.GetEnd();
		for (int i = 0; i < end; i++)
		{
			if (((CharlistAbcLargeID)i).GetName() == text)
			{
				_listIndex = i;
				_inputType = InputType.Large;
				return;
			}
		}
		int end2 = CharlistAbcSmallIDEnum.GetEnd();
		for (int j = 0; j < end2; j++)
		{
			if (((CharlistAbcSmallID)j).GetName() == text)
			{
				_listIndex = j;
				_inputType = InputType.Small;
				return;
			}
		}
		int end3 = CharlistNumIDEnum.GetEnd();
		for (int k = 0; k < end3; k++)
		{
			if (((CharlistNumID)k).GetName() == text)
			{
				_listIndex = k;
				_inputType = InputType.Num;
				return;
			}
		}
		int end4 = CharlistSymboleIDEnum.GetEnd();
		for (int l = 0; l < end4; l++)
		{
			if (((CharlistSymboleID)l).GetName() == text)
			{
				_listIndex = l;
				_inputType = InputType.Symbol;
				break;
			}
		}
	}

	public string GetString()
	{
		return GetName(_listIndex);
	}

	public string GetString(int diff)
	{
		int loadIndex = GetLoadIndex(diff);
		return GetName(loadIndex);
	}

	private int GetCurrentListNum(InputType type)
	{
		return type switch
		{
			InputType.Large => CharlistAbcLargeIDEnum.GetEnd(), 
			InputType.Num => CharlistNumIDEnum.GetEnd(), 
			InputType.Small => CharlistAbcSmallIDEnum.GetEnd(), 
			InputType.Symbol => CharlistSymboleIDEnum.GetEnd(), 
			_ => 0, 
		};
	}

	public void GotoEnd()
	{
		_listIndex = GetCurrentListNum(_inputType) - 1;
	}

	public void ScrollLeft()
	{
		if (_listIndex + 1 < GetCurrentListNum(_inputType))
		{
			_listIndex++;
		}
		else
		{
			_listIndex = 0;
		}
	}

	public void ScrollRight()
	{
		if (0 <= _listIndex - 1)
		{
			_listIndex--;
		}
		else
		{
			_listIndex = GetCurrentListNum(_inputType) - 1;
		}
	}

	public void SkipEnd()
	{
		_listIndex = GetCurrentListNum(_inputType) - 1;
	}

	public void AddString(string str)
	{
		_name.Append(str);
	}

	public void DeleteName()
	{
		_nameFieldIndex--;
		_name.Delete();
	}

	public void DeleteFieldEnd()
	{
		_name.DeleteEnd();
		_isFieldEnd = false;
	}

	public void ReplaceDefaultName()
	{
		_name.ReplaceDefaultName();
		_nameFieldIndex = CommonMessageID.DefaultUserName.GetName().Length;
	}
}
