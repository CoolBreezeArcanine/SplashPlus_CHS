using Manager;
using UI;
using UnityEngine;

public class MapResultButtonController : ButtonControllerBase
{
	[SerializeField]
	private Sprite[] _sprites;

	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[0] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).LedColor);
		CommonButtons[0].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: false);
		CommonButtons[0].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Cue);
	}
}
