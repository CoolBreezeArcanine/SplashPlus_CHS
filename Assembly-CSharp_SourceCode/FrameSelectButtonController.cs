using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class FrameSelectButtonController : ButtonControllerBase
{
	private bool _isAnimation;

	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[0] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
		CommonButtons[0].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: false);
		CommonButtons[1] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[1]);
		CommonButtons[1].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
		CommonButtons[1].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: true);
		for (int i = 0; i < CommonButtons.Length; i++)
		{
			CommonButtons[i].SetSE(Cue.SE_SYS_CURSOR);
		}
		_isAnimation = false;
	}

	public override void SetAnimationActive(int index)
	{
		if (IsActive && CommonButtons[index] != null)
		{
			CommonButtons[index].Pressed();
		}
	}

	public override void ViewUpdate(float gameMSecAdd)
	{
		if (!_isAnimation)
		{
			return;
		}
		SyncTimer += gameMSecAdd / 1000f;
		for (int i = 0; i < CommonButtons.Length; i++)
		{
			if (CommonButtons[i] != null)
			{
				CommonButtons[i].ViewUpdate(SyncTimer);
			}
		}
		if (SyncTimer > 1f)
		{
			SyncTimer = 0f;
		}
	}

	public void ChangeActiveAnimationFlag(bool isActive)
	{
		_isAnimation = isActive;
	}
}
