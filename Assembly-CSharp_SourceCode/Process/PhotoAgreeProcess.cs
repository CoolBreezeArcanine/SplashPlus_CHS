using System;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Monitor.PhotoAgree;
using Process.CourseSelect;
using Process.Information;
using Process.MusicSelectInfo;
using UnityEngine;

namespace Process
{
	public class PhotoAgreeProcess : ProcessBase
	{
		private enum PhotoAgreeState : byte
		{
			Wait,
			Disp,
			DispEnd,
			GotoEnd,
			Released
		}

		private PhotoAgreeMonitor[] _monitors;

		private PhotoAgreeState _state;

		private PhotoAgreeMonitor.EntryStatus _entryStatus;

		private float _timeCounter;

		public PhotoAgreeProcess(ProcessDataContainer dataContainer)
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
				UnityEngine.Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/PhotoAgree/PhotoAgreeProcess");
			_monitors = new PhotoAgreeMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<PhotoAgreeMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<PhotoAgreeMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			_state = PhotoAgreeState.GotoEnd;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case PhotoAgreeState.GotoEnd:
				_timeCounter = 0f;
				_state = PhotoAgreeState.Released;
				if (GameManager.IsGotoCharacterSelect)
				{
					container.processManager.AddProcess(new CharacterSelectProces(container), 50);
				}
				else if (GameManager.MusicTrackNumber == 1)
				{
					if (!Singleton<UserDataManager>.Instance.IsPlayCountEnouth(3))
					{
						container.processManager.AddProcess(new TutorialSelectProces(container), 50);
					}
					else
					{
						container.processManager.AddProcess(new GetMusicProcess(container), 50);
					}
				}
				else
				{
					container.processManager.AddProcess(new MusicSelectInfoProcess(container), 50);
				}
				container.processManager.ReleaseProcess(this);
				break;
			case PhotoAgreeState.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				container.processManager.SetVisibleTimers(isVisible: false);
				_state = PhotoAgreeState.Disp;
				Action both = delegate
				{
					if (_entryStatus == PhotoAgreeMonitor.EntryStatus.None)
					{
						_entryStatus = PhotoAgreeMonitor.EntryStatus.Disagree;
					}
				};
				for (int j = 0; j < _monitors.Length; j++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						container.processManager.EnqueueMessage(j, WindowMessageID.PhotoAgree);
						_monitors[j].Play();
					}
				}
				container.processManager.PrepareTimer(10, 0, isEntry: false, both);
				break;
			}
			case PhotoAgreeState.Disp:
			{
				if (_entryStatus == PhotoAgreeMonitor.EntryStatus.None)
				{
					break;
				}
				_state = PhotoAgreeState.DispEnd;
				_timeCounter = 0f;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						SoundManager.PlaySE(Cue.SE_SYS_FIX, i);
						container.processManager.CloseWindow(i);
						_monitors[i].Stop();
						if (_entryStatus == PhotoAgreeMonitor.EntryStatus.Agree)
						{
							_monitors[i].Agree();
						}
						else
						{
							_monitors[i].Disagree();
						}
					}
				}
				break;
			}
			case PhotoAgreeState.DispEnd:
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_timeCounter = 0f;
				_state = PhotoAgreeState.Released;
				if (GameManager.IsEventMode)
				{
					if (GameManager.IsCourseMode)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new CourseSelectProcess(container), FadeProcess.FadeType.Type2), 50);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectProcess(container), FadeProcess.FadeType.Type2), 50);
					}
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new InformationProcess(container)), 50);
				}
				container.processManager.SetVisibleTimers(isVisible: false);
				break;
			case PhotoAgreeState.Released:
				break;
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if ((int)_state >= 1 && !_monitors[monitorId].IsSelected() && Singleton<UserDataManager>.Instance.GetUserData(monitorId).IsEntry)
			{
				if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
				{
					_entryStatus = PhotoAgreeMonitor.EntryStatus.Agree;
					container.processManager.ForceTimeUp();
				}
				else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
				{
					_entryStatus = PhotoAgreeMonitor.EntryStatus.Disagree;
					container.processManager.ForceTimeUp();
				}
			}
		}

		public override void OnLateUpdate()
		{
		}
	}
}
