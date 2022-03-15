using System.Text;
using DB;

public class PseudoStringList
{
	private string[] _list;

	private int _currentIndex;

	private StringBuilder builder = new StringBuilder();

	public PseudoStringList()
	{
		_list = new string[8];
		Clear();
	}

	public void Delete()
	{
		if (0 <= _currentIndex - 1)
		{
			_currentIndex--;
		}
		_list[_currentIndex] = "";
	}

	public void DeleteEnd()
	{
		_list[_currentIndex] = "";
	}

	public void Append(string str)
	{
		_list[_currentIndex] = str;
		if (_currentIndex < _list.Length - 1)
		{
			_currentIndex++;
		}
	}

	public bool IsEnd()
	{
		return _list.Length == _currentIndex;
	}

	public void ReplaceDefaultName()
	{
		Clear();
		string name = CommonMessageID.DefaultUserName.GetName();
		for (int i = 0; i < name.Length; i++)
		{
			Append(name[i].ToString());
		}
	}

	public string Name()
	{
		return ConvertList();
	}

	private string ConvertList()
	{
		builder.Clear();
		for (int i = 0; i < _list.Length; i++)
		{
			if (_list[i] != "")
			{
				builder.Append(_list[i]);
			}
		}
		return builder.ToString();
	}

	public string EndMaxStr()
	{
		return _list[7];
	}

	private void Clear()
	{
		for (int i = 0; i < _list.Length; i++)
		{
			_list[i] = "";
		}
		_currentIndex = 0;
	}
}
