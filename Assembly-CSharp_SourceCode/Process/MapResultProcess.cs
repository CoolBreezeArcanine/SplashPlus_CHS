using DB;
using MAI2.Util;
using Manager;
using Monitor;
using UnityEngine;

namespace Process
{
	public class MapResultProcess : ProcessBase
	{
		private enum ProcessState : byte
		{
			Wait,
			Disp,
			Released
		}

		private MapResultMonitor[] _monitors;

		private ProcessState _state;

		private bool _isExistReleaseMap;

		private bool _isExistNewCharacter;

		public MapResultProcess(ProcessDataContainer dataContainer)
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
			if (GameManager.IsFreedomMode)
			{
				GameManager.PauseFreedomModeTimer(isPause: true);
			}
			GameObject prefs = Resources.Load<GameObject>("Process/MapResult/MapResultProcess");
			_monitors = new MapResultMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<MapResultMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<MapResultMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].SetAssetManager(container.assetManager);
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (userData.IsEntry && userData.IsGuest())
				{
					container.processManager.EnqueueMessage(j, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case ProcessState.Wait:
				_state = ProcessState.Disp;
				if (GameManager.IsFreedomMode)
				{
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20003, true));
					if (!GameManager.IsFreedomTimeUp)
					{
						container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20002, true));
					}
				}
				break;
			case ProcessState.Disp:
			{
				if (_monitors[0].IsEnd() && _monitors[1].IsEnd())
				{
					_state = ProcessState.Released;
					for (int i = 0; i < _monitors.Length; i++)
					{
						SoundManager.StopBGM(i);
						SoundManager.StopLoopSe(i);
						if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && !Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
						{
							if (_monitors[i].IsReleaseNewMap())
							{
								_isExistReleaseMap = true;
								break;
							}
							if (_monitors[i].IsGetNewCharacter())
							{
								_isExistNewCharacter = true;
							}
						}
					}
					for (int j = 0; j < _monitors.Length; j++)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
						if (userData.IsEntry && userData.IsGuest())
						{
							container.processManager.CloseWindow(j);
						}
						else if (container.processManager.IsOpening(j, WindowPositionID.Middle))
						{
							container.processManager.CloseWindow(j);
						}
					}
					if (_isExistReleaseMap || _isExistNewCharacter)
					{
						if (_isExistReleaseMap)
						{
							GameManager.NextMapSelect = true;
						}
						else if (_isExistNewCharacter)
						{
							GameManager.NextCharaSelect = true;
						}
						container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
					}
					else
					{
						container.processManager.AddProcess(new NextTrackProcess(container, this), 50);
					}
					break;
				}
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (_monitors[k].IsEnd() && !container.processManager.IsOpening(k, WindowPositionID.Middle))
					{
						UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(k);
						if (userData2.IsEntry && !userData2.IsGuest())
						{
							_monitors[k].SetLastBlur();
							container.processManager.EnqueueMessage(k, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
					}
				}
				break;
			}
			}
			for (int l = 0; l < _monitors.Length; l++)
			{
				MapResultMonitor mapResultMonitor = _monitors[l];
				if (mapResultMonitor._isDispInfoWindow1 || mapResultMonitor._isDispInfoWindow2)
				{
					if (mapResultMonitor._info_timer != 0)
					{
						mapResultMonitor._info_timer--;
					}
					WindowMessageID messageId = WindowMessageID.CharaAwakeFirst;
					uint info_timer = 180u;
					uint info_timer2 = 420u;
					if (mapResultMonitor._isDispInfoWindow1)
					{
						messageId = WindowMessageID.MapCounterStop;
						info_timer = 120u;
						info_timer2 = 420u;
					}
					if (mapResultMonitor._isDispInfoWindow2)
					{
						messageId = WindowMessageID.CharaAwakeFirst;
						info_timer = 180u;
						info_timer2 = 420u;
					}
					switch (mapResultMonitor._info_state)
					{
					case MapResultMonitor.InfoWindowState.None:
						mapResultMonitor.SetBlur();
						mapResultMonitor.ResetSkipStart();
						if (!container.processManager.IsOpening(l, WindowPositionID.Middle))
						{
							container.processManager.EnqueueMessage(l, messageId, WindowPositionID.Middle);
							mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.Open;
						}
						break;
					case MapResultMonitor.InfoWindowState.Open:
						mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.OpenWait;
						mapResultMonitor._info_timer = info_timer;
						if (!mapResultMonitor._isInfoWindowVoice)
						{
							mapResultMonitor._isInfoWindowVoice = true;
						}
						break;
					case MapResultMonitor.InfoWindowState.OpenWait:
						if (mapResultMonitor._info_timer == 0)
						{
							mapResultMonitor.SkipStart();
							mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.Wait;
							mapResultMonitor._info_timer = info_timer2;
						}
						break;
					case MapResultMonitor.InfoWindowState.Wait:
						if (mapResultMonitor._isSkip)
						{
							mapResultMonitor.ResetSkipStart();
							mapResultMonitor._info_timer = 0u;
						}
						if (mapResultMonitor._info_timer == 0)
						{
							mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.Close;
						}
						break;
					case MapResultMonitor.InfoWindowState.Close:
						container.processManager.CloseWindow(l);
						if (mapResultMonitor._isEntry && !mapResultMonitor._isGuest && mapResultMonitor._isDispInfoWindow2)
						{
							Singleton<UserDataManager>.Instance.GetUserData(mapResultMonitor.MonitorIndex).Detail.ContentBit.SetFlag(ContentBitID.FirstCharaAwake, flag: true);
						}
						mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.CloseWait;
						break;
					case MapResultMonitor.InfoWindowState.CloseWait:
						mapResultMonitor._info_state = MapResultMonitor.InfoWindowState.End;
						if (mapResultMonitor._isDispInfoWindow1)
						{
							mapResultMonitor._isDispInfoWindow1 = false;
						}
						if (mapResultMonitor._isDispInfoWindow2)
						{
							mapResultMonitor._isDispInfoWindow2 = false;
						}
						mapResultMonitor.ResetBlur();
						break;
					}
				}
				_monitors[l].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
			if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				_monitors[monitorId].SkipAnim(InputManager.ButtonSetting.Button04);
			}
		}

		public override void OnLateUpdate()
		{
		}
	}
}
