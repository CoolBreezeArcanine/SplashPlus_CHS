using System.Collections.Generic;
using System.Linq;
using IO;
using Manager;
using Monitor.Entry.Util;
using Monitor.MapCore;
using UI;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class ButtonManager : MapBehaviour
	{
		private class Container
		{
			public readonly InputManager.ButtonSetting ButtonSetting;

			public readonly string SpritePath;

			public readonly CommonButtonObject.LedColors LedColor;

			public Sprite Sprite;

			public readonly int CueIndex;

			public Container(InputManager.ButtonSetting setting, ButtonControllerBase.ButtonInformation buttonInfo)
			{
				ButtonSetting = setting;
				SpritePath = buttonInfo.FileName;
				LedColor = buttonInfo.LedColor;
				CueIndex = (int)buttonInfo.Cue;
			}
		}

		[SerializeField]
		private Transform[] _layouts = new Transform[9];

		[SerializeField]
		private GameObject _buttonPrefab;

		private readonly List<EntryButton> _buttons = new List<EntryButton>();

		private CommonButtonObject.LedColors[] _cacheLedColors;

		private float _syncTimer;

		private readonly Dictionary<ButtonType, Container> _containers = new Dictionary<ButtonType, Container>
		{
			[ButtonType.AccessCode] = new Container(InputManager.ButtonSetting.Button07, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Accesscode)),
			[ButtonType.Freedom] = new Container(InputManager.ButtonSetting.Button02, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Freedom)),
			[ButtonType.GuestPlay] = new Container(InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.GuestPlay)),
			[ButtonType.Yes] = new Container(InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Yes)),
			[ButtonType.No] = new Container(InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.No)),
			[ButtonType.Agree] = new Container(InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Agree)),
			[ButtonType.Disagree] = new Container(InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Disagree)),
			[ButtonType.Skip] = new Container(InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Skip)),
			[ButtonType.TimeSkip] = new Container(InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.TimeSkip)),
			[ButtonType.Cancel] = new Container(InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Cancel)),
			[ButtonType.Return] = new Container(InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Back)),
			[ButtonType.Entry] = new Container(InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(ButtonControllerBase.FlatButtonType.Entry))
		};

		public void Initialize()
		{
			foreach (Container value in _containers.Values)
			{
				value.Sprite = Resources.Load<Sprite>(value.SpritePath);
			}
			foreach (Transform item in _layouts.Where((Transform l) => l != null))
			{
				Object.Instantiate(Resources.Load<GameObject>("CMN_DecideButton/FX_CMN_DecideButton_v110"), item, worldPositionStays: false);
			}
			_cacheLedColors = Enumerable.Repeat(CommonButtonObject.LedColors.Black, 9).ToArray();
			State = StateUpdate;
		}

		public EntryButton CreateButton(ButtonType type, bool isGrayButton = false)
		{
			Transform parent = _layouts[(int)_containers[type].ButtonSetting];
			EntryButton component = Object.Instantiate(_buttonPrefab, parent, worldPositionStays: false).GetComponent<EntryButton>();
			component.transform.SetAsLastSibling();
			CommonButtonObject.LedColors ledColor = _containers[type].LedColor;
			if (isGrayButton)
			{
				ledColor = CommonButtonObject.LedColors.Black;
			}
			component.Initialize(_containers[type].ButtonSetting, _containers[type].Sprite, ledColor, _containers[type].CueIndex);
			_buttons.Add(component);
			return component;
		}

		private void StateUpdate(float deltaTime)
		{
			_syncTimer += deltaTime;
			_buttons.RemoveAll((EntryButton b) => b == null);
			_buttons.ForEach(delegate(EntryButton b)
			{
				b.SetSyncTimer(_syncTimer);
			});
			if (_syncTimer > 1f)
			{
				_syncTimer = 0f;
			}
			UpdateLedColors();
		}

		private void UpdateLedColors()
		{
			EntryButton[] array = new EntryButton[9];
			foreach (EntryButton button in _buttons)
			{
				int buttonSetting = (int)button.ButtonSetting;
				if (!(array[buttonSetting] != null) || array[buttonSetting].LedColor != CommonButtonObject.LedColors.White)
				{
					array[buttonSetting] = button;
				}
			}
			for (int i = 0; i < 9; i++)
			{
				if (i != 8)
				{
					CommonButtonObject.LedColors ledColors = array[i]?.LedColor ?? CommonButtonObject.LedColors.Black;
					if (_cacheLedColors[i] != ledColors)
					{
						MechaManager.LedIf[base.Monitor.MonitorIndex].SetColorButton((byte)i, array[i]?.LedColor32 ?? ((Color32)Color.black));
						_cacheLedColors[i] = ledColors;
					}
				}
			}
		}
	}
}
