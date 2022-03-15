using Manager;
using UI;
using UnityEngine;

public class ContinueButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Image, isFlip: false);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Cue);
		CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
		CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.No).LedColor);
		CommonButtons[4].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.No).Image, isFlip: true);
		CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.No).Cue);
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
