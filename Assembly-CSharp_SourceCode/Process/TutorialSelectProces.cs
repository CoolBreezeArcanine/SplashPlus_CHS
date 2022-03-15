using System;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Monitor;
using Process.MusicSelectInfo;
using UnityEngine;

namespace Process
{
	public class TutorialSelectProces : ProcessBase
	{
		public enum TutorialSelectSequence
		{
			Init,
			Wait,
			DispWait,
			Disp,
			DispEnd,
			Release
		}

		private TutorialSelectSequence _state;

		private TutorialSelectMonitor[] _monitors;

		private float _timeCounter;

		private TutorialSelectMonitor.EntryStatus _entryStatus;

		public TutorialSelectProces(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/TutorialSelect/TutorialSelectProcess");
			_monitors = new TutorialSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TutorialSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TutorialSelectMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			_entryStatus = TutorialSelectMonitor.EntryStatus.None;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case TutorialSelectSequence.Init:
				_state = TutorialSelectSequence.Wait;
				break;
			case TutorialSelectSequence.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_timeCounter = 0f;
				_state = TutorialSelectSequence.DispWait;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						_monitors[k].Play();
					}
				}
				container.processManager.NotificationFadeIn();
				break;
			}
			case TutorialSelectSequence.DispWait:
			{
				if (!_monitors[0].IsStartDispEnd() || !_monitors[1].IsStartDispEnd())
				{
					break;
				}
				Action both = delegate
				{
					if (_entryStatus == TutorialSelectMonitor.EntryStatus.None)
					{
						_entryStatus = TutorialSelectMonitor.EntryStatus.NotTutorialSelect;
					}
				};
				container.processManager.PrepareTimer(10, 0, isEntry: false, both);
				_state = TutorialSelectSequence.Disp;
				_timeCounter = 0f;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						_monitors[i].Loop();
					}
				}
				break;
			}
			case TutorialSelectSequence.Disp:
			{
				if (_entryStatus == TutorialSelectMonitor.EntryStatus.None)
				{
					break;
				}
				_state = TutorialSelectSequence.DispEnd;
				_timeCounter = 0f;
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						if (_entryStatus == TutorialSelectMonitor.EntryStatus.BasicTutorialSelect)
						{
							SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000036, j);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, j);
							_monitors[j].SelectBasicTutorialSelect();
							GameManager.TutorialPlayed = GameManager.TutorialEnum.BasicPlay;
						}
						else if (_entryStatus == TutorialSelectMonitor.EntryStatus.NewTutorialSelect)
						{
							SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000036, j);
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, j);
							_monitors[j].SelectNewTutorialSelect();
							GameManager.TutorialPlayed = GameManager.TutorialEnum.NewPlay;
						}
						else if (_entryStatus == TutorialSelectMonitor.EntryStatus.NotTutorialSelect)
						{
							_monitors[j].SelectNotTutorialSelect();
							GameManager.TutorialPlayed = GameManager.TutorialEnum.NotPlay;
						}
						container.processManager.CloseWindow(j);
					}
				}
				break;
			}
			case TutorialSelectSequence.DispEnd:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 2f)
				{
					_timeCounter = 0f;
					_state = TutorialSelectSequence.Release;
					if (_entryStatus == TutorialSelectMonitor.EntryStatus.BasicTutorialSelect)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new TutorialProcess(container)), 50);
					}
					else if (_entryStatus == TutorialSelectMonitor.EntryStatus.NewTutorialSelect)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new TutorialProcess(container)), 50);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectInfoProcess(container)), 50);
					}
					_state = TutorialSelectSequence.Release;
					container.processManager.SetVisibleTimers(isVisible: false);
				}
				break;
			case TutorialSelectSequence.Release:
				return;
			}
			TutorialSelectMonitor[] monitors = _monitors;
			for (int l = 0; l < monitors.Length; l++)
			{
				monitors[l].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			TutorialSelectSequence state = _state;
			if (state == TutorialSelectSequence.Disp)
			{
				DetailInputOnDisp(monitorId);
			}
		}

		private void DetailInputOnDisp(int monitorId)
		{
			if (!_monitors[monitorId].IsSelected() && Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E7) || InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B7) || InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B6) || InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button06))
				{
					_entryStatus = TutorialSelectMonitor.EntryStatus.BasicTutorialSelect;
					container.processManager.ForceTimeUp();
				}
				else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E3) || InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B2) || InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B3) || InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button03))
				{
					_entryStatus = TutorialSelectMonitor.EntryStatus.NewTutorialSelect;
					container.processManager.ForceTimeUp();
				}
				else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
				{
					_entryStatus = TutorialSelectMonitor.EntryStatus.NotTutorialSelect;
					container.processManager.ForceTimeUp();
				}
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i].gameObject);
			}
		}
	}
}
