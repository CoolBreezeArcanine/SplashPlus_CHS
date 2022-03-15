using TMPro;
using UnityEngine;

public class CustomInputField : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _inputField;

	private int _caretIndex;

	private void Awake()
	{
		_inputField.richText = false;
	}

	public void Clear()
	{
		_inputField.text = "";
	}

	public void UpdateCaretPos()
	{
		if (_inputField.caretPosition != 0)
		{
			_caretIndex = _inputField.caretPosition;
		}
	}

	public string GetText()
	{
		return _inputField.text;
	}

	public void InsertText(string message)
	{
		string text = GetText();
		if (text.Length == 0)
		{
			_caretIndex = 0;
		}
		_inputField.text = text.Insert(_caretIndex, message);
	}
}
