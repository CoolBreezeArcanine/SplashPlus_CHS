using Manager;
using UI;
using UnityEngine;

public class TakeOverButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Cue);
	}

	public void ChangeFlatButtonSymbol(int index, int spriteNum)
	{
		if (CommonButtons[index] != null)
		{
			CommonButtons[index].SetSymbol(ButtonControllerBase.GetFlatButtonParam(spriteNum).Image, isFlip: false);
			CommonButtons[index].ChangeColor(ButtonControllerBase.GetFlatButtonParam(spriteNum).LedColor);
			CommonButtons[index].SetSE(ButtonControllerBase.GetFlatButtonParam(spriteNum).Cue);
		}
	}
}