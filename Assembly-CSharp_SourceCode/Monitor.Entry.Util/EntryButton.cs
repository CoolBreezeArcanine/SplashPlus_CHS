using System;
using MAI2.Util;
using Manager;
using Monitor.MapCore;
using Monitor.MapCore.Component;
using UI;
using UnityEngine;

namespace Monitor.Entry.Util
{
	public class EntryButton : MapBehaviour
	{
		private EntryButtonUI _ui;

		private DelayComponent _delay;

		private int _cueIndex = -1;

		public Action<EntryButton> OnPress { get; set; }

		public Action<EntryButton> OnPush { get; set; }

		public Func<bool> IsValid { get; set; }

		public bool IsLock { get; set; }

		public bool IsPressed { get; private set; }

		public InputManager.ButtonSetting ButtonSetting => _ui.ButtonSetting;

		public CommonButtonObject.LedColors LedColor
		{
			get
			{
				if (IsPressed)
				{
					return CommonButtonObject.LedColors.White;
				}
				return _ui.LedColor;
			}
		}

		public Color32 LedColor32 => CommonButtonObject.LedColors32(LedColor);

		private void Awake()
		{
			_ui = GetComponent<EntryButtonUI>();
			_delay = base.gameObject.AddComponent<DelayComponent>();
		}

		public void Initialize(InputManager.ButtonSetting buttonSetting, Sprite sprite, CommonButtonObject.LedColors ledColor, int cueIndex)
		{
			_ui.Initialize(base.Monitor.MonitorIndex, buttonSetting, ledColor);
			_ui.SetSymbol(sprite, isFlip: false);
			_cueIndex = cueIndex;
		}

		public void Activate()
		{
			UpdateColor();
			_ui.SetActiveButton(isActive: true);
			_delay.StartDelay(0.3f, delegate
			{
				State = StateUpdate;
			});
		}

		public void Deactivate()
		{
			SetStateTerminate();
			_ui.SetActiveButton(isActive: false);
			_ui.ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
			_delay.StartDelay(0.2f, delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
		}

		private void StateUpdate(float deltaTime)
		{
			if (InputManager.GetButtonLongPush(base.Monitor.MonitorIndex, _ui.ButtonSetting, 250L))
			{
				IsPressed = true;
			}
			else
			{
				IsPressed = false;
			}
			if (!IsLock && UpdateColor() && InputManager.GetButtonDown(base.Monitor.MonitorIndex, _ui.ButtonSetting))
			{
				OnPress?.Invoke(this);
				_ui.Pressed();
				Singleton<SeManager>.Instance.PlaySE(_cueIndex, base.Monitor.MonitorIndex);
				_delay.StartDelay(0.4f, delegate
				{
					OnPush?.Invoke(this);
				});
			}
		}

		private bool UpdateColor()
		{
			bool flag = IsValid?.Invoke() ?? false;
			_ui.SetImageColor(flag ? Color.white : Color.gray);
			return flag;
		}

		public void SetSyncTimer(float timer)
		{
			_ui.ViewUpdate(timer);
		}
	}
}
