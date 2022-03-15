using System.Diagnostics;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using UnityEngine;

namespace Monitor.TakeOver
{
	public class TakeOverMonitor : MonitorBase
	{
		public enum DispState
		{
			Init,
			Info01FadeIn,
			Info01FadeInWait,
			Info01Wait,
			Info01FadeOut,
			Info02FadeIn,
			Info02FadeInWait,
			Info02Wait,
			Info02FadeOut,
			AnotherWait,
			EndWait,
			End
		}

		public enum MajorRomVersion
		{
			NOTHING = 0,
			NONE = 1,
			NONE_PLUS = 1010000,
			SPLASH = 1014000,
			SPLASH_PLUS = 1017000,
			NOW = 1017000
		}

		public enum MinorRomVersion
		{
			NOTHING = 0,
			NONE = 1,
			NONE_PLUS = 1010000,
			SPLASH = 1014000,
			SPLASH_PLUS = 1017000
		}

		private DispState state = DispState.End;

		private Stopwatch timer = new Stopwatch();

		private ProcessManager _processManager;

		[SerializeField]
		private TakeOverButtonController _buttonController;

		[SerializeField]
		[Header("１ウィンドウ辺りの待ち時間(ms)")]
		private int phaseWaitTime;

		[SerializeField]
		[Header("最終待機時間(ms)")]
		private int phaseEndTime;

		private int _timer;

		public uint _rom_version;

		public MajorRomVersion _major_version = MajorRomVersion.NONE;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			if (!active)
			{
				state = DispState.AnotherWait;
				return;
			}
			state = DispState.Init;
			_buttonController.Initialize(base.MonitorIndex);
			_buttonController.SetVisibleImmediate(false, 3);
			timer.Restart();
		}

		public void SetProcessManager(ProcessManager processManager)
		{
			_processManager = processManager;
		}

		public override void ViewUpdate()
		{
			if (_timer > 0)
			{
				_timer--;
			}
			switch (state)
			{
			case DispState.Init:
				timer.Restart();
				state = DispState.Info01FadeIn;
				break;
			case DispState.Info01FadeIn:
				switch (_major_version)
				{
				case MajorRomVersion.NOTHING:
					_processManager.EnqueueMessage(base.MonitorIndex, WindowMessageID.TakeOverInfo01);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000015, base.MonitorIndex);
					break;
				default:
					_processManager.EnqueueMessage(base.MonitorIndex, WindowMessageID.TransferDx01);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_INFO_NORMAL, base.MonitorIndex);
					break;
				}
				timer.Restart();
				_timer = 60;
				state = DispState.Info01FadeInWait;
				break;
			case DispState.Info01FadeInWait:
				if (_timer == 0)
				{
					_buttonController.Initialize(base.MonitorIndex);
					_buttonController.SetVisible(true, 3);
					timer.Restart();
					state = DispState.Info01Wait;
				}
				break;
			case DispState.Info01Wait:
				if (timer.ElapsedMilliseconds > phaseWaitTime || InputManager.GetButtonDown(base.MonitorIndex, InputManager.ButtonSetting.Button04))
				{
					_buttonController.SetAnimationActive(3);
					timer.Restart();
					state = DispState.Info01FadeOut;
					_timer = 30;
				}
				break;
			case DispState.Info01FadeOut:
				if (_timer == 0)
				{
					_buttonController.SetVisible(false, 3);
					timer.Restart();
					state = DispState.Info02FadeIn;
				}
				break;
			case DispState.Info02FadeIn:
				switch (_major_version)
				{
				case MajorRomVersion.NOTHING:
					_processManager.EnqueueMessage(base.MonitorIndex, WindowMessageID.TakeOverInfo02);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000016, base.MonitorIndex);
					break;
				default:
					_processManager.EnqueueMessage(base.MonitorIndex, WindowMessageID.TransferDx02);
					break;
				}
				timer.Restart();
				_timer = 60;
				state = DispState.Info02FadeInWait;
				break;
			case DispState.Info02FadeInWait:
				if (_timer == 0)
				{
					_buttonController.SetVisible(true, 3);
					timer.Restart();
					state = DispState.Info02Wait;
				}
				break;
			case DispState.Info02Wait:
				if (timer.ElapsedMilliseconds > phaseWaitTime || InputManager.GetButtonDown(base.MonitorIndex, InputManager.ButtonSetting.Button04))
				{
					_buttonController.SetAnimationActive(3);
					timer.Restart();
					state = DispState.Info02FadeOut;
					_timer = 30;
				}
				break;
			case DispState.Info02FadeOut:
				if (_timer == 0)
				{
					_buttonController.SetVisible(false, 3);
					timer.Restart();
					state = DispState.AnotherWait;
				}
				break;
			case DispState.AnotherWait:
				if (Singleton<UserDataManager>.Instance.IsSingleUser())
				{
					_processManager.CloseWindow(base.MonitorIndex);
				}
				else
				{
					_processManager.EnqueueMessage(base.MonitorIndex, WindowMessageID.PlayPreparationWait);
				}
				state = DispState.EndWait;
				timer.Restart();
				break;
			case DispState.EndWait:
				if (timer.ElapsedMilliseconds > phaseEndTime)
				{
					state = DispState.End;
				}
				break;
			case DispState.End:
				break;
			}
		}

		public bool IsState(DispState _state)
		{
			return state == _state;
		}
	}
}
