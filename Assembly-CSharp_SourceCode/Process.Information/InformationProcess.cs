using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Monitor.Information;
using Process.ModeSelect;
using UnityEngine;
using Util;

namespace Process.Information
{
	public class InformationProcess : ProcessBase
	{
		private enum InformationState : byte
		{
			Wait,
			Disp,
			Released,
			GotoEnd
		}

		private InformationMonitor[] _monitors;

		private InformationState _state;

		private readonly List<InformationData>[] _infoList = new List<InformationData>[2];

		public InformationProcess(ProcessDataContainer dataContainer)
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
				container.processManager.ForcedCloseWindow(i);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Information/InformationProcess");
			_monitors = new InformationMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<InformationMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<InformationMonitor>()
			};
			if (!GameManager.IsEventMode)
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
					_infoList[i] = new List<InformationData>();
					_infoList[i].Clear();
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && !Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
					{
						Safe.ReadonlySortedDictionary<int, InformationData> informations = Singleton<DataManager>.Instance.GetInformations();
						long unixTime = TimeManager.GetUnixTime(Singleton<UserDataManager>.Instance.GetUserData(i).Detail.EventWatchedDate);
						foreach (KeyValuePair<int, InformationData> item in informations)
						{
							if (!item.Value.disable && Singleton<EventManager>.Instance.IsOpenEvent(item.Value.eventName.id) && Singleton<EventManager>.Instance.IsDateNewAndOpen(item.Value.eventName.id, unixTime))
							{
								_infoList[i].Add(item.Value);
							}
						}
						_monitors[i].SetInfoList(_infoList[i]);
					}
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && _infoList[i].Count == 0 && !Singleton<UserDataManager>.Instance.IsSingleUser())
					{
						container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					}
				}
			}
			if (GameManager.IsEventMode)
			{
				_state = InformationState.GotoEnd;
			}
			else if (_infoList[0].Count == 0 && _infoList[1].Count == 0)
			{
				_state = InformationState.GotoEnd;
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
			case InformationState.GotoEnd:
				_state = InformationState.Released;
				container.processManager.AddProcess(new ModeSelectProcess(container), 50);
				container.processManager.ReleaseProcess(this);
				break;
			case InformationState.Wait:
				_state = InformationState.Disp;
				break;
			case InformationState.Disp:
				if (_monitors[0].IsState(InformationMonitor.DispState.End) && _monitors[1].IsState(InformationMonitor.DispState.End))
				{
					_state = InformationState.Released;
					container.processManager.AddProcess(new FadeProcess(container, this, new ModeSelectProcess(container)), 50);
				}
				break;
			}
			InformationMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
		}

		public override void OnLateUpdate()
		{
			if (Singleton<UserDataManager>.Instance.IsSingleUser())
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i].IsState(InformationMonitor.DispState.EndWait))
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
			}
		}
	}
}
