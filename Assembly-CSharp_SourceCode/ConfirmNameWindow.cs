using DB;
using UnityEngine;

public class ConfirmNameWindow : CommonWindow
{
	[SerializeField]
	[Header("名前入力フィールド")]
	private InputNameFieldController _inputController;

	public void Prepare(NameField[] nameField, WindowMessageID id)
	{
		_inputController.AllResetString();
		for (int i = 0; i < nameField.Length; i++)
		{
			string str = nameField[i].GetStr();
			if (str == "终")
			{
				break;
			}
			_inputController.SetString(i, (str == "␣") ? "\u3000" : str);
		}
		_titleText.text = id.GetTitle();
		_messageText.text = id.GetName();
		_lifeCounter = 0f;
		LifeTime = -1f;
		_state = WindowState.Prepare;
		_isOpening = true;
		SetPosition(Vector3.zero);
	}
}
