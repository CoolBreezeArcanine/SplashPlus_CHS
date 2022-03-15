using Manager;
using UI;
using UnityEngine;

public class InformationButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: true);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Cue);
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
