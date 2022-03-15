using Manager;
using UI;
using UnityEngine;

public class TutorialSelectButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
		CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TutorialSkip).LedColor);
		CommonButtons[4].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TutorialSkip).Image, isFlip: true);
		CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TutorialSkip).Cue);
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
