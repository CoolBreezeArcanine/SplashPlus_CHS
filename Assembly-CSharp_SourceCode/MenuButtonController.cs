using IO;
using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class MenuButtonController : ButtonControllerBase
{
	[SerializeField]
	private CommonButtonObject[] _classicButtons;

	[SerializeField]
	private CommonButtonObject _photoButton;

	[SerializeField]
	private Animator[] _shootingButtons;

	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[1] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[1]);
		CommonButtons[1].Initialize(monitorIndex, InputManager.ButtonSetting.Button02, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Plus).LedColor);
		CommonButtons[1].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Plus).Image, isFlip: false);
		CommonButtons[1].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[2] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[2]);
		CommonButtons[2].Initialize(monitorIndex, InputManager.ButtonSetting.Button03, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Minus).LedColor);
		CommonButtons[2].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Minus).Image, isFlip: false);
		CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(monitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
		CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Image, isFlip: false);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Cue);
		CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
		CommonButtons[4].Initialize(monitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).LedColor);
		CommonButtons[4].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).Image, isFlip: false);
		CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).Cue);
		CommonButtons[5] = _photoButton;
		CommonButtons[5].Initialize(monitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
		Animator[] shootingButtons = _shootingButtons;
		for (int i = 0; i < shootingButtons.Length; i++)
		{
			shootingButtons[i].SetTrigger("Disabled");
		}
	}

	public void SetActivePhotoShhtingButton()
	{
		Animator[] shootingButtons = _shootingButtons;
		for (int i = 0; i < shootingButtons.Length; i++)
		{
			shootingButtons[i].SetTrigger("Pressed");
		}
		MechaManager.LedIf[MonitorIndex].SetColorButtonPressed(0, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
		MechaManager.LedIf[MonitorIndex].SetColorButtonPressed(7, CommonButtonObject.LedColors32(CommonButtonObject.LedColors.White));
	}

	public void SetPhotoShootingButton(bool isActive)
	{
		Animator[] shootingButtons = _shootingButtons;
		for (int i = 0; i < shootingButtons.Length; i++)
		{
			shootingButtons[i].SetTrigger(isActive ? "Loop" : "Disabled");
		}
		MechaManager.LedIf[MonitorIndex].SetColorButton(0, CommonButtonObject.LedColors32(isActive ? CommonButtonObject.LedColors.Red : CommonButtonObject.LedColors.Black));
		MechaManager.LedIf[MonitorIndex].SetColorButton(7, CommonButtonObject.LedColors32(isActive ? CommonButtonObject.LedColors.Red : CommonButtonObject.LedColors.Black));
	}

	public override void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
	{
		CommonButtons[(int)button].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index).Image, isFlip: false);
		CommonButtons[(int)button].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index).LedColor);
		CommonButtons[(int)button].SetSE(ButtonControllerBase.GetFlatButtonParam(index).Cue);
	}

	public override void ViewUpdate(float gameMsecAdd)
	{
		base.ViewUpdate(gameMsecAdd);
		Animator[] shootingButtons = _shootingButtons;
		foreach (Animator animator in shootingButtons)
		{
			if (animator.gameObject.activeSelf && animator.gameObject.activeInHierarchy)
			{
				animator.SetFloat("SyncTimer", SyncTimer);
			}
		}
	}
}
