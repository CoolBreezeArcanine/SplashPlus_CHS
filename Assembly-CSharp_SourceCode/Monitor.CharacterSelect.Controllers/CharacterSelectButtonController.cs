using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

namespace Monitor.CharacterSelect.Controllers
{
	public class CharacterSelectButtonController : ButtonControllerBase
	{
		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Image, isFlip: false);
			CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Cue);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
			CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).LedColor);
			CommonButtons[4].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).Image, isFlip: false);
			CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).Cue);
			CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[5]);
			CommonButtons[5].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[5].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: true);
			CommonButtons[5].SetSE(Cue.SE_SYS_CURSOR);
		}

		public override void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
		{
			if (IsActive && CommonButtons[(int)button] != null)
			{
				CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index).Image, isFlip: false);
				CommonButtons[(int)button].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index).LedColor, nowFlash: true);
				CommonButtons[(int)button].SetSE(ButtonControllerBase.GetFlatButtonParam(index).Cue);
			}
		}

		public void PressedOut(InputManager.ButtonSetting button)
		{
			if (IsActive && CommonButtons[(int)button] != null)
			{
				CommonButtons[(int)button].PressedOut();
			}
		}
	}
}
