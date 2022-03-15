using Manager;
using UI;
using UnityEngine;

public class CollectionInfoButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		FlatButtonType type = FlatButtonType.Skip;
		CommonButtons[0] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
		CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
		CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
	}

	public void ChangeFlatButtonSymbol(int index, int spriteNum)
	{
		if (CommonButtons[index] != null)
		{
			CommonButtons[index].SetSymbol(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).Image, isFlip: false);
			CommonButtons[index].ChangeColor(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).LedColor);
			CommonButtons[index].SetSE(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).Cue);
		}
	}
}
