using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

namespace Monitor.TicketSelect
{
	public class TicketSelectButtonController : ButtonControllerBase
	{
		[SerializeField]
		private Sprite[] _sprites;

		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			FlatButtonType type = FlatButtonType.Ok;
			CommonButtons[0] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[0]);
			CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
			type = FlatButtonType.TimeSkip;
			CommonButtons[1] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[1]);
			CommonButtons[1].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[1].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[1].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
			type = FlatButtonType.Back;
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[2].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[2].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[3].SetSymbol(ButtonControllerBase.ArrowSelectorSprite, isFlip: false);
			CommonButtons[3].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
			CommonButtons[4].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[4].SetSymbol(ButtonControllerBase.ArrowSelectorSprite, isFlip: false);
			CommonButtons[4].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[5]);
			CommonButtons[5].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[5].SetSymbol(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[5].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[6] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[6]);
			CommonButtons[6].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[6].SetSymbol(ButtonControllerBase.ArrowSprite, isFlip: true);
			CommonButtons[6].SetSE(Cue.SE_SYS_CURSOR);
			type = FlatButtonType.EMoneyChange;
			CommonButtons[7] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[7]);
			CommonButtons[7].Initialize(monitorIndex, InputManager.ButtonSetting.Button02, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[7].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[7].SetSE(Cue.SE_SYS_CURSOR);
		}

		public void ChangeButtonType()
		{
			FlatButtonType type = FlatButtonType.Skip;
			CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[0].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
		}

		public void ChangeButtonType(FlatButtonType type)
		{
			switch (type)
			{
			case FlatButtonType.Ok:
				CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
				CommonButtons[0].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor);
				CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
				break;
			case FlatButtonType.Decide:
				CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
				CommonButtons[0].ChangeColor(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
				CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
				break;
			case FlatButtonType.Back:
				CommonButtons[2].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
				CommonButtons[2].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor);
				CommonButtons[2].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
				break;
			case FlatButtonType.Cancel:
				CommonButtons[2].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
				CommonButtons[2].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor);
				CommonButtons[2].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
				break;
			}
		}

		public void ChangeOKButtonColor(bool isGray = false)
		{
			if (!isGray)
			{
				FlatButtonType type = FlatButtonType.Ok;
				CommonButtons[0].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor, nowFlash: true);
			}
			else
			{
				CommonButtons[0].ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
			}
		}

		public void ChangeBackButtonColor(bool isGray = false)
		{
			if (!isGray)
			{
				FlatButtonType type = FlatButtonType.Back;
				CommonButtons[2].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor, nowFlash: true);
			}
			else
			{
				CommonButtons[2].ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
			}
		}

		public void ChangeBalanceButtonColor(bool isGray = false)
		{
			if (!isGray)
			{
				FlatButtonType type = FlatButtonType.EMoneyChange;
				CommonButtons[7].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor, nowFlash: true);
			}
			else
			{
				CommonButtons[7].ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
			}
		}
	}
}
