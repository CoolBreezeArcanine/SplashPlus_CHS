using Manager;
using UI;
using UnityEngine;

public class CollectionGetButtonController : ButtonControllerBase
{
	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[0] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
		CommonButtons[0].SetSymbolStartDisable(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
		CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Cue);
	}
}
