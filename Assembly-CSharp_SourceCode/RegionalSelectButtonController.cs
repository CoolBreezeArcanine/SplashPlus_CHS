using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class RegionalSelectButtonController : ButtonControllerBase
{
	public readonly int HashIn = Animator.StringToHash("In");

	public readonly int HashOut = Animator.StringToHash("Out");

	public readonly int HashSwitchOff = Animator.StringToHash("Switch_Off");

	public readonly int HashSwitchOn = Animator.StringToHash("Switch_On");

	[SerializeField]
	private Transform _button06Target;

	private Animator _toggleButtonAnimation;

	private bool _isEvent;

	private bool _isRegionStateChange;

	public void Initialize(int monitorIndex, bool isEvent)
	{
		base.Initialize(monitorIndex);
		_isEvent = isEvent;
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[1] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[1]);
		CommonButtons[1].Initialize(monitorIndex, InputManager.ButtonSetting.Button02, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.CollaboArea).LedColor);
		CommonButtons[1].SetSymbolStartDisable(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.CollaboArea).Image, isFlip: false);
		CommonButtons[1].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.CollaboArea).Cue);
		CommonButtons[2] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[2]);
		CommonButtons[2].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
		CommonButtons[2].SetSymbolStartDisable(ButtonControllerBase.ArrowSelectorSprite, isFlip: false);
		CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(monitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).LedColor);
		CommonButtons[3].SetSymbolStartDisable(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Image, isFlip: false);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Skip).Cue);
		CommonButtons[5] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[5]);
		CommonButtons[5].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
		CommonButtons[5].SetSymbolStartDisable(ButtonControllerBase.ArrowSelectorSprite, isFlip: true);
		CommonButtons[5].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[5].transform.SetParent(_button06Target, worldPositionStays: false);
		CommonButtons[5].transform.localScale = new Vector3(1f, 1f, 1f);
		CommonButtons[5].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		CommonButtons[5].transform.localPosition = Vector3.zero;
		CommonButtons[7] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[7]);
		CommonButtons[7].Initialize(monitorIndex, InputManager.ButtonSetting.Button08, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Setting).LedColor);
		CommonButtons[7].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Setting).Image, isFlip: false);
		CommonButtons[7].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Setting).Cue);
	}

	public void SetActiveToggleButton(bool isActive, bool toggleState)
	{
		if ((bool)_toggleButtonAnimation)
		{
			_toggleButtonAnimation.SetLayerWeight(1, toggleState ? 1f : 0f);
			_toggleButtonAnimation.Play(isActive ? HashIn : HashOut, 0, 0f);
		}
	}

	public void SwtchToggle(bool value)
	{
		if ((bool)_toggleButtonAnimation)
		{
			_toggleButtonAnimation.SetLayerWeight(1, value ? 1f : 0f);
			_toggleButtonAnimation.Play(value ? HashSwitchOn : HashSwitchOff, 0, 0f);
		}
	}

	public void ChangeDecisionButton()
	{
		ButtonInformation flatButtonParam = ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok);
		CommonButtons[3].SetSymbol(flatButtonParam.Image, isFlip: false);
		CommonButtons[3].ChangeColor(flatButtonParam.LedColor);
		CommonButtons[3].SetSE(flatButtonParam.Cue);
	}

	public void InitRegionButtonState(bool value)
	{
		_isRegionStateChange = value;
		if (_isEvent)
		{
			if (value)
			{
				ButtonInformation flatButtonParam = ButtonControllerBase.GetFlatButtonParam(FlatButtonType.CollaboArea);
				CommonButtons[1].ChangeColor(flatButtonParam.LedColor);
				CommonButtons[1].SetSymbol(flatButtonParam.Image, isFlip: false);
				CommonButtons[1].SetSE(flatButtonParam.Cue);
			}
			else
			{
				ButtonInformation flatButtonParam2 = ButtonControllerBase.GetFlatButtonParam(FlatButtonType.OriginalArea);
				CommonButtons[1].ChangeColor(flatButtonParam2.LedColor);
				CommonButtons[1].SetSymbol(flatButtonParam2.Image, isFlip: false);
				CommonButtons[1].SetSE(flatButtonParam2.Cue);
			}
		}
	}

	public void ChangeRegionButtonState(bool value)
	{
		if (_isEvent && _isRegionStateChange != value)
		{
			_isRegionStateChange = value;
			if (value)
			{
				ButtonInformation flatButtonParam = ButtonControllerBase.GetFlatButtonParam(FlatButtonType.CollaboArea);
				CommonButtons[1]?.ChangeColor(flatButtonParam.LedColor);
				CommonButtons[1]?.SetSymbol(flatButtonParam.Image, isFlip: false);
				CommonButtons[1]?.SetSE(flatButtonParam.Cue);
			}
			else
			{
				ButtonInformation flatButtonParam2 = ButtonControllerBase.GetFlatButtonParam(FlatButtonType.OriginalArea);
				CommonButtons[1]?.ChangeColor(flatButtonParam2.LedColor);
				CommonButtons[1]?.SetSymbol(flatButtonParam2.Image, isFlip: false);
				CommonButtons[1]?.SetSE(flatButtonParam2.Cue);
			}
		}
	}

	public void ChangeSettingButton(bool mode)
	{
		FlatButtonType type = (mode ? FlatButtonType.Setting : FlatButtonType.SettingBack);
		CommonButtons[7].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
	}
}
