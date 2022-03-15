using Manager;
using UI;
using UnityEngine;

public class PhotoAgreeButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Agree).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Image, isFlip: false);
		CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
		CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Disagree).LedColor);
		CommonButtons[4].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.No).Image, isFlip: false);
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
