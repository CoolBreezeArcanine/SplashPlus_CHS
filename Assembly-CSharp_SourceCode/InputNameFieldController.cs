using System.Text;
using UnityEngine;

public class InputNameFieldController : MonoBehaviour
{
	[SerializeField]
	private Sprite _activeSprite;

	[SerializeField]
	private Sprite _nonActiveSprite;

	[SerializeField]
	private NameField[] _field;

	private StringBuilder _builder = new StringBuilder();

	public void Prepare()
	{
		for (int i = 0; i < _field.Length; i++)
		{
			_field[i].Prepare(i);
		}
	}

	public void ChangeActiveField(int index)
	{
		RefreshColor();
		if (_field.Length > index)
		{
			_field[index].SetFieldBgSprite(_activeSprite);
		}
	}

	public void AllNonActiveField()
	{
		RefreshColor();
	}

	public void ChangeBlank(int index)
	{
		SetString(index, "");
	}

	public void SetString(int index, string str)
	{
		if (_field.Length > index)
		{
			_field[index].SetString(str);
		}
	}

	private void RefreshColor()
	{
		for (int i = 0; i < _field.Length; i++)
		{
			_field[i].SetFieldBgSprite(_nonActiveSprite);
		}
	}

	public string GetCurrentName()
	{
		_builder.Length = 0;
		for (int i = 0; i < _field.Length; i++)
		{
			string str = _field[i].GetStr();
			if (str != "")
			{
				_builder.Append(str);
			}
		}
		return _builder.ToString();
	}

	public void AllResetString()
	{
		for (int i = 0; i < _field.Length; i++)
		{
			_field[i].SetString("");
		}
	}

	public NameField[] GetNameFieldArray()
	{
		return _field;
	}

	public int GetFullFillFieldCount()
	{
		return _field.Length;
	}
}
