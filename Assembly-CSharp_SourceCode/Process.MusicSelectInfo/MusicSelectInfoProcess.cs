using DB;
using MAI2.Util;
using Manager;
using Monitor.MusicSelectInfo;
using Process.CourseSelect;
using UnityEngine;

namespace Process.MusicSelectInfo
{
	public class MusicSelectInfoProcess : ProcessBase
	{
		private enum MusicSelectInfoState : byte
		{
			Wait,
			Disp,
			DispEnd,
			GotoEnd,
			Released
		}

		private MusicSelectInfoMonitor[] _monitors;

		private MusicSelectInfoState _state;

		private float _timeCounter;

		public const uint WindowTimeOut = 10000u;

		public const uint WindowInputWait = 1000u;

		public MusicSelectInfoProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/MusicSelectInfo/MusicSelectInfoProcess");
			_monitors = new MusicSelectInfoMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<MusicSelectInfoMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<MusicSelectInfoMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			_state = MusicSelectInfoState.Wait;
			if (_monitors[0].IsFinish() && _monitors[1].IsFinish())
			{
				_state = MusicSelectInfoState.GotoEnd;
			}
			else
			{
				container.processManager.NotificationFadeIn();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case MusicSelectInfoState.GotoEnd:
				_state = MusicSelectInfoState.Released;
				if (GameManager.IsCourseMode)
				{
					container.processManager.AddProcess(new CourseSelectProcess(container), 50);
				}
				else
				{
					container.processManager.AddProcess(new MusicSelectProcess(container), 50);
				}
				container.processManager.ReleaseProcess(this);
				break;
			case MusicSelectInfoState.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_state = MusicSelectInfoState.Disp;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						continue;
					}
					if (_monitors[k].IsFinish())
					{
						if (!Singleton<UserDataManager>.Instance.IsSingleUser())
						{
							container.processManager.EnqueueMessage(k, WindowMessageID.PlayPreparationWait);
						}
						_monitors[k].SetStatus(MusicSelectInfoMonitor.InfoStatus.Finish);
					}
					else
					{
						MusicSelectInfoMonitor.WindowDataSt nowMessageId = _monitors[k].GetNowMessageId();
						_monitors[k].Play();
						container.processManager.EnqueueMessage(k, nowMessageId.windowId);
						SoundManager.PlayVoice(nowMessageId.cue, k);
						SetInputLockInfo(k, 1000f);
					}
				}
				break;
			}
			case MusicSelectInfoState.Disp:
			{
				if (!IsNext())
				{
					break;
				}
				_state = MusicSelectInfoState.DispEnd;
				_timeCounter = 0f;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						container.processManager.CloseWindow(j);
					}
				}
				break;
			}
			case MusicSelectInfoState.DispEnd:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 1f)
				{
					_timeCounter = 0f;
					_state = MusicSelectInfoState.Released;
					if (GameManager.IsCourseMode)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new CourseSelectProcess(container), FadeProcess.FadeType.Type2), 50);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectProcess(container), FadeProcess.FadeType.Type2), 50);
					}
					container.processManager.SetVisibleTimers(isVisible: false);
					for (int i = 0; i < _monitors.Length; i++)
					{
						SoundManager.StopVoice(i);
					}
				}
				break;
			case MusicSelectInfoState.Released:
				break;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if ((int)_state < 1 || _monitors[monitorId].IsFinish() || !_monitors[monitorId].IsActive() || (!InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) && !_monitors[monitorId].TimeUp()))
			{
				return;
			}
			_monitors[monitorId].Next();
			if (_monitors[monitorId].IsFinish())
			{
				if (!Singleton<UserDataManager>.Instance.IsSingleUser())
				{
					container.processManager.EnqueueMessage(monitorId, WindowMessageID.PlayPreparationWait);
				}
				_monitors[monitorId].Stop();
			}
			else
			{
				MusicSelectInfoMonitor.WindowDataSt nowMessageId = _monitors[monitorId].GetNowMessageId();
				container.processManager.EnqueueMessage(monitorId, nowMessageId.windowId);
				SoundManager.PlayVoice(nowMessageId.cue, monitorId);
				_monitors[monitorId].PushButton();
				SetInputLockInfo(monitorId, 1000f);
			}
		}

		public override void OnLateUpdate()
		{
		}

		public bool IsNext()
		{
			if (_monitors[0].IsFinish())
			{
				return _monitors[1].IsFinish();
			}
			return false;
		}
	}
}
