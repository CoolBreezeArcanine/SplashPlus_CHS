using UnityEngine;
using UnityEngine.UI;

public class InputFieldViewer : MonoBehaviour, IInputField
{
	[SerializeField]
	private CustomInputField _titleField;

	[SerializeField]
	private CustomInputField _messageField;

	[SerializeField]
	private Button _clearTButton;

	[SerializeField]
	private Button _clearMButton;

	private void Awake()
	{
		_clearTButton.onClick.RemoveAllListeners();
		_clearTButton.onClick.AddListener(ClearTitle);
		_clearMButton.onClick.RemoveAllListeners();
		_clearMButton.onClick.AddListener(ClearMessage);
	}

	public void InsertMessage(string message)
	{
		_messageField.InsertText(message);
	}

	public void InsertTitle(string message)
	{
		_titleField.InsertText(message);
	}

	public string Title()
	{
		return _titleField.GetText();
	}

	public string Message()
	{
		return _messageField.GetText();
	}

	private void ClearTitle()
	{
		_titleField.Clear();
	}

	private void ClearMessage()
	{
		_messageField.Clear();
	}

	public void UpdateInputField()
	{
		if (_messageField != null)
		{
			_messageField.UpdateCaretPos();
		}
		if (_titleField != null)
		{
			_titleField.UpdateCaretPos();
		}
	}
}
