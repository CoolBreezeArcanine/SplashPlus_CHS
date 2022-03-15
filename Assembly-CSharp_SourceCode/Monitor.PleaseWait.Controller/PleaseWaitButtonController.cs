using Manager;
using UI;
using UnityEngine;

namespace Monitor.PleaseWait.Controller
{
	public class PleaseWaitButtonController : ButtonControllerBase
	{
		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(monitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Image, isFlip: false);
			CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Cue);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
			CommonButtons[4].Initialize(monitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Cancel).LedColor);
			CommonButtons[4].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Cancel).Image, isFlip: false);
			CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Cancel).Cue);
		}
	}
}
