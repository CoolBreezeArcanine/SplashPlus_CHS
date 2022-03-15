using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

namespace Monitor.CourseSelect
{
	public class CourseSelectButtonController : ButtonControllerBase
	{
		[SerializeField]
		private Sprite[] _sprites;

		public override void Initialize(int monitorIndex)
		{
			base.Initialize(monitorIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			FlatButtonType type = FlatButtonType.Ok;
			FlatButtonType type2 = FlatButtonType.Back;
			CommonButtons[0] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[0]);
			CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
			CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(type).Cue);
			CommonButtons[1] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[1]);
			CommonButtons[1].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(type2).LedColor);
			CommonButtons[1].SetSymbol(ButtonControllerBase.GetFlatButtonParam(type2).Image, isFlip: false);
			CommonButtons[1].SetSE(ButtonControllerBase.GetFlatButtonParam(type2).Cue);
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[3].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: true);
			CommonButtons[3].SetSE(Cue.SE_SYS_CURSOR);
		}
	}
}
