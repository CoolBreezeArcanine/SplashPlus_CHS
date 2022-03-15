using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

namespace Monitor.LoginBonus
{
	public class LoginBonusButtonController : ButtonControllerBase
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
			CommonButtons[1] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[1]);
			CommonButtons[1].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[1].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[1].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: true);
			CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
		}

		public void ChangeButtonType()
		{
			FlatButtonType type = FlatButtonType.Skip;
			CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[0].ChangeColor(ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
		}
	}
}
