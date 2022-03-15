using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

namespace Monitor.Result
{
	public class ResultButtonController : ButtonControllerBase
	{
		public enum ButtonType
		{
			Detail,
			Simple
		}

		[SerializeField]
		[Header("詳細切り替えボタン")]
		private CommonButtonObject _buttonObject;

		[SerializeField]
		private Sprite[] _detailsSymbols;

		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			_buttonObject.Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			_buttonObject.SetSE(Cue.SE_SYS_FIX);
			CommonButtons[2] = _buttonObject;
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: false);
		}

		public override void SetAnimationActive(int index)
		{
			if (index == 2)
			{
				_buttonObject.Pressed();
			}
			else
			{
				base.SetAnimationActive(index);
			}
		}

		public void PressedOut(InputManager.ButtonSetting button)
		{
			CommonButtons[(int)button].PressedOut();
		}

		public override void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
		{
			CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index).Image, isFlip: false);
			CommonButtons[(int)button].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index).LedColor);
		}
	}
}
