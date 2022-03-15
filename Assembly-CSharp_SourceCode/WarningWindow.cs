using UnityEngine;

public class WarningWindow : CommonWindow
{
	public void Prepare(WarningWindowInfo info, Vector3 position)
	{
		CommonPrepare(info, position);
	}

	private void CommonPrepare(WarningWindowInfo info, Vector3 position)
	{
		_messageText.text = info.Message;
		SetHeight();
		_titleText.text = info.Title;
		SetPosition(position);
		_lifeCounter = 0f;
		LifeTime = info.LifeTime;
		_state = WindowState.Prepare;
	}
}
