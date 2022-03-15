using Manager;
using UI;
using UnityEngine;

namespace Monitor.CourseResult
{
	public class CourseResultButtonController : ButtonControllerBase
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
		}
	}
}
