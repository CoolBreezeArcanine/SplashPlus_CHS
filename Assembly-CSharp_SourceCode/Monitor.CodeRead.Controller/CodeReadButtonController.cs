using Manager;
using UI;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class CodeReadButtonController : ButtonControllerBase
	{
		private int _button4CurrentIndex;

		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
			CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TimeSkip).LedColor);
			CommonButtons[4].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.TimeSkip).Image, isFlip: false);
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[5]);
			CommonButtons[5].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[5].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: true);
		}

		public override void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
		{
			switch (button)
			{
			case InputManager.ButtonSetting.Button04:
				if (index != _button4CurrentIndex)
				{
					_button4CurrentIndex = index;
					switch (index)
					{
					case 0:
						CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
						break;
					case 1:
						CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Set).Image, isFlip: false);
						break;
					case 2:
						CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: false);
						break;
					}
				}
				break;
			case InputManager.ButtonSetting.Button05:
				switch (index)
				{
				case 0:
				{
					int index2 = 7;
					CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index2).Image, isFlip: false);
					CommonButtons[(int)button].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index2).LedColor, nowFlash: true);
					CommonButtons[(int)button].SetSE(ButtonControllerBase.GetFlatButtonParam(index2).Cue);
					break;
				}
				case 1:
				{
					int index2 = 6;
					CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index2).Image, isFlip: false);
					CommonButtons[(int)button].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index2).LedColor, nowFlash: true);
					CommonButtons[(int)button].SetSE(ButtonControllerBase.GetFlatButtonParam(index2).Cue);
					break;
				}
				}
				break;
			}
		}
	}
}
