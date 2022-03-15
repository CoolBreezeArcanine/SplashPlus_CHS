using IO;
using Manager;
using Monitor.PhotoEdit.UI;
using Process;
using UI;
using UnityEngine;

namespace Monitor.PhotoEdit
{
	public class PhotoEditButtonController : ButtonControllerBase
	{
		private const string PhotoCounterPath = "Null_PhotoButton/IMG_Button_B/IMG_Button_A/NUM_PhotoNumber";

		[SerializeField]
		[Header("トラック切り替えボタン")]
		private CommonButtonObject _leftPhotoButton;

		[SerializeField]
		private CommonButtonObject _rightPhotoButton;

		[SerializeField]
		private CommonButtonObject _changeButtonObject;

		[SerializeField]
		private CommonButtonObject _editTouchButtonObject;

		[SerializeField]
		private CommonButtonObject _zoomTouchButtonObject;

		[SerializeField]
		private Sprite[] _changeSymbolSprites;

		private SpriteCounter _rightPhotoCounter;

		private SpriteCounter _leftPhotoCounter;

		public override void Initialize(int playerIndex)
		{
			base.Initialize(playerIndex);
			CommonButtons = new CommonButtonObject[_positions.Length];
			CommonButtons[1] = _rightPhotoButton;
			CommonButtons[1].Initialize(playerIndex, InputManager.ButtonSetting.Button02, CommonButtonObject.LedColors.White);
			CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
			CommonButtons[2].Initialize(playerIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
			CommonButtons[2].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: false);
			CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
			CommonButtons[3].Initialize(playerIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).LedColor);
			CommonButtons[3].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Next).Image, isFlip: false);
			CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
			CommonButtons[4].Initialize(playerIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Cancel).LedColor);
			CommonButtons[4].SetSymbol(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Cancel).Image, isFlip: false);
			CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[5]);
			CommonButtons[5].Initialize(playerIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
			CommonButtons[5].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: true);
			CommonButtons[6] = _leftPhotoButton;
			CommonButtons[6].Initialize(playerIndex, InputManager.ButtonSetting.Button07, CommonButtonObject.LedColors.White);
			_rightPhotoCounter = _rightPhotoButton.transform.Find("Null_PhotoButton/IMG_Button_B/IMG_Button_A/NUM_PhotoNumber").GetComponent<SpriteCounter>();
			_leftPhotoCounter = _leftPhotoButton.transform.Find("Null_PhotoButton/IMG_Button_B/IMG_Button_A/NUM_PhotoNumber").GetComponent<SpriteCounter>();
		}

		public void ViewUpdate()
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			SyncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
			CommonButtonObject[] commonButtons = CommonButtons;
			foreach (CommonButtonObject commonButtonObject in commonButtons)
			{
				if (commonButtonObject != null)
				{
					commonButtonObject.ViewUpdate(SyncTimer);
				}
			}
			_editTouchButtonObject.ViewUpdate(SyncTimer);
			_zoomTouchButtonObject.ViewUpdate(SyncTimer);
			if (SyncTimer > 1f)
			{
				SyncTimer = 0f;
			}
		}

		public void SetPhotoCount(int leftCount, int rightCount)
		{
			_leftPhotoCounter.ChangeText(string.Concat(leftCount));
			_rightPhotoCounter.ChangeText(string.Concat(rightCount));
		}

		public void Pressed(InputManager.ButtonSetting buttonSetting)
		{
			if (CommonButtons[(int)buttonSetting] != null)
			{
				CommonButtons[(int)buttonSetting].Pressed();
			}
		}

		public void TouchEditButton()
		{
			_editTouchButtonObject.Pressed();
		}

		public void TouchZoomButton()
		{
			_zoomTouchButtonObject.Pressed();
		}

		public void SetVisibleEditButton(bool isVisible)
		{
			_editTouchButtonObject.SetActiveButton(isVisible);
		}

		public void SetVisibleZoomButton(bool isVisible)
		{
			_zoomTouchButtonObject.SetActiveButton(isVisible);
		}

		public void PressedChangeButton(bool on)
		{
			_ = base.gameObject.activeSelf;
		}

		public void SetVisibleChangeButton(bool isVisible)
		{
		}

		public void SetVisible(bool isVisible, InputManager.ButtonSetting buttonSetting)
		{
			if (CommonButtons[(int)buttonSetting] != null)
			{
				CommonButtons[(int)buttonSetting].SetActiveButton(isVisible);
				if (CommonButtons[(int)buttonSetting] is PhotoChangeButton)
				{
					((PhotoChangeButton)CommonButtons[(int)buttonSetting]).SetPressedOutFlag(!isVisible);
					Color32 color = CommonButtonObject.LedColors32(isVisible ? CommonButtonObject.LedColors.White : CommonButtonObject.LedColors.Black);
					MechaManager.LedIf[MonitorIndex].SetColorButton((byte)buttonSetting, color);
				}
			}
		}

		public void ChangeButtonSymbol(InputManager.ButtonSetting setting, PhotoEditProcess.SelectMode mod)
		{
			FlatButtonType type = FlatButtonType.Yes;
			int num = -1;
			switch (setting)
			{
			case InputManager.ButtonSetting.Button04:
				num = 3;
				switch (mod)
				{
				case PhotoEditProcess.SelectMode.MainMenu:
					type = FlatButtonType.Skip;
					break;
				case PhotoEditProcess.SelectMode.CustomEdit:
					type = FlatButtonType.Upload;
					break;
				case PhotoEditProcess.SelectMode.Confirmation:
					type = FlatButtonType.Agree;
					break;
				case PhotoEditProcess.SelectMode.Complete:
					type = FlatButtonType.Ok;
					break;
				case PhotoEditProcess.SelectMode.Unregistered:
					type = FlatButtonType.Ok;
					break;
				}
				break;
			case InputManager.ButtonSetting.Button05:
				num = 4;
				switch (mod)
				{
				case PhotoEditProcess.SelectMode.CustomEdit:
					type = FlatButtonType.Back;
					break;
				case PhotoEditProcess.SelectMode.Confirmation:
					type = FlatButtonType.Disagree;
					break;
				}
				break;
			}
			if (-1 != num)
			{
				CommonButtons[num].Initialize(MonitorIndex, setting, ButtonControllerBase.GetFlatButtonParam(type).LedColor);
				CommonButtons[num].SetSymbolSprite(ButtonControllerBase.GetFlatButtonParam(type).Image, isFlip: false);
			}
		}
	}
}
