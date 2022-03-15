using Manager;
using Monitor.MusicSelect.UI;
using UI;
using UnityEngine;

public class ButtonSelectController : ButtonControllerBase
{
	[SerializeField]
	[Header("固定難易度変更ボタン")]
	private DifficultyButtonObject _difficultyButton01;

	[SerializeField]
	private DifficultyButtonObject _difficultyButton06;

	[SerializeField]
	[Header("カテゴリタブボタン")]
	private CategorytabButtonObject _categorytabButton00;

	[SerializeField]
	private CategorytabButtonObject _categorytabButton07;

	[SerializeField]
	private CommonButtonObject _plusButton;

	[SerializeField]
	private CommonButtonObject _minusButton;

	[SerializeField]
	private Transform[] _buttonPositions;

	public void Initialize(int monitorIndex, bool isActive)
	{
		MonitorIndex = monitorIndex;
		IsActive = isActive;
		if (isActive)
		{
			CommonButtons = new CommonButtonObject[_buttonPositions.Length];
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _buttonPositions[2]);
			CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _buttonPositions[3]);
			CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _buttonPositions[4]);
			CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).LedColor);
			CommonButtons[4].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).Image, isFlip: false);
			CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _buttonPositions[5]);
			CommonButtons[5].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[5].SetSymbolSprite(ButtonControllerBase.ArrowSprite, isFlip: true);
		}
	}

	public void InitializeForInfo(int monitorIndex, bool isActive)
	{
		MonitorIndex = monitorIndex;
		IsActive = isActive;
		CommonButtons = new CommonButtonObject[_buttonPositions.Length];
		if (isActive)
		{
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _buttonPositions[3]);
			CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).LedColor);
			CommonButtons[3].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Yes).Image, isFlip: false);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _buttonPositions[4]);
			CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Back).LedColor);
			CommonButtons[4].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.No).Image, isFlip: false);
			SetVisible(false, InputManager.ButtonSetting.Button04, InputManager.ButtonSetting.Button05);
		}
	}

	public void ViewUpdate()
	{
		if (!IsActive)
		{
			return;
		}
		SyncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
		for (int i = 0; i < CommonButtons.Length; i++)
		{
			if (CommonButtons[i] != null)
			{
				CommonButtons[i].ViewUpdate(SyncTimer);
			}
		}
		_plusButton?.ViewUpdate(SyncTimer);
		_minusButton?.ViewUpdate(SyncTimer);
		if (SyncTimer > 1f)
		{
			SyncTimer = 0f;
		}
	}

	public void Pressed(InputManager.ButtonSetting buttonSetting)
	{
		if (IsActive && CommonButtons[(int)buttonSetting] != null)
		{
			CommonButtons[(int)buttonSetting].Pressed();
		}
	}

	public bool GetVisible(int id)
	{
		return CommonButtons[id].IsVisible();
	}

	public void ChangeButtonImage(InputManager.ButtonSetting buttonSetting, int index)
	{
		if (IsActive)
		{
			if ((int)buttonSetting < CommonButtons.Length && index < ButtonControllerBase.FlatButtonParam.Length)
			{
				CommonButtons[(int)buttonSetting].SetSymbol(ButtonControllerBase.GetFlatButtonParam(index).Image, isFlip: false);
				CommonButtons[(int)buttonSetting].ChangeColor(ButtonControllerBase.GetFlatButtonParam(index).LedColor, nowFlash: true);
				CommonButtons[(int)buttonSetting].SetSE(ButtonControllerBase.GetFlatButtonParam(index).Cue);
			}
		}
	}

	public void PressedTouchPanel(InputManager.TouchPanelArea area)
	{
		switch (area)
		{
		case InputManager.TouchPanelArea.B4:
			_plusButton.Pressed();
			break;
		case InputManager.TouchPanelArea.B5:
			_minusButton.Pressed();
			break;
		}
	}
}
