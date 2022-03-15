using Manager;
using UI;
using UnityEngine;

public class FriendBattleResultButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: false);
	}

	public override void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
	{
		if (button == InputManager.ButtonSetting.Button04)
		{
			CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam((index == 0) ? FlatButtonType.Skip : FlatButtonType.Next).Image, isFlip: false);
		}
	}

	public override void SetVisible(bool visible, params int[] ids)
	{
		foreach (int num in ids)
		{
			CommonButtons[num].SetActiveImmediateButton(visible);
		}
	}
}
