using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class CollectionButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[0] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
		CommonButtons[0].SetSymbol(ButtonControllerBase.ArrowSprite, isFlip: false);
		CommonButtons[0].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[1] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[1]);
		CommonButtons[1].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
		CommonButtons[1].SetSymbol(ButtonControllerBase.ArrowSprite, isFlip: true);
		CommonButtons[1].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[2] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[2]);
		CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
		CommonButtons[2].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Image, isFlip: false);
		CommonButtons[2].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Cue);
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TimeSkip).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TimeSkip).Image, isFlip: true);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TimeSkip).Cue);
	}

	public void ChangeFlatButtonSymbol(int index, int spriteNum)
	{
		if (CommonButtons[index] != null)
		{
			CommonButtons[index].SetSymbol(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).Image, isFlip: false);
			CommonButtons[index].ChangeColor(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).LedColor, nowFlash: true);
			CommonButtons[index].SetSE(ButtonControllerBase.GetFlatButtonParam((FlatButtonType)spriteNum).Cue);
		}
	}
}
