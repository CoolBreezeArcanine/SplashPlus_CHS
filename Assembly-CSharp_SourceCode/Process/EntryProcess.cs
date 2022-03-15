using System;
using System.Collections.Generic;
using System.Linq;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using MAI2System;
using Manager;
using Mecha;
using Monitor;
using Monitor.Entry;
using Monitor.TakeOver;
using Net;
using Process.Entry.State;
using Process.MapCore;
using Process.TakeOver;
using UnityEngine;

namespace Process
{
	public class EntryProcess : MapProcess
	{
		private List<EntryMonitor> _monitors;

		private ContextEntry _context;

		private EntryAgingProcess _aging;

		public EntryProcess(ProcessDataContainer container)
			: base(container)
		{
		}

		public override void OnStart()
		{
			SoundManager.ResetMasterVolume();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.ClearPaydCost();
			GameObject original = Resources.Load<GameObject>("Process/Entry/EntryProcess");
			Transform[] array = new Transform[2] { container.LeftMonitor, container.RightMonitor };
			for (int j = 0; j < array.Length; j++)
			{
				EntryMonitor component = UnityEngine.Object.Instantiate(original, array[j], worldPositionStays: false).GetComponent<EntryMonitor>();
				component.Initialize(this, j, active: true);
				Monitors.Add(component);
			}
			_monitors = Monitors.OfType<EntryMonitor>().ToList();
			_monitors[0].PostInitialize(_monitors[1]);
			_monitors[1].PostInitialize(_monitors[0]);
			base.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 30000));
			base.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 50000));
			base.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 50002, CommonMonitor.SkyDaylight.MorningNow));
			foreach (EntryMonitor monitor in _monitors)
			{
				base.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, monitor.MonitorIndex, 1));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 45002));
			}
			WebCamManager.ClearnUpFolder();
			SoundManager.StopBGM(2);
			SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 2);
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000005, 0);
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000005, 1);
			Action both = delegate
			{
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.OnCoinIn != null)
				{
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.OnCoinIn = null;
					base.ProcessManager.SetVisibleTimers(isVisible: false);
					_context.SetState(StateType.DoneEntry);
					SetStateTerminate();
				}
			};
			base.ProcessManager.PrepareTimer(99, 0, isEntry: true, both);
			base.ProcessManager.NotificationFadeIn();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SetCoinCostInit();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.OnCoinIn = delegate
			{
				base.ProcessManager.PrepareTimer(99, 0, isEntry: true, both);
			};
			bool[] array2 = new bool[2]
			{
				GameManager.IsSelectContinue[0],
				GameManager.IsSelectContinue[1]
			};
			GameManager.Initialize();
			GameManager.IsSelectContinue[0] = array2[0];
			GameManager.IsSelectContinue[1] = array2[1];
			Singleton<NetDataManager>.Instance.Initialize();
			Singleton<GhostManager>.Instance.ResetGhostServerData();
			Singleton<CourseManager>.Instance.Initialize();
			Singleton<TicketManager>.Instance.Initialize();
			GameManager.IsCourseMode = false;
			GameManager.SpecialKindNum = GameManager.SpecialKind.None;
			foreach (EntryMonitor monitor2 in _monitors)
			{
				if (!GameManager.IsSelectContinue[monitor2.MonitorIndex] || Singleton<UserDataManager>.Instance.GetUserData(monitor2.MonitorIndex).IsGuest())
				{
					Singleton<UserDataManager>.Instance.GetUserData(monitor2.MonitorIndex).Initialize();
					Singleton<UserDataManager>.Instance.SetDefault(monitor2.MonitorIndex);
				}
				else
				{
					Singleton<UserDataManager>.Instance.GetUserData(monitor2.MonitorIndex).IsEntry = false;
				}
			}
			_context = new ContextEntry(this, _monitors[0], _monitors[1]);
			_context.AddState(StateType.ConfirmEntry, new ConfirmEntry());
			_context.AddState(StateType.ConfirmGuest, new ConfirmGuest());
			_context.AddState(StateType.ConfirmNewUser, new ConfirmNewUser());
			_context.AddState(StateType.ConfirmPlay, new ConfirmPlay());
			_context.AddState(StateType.ConfirmFreedom, new ConfirmFreedom());
			_context.AddState(StateType.ConfirmFreedomOne, new ConfirmFreedomOne());
			_context.AddState(StateType.ConfirmAccessCode, new ConfirmAccessCode());
			_context.AddState(StateType.ConfirmContinue, new ConfirmContinue());
			_context.AddState(StateType.ConfirmContinueOne, new ConfirmContinueOne());
			_context.AddState(StateType.DoneEntry, new DoneEntry());
			EntryMonitor[] array3 = _monitors.Where((EntryMonitor i) => !UserID.IsGuest(Singleton<UserDataManager>.Instance.GetUserData(i.MonitorIndex).Detail.UserID)).ToArray();
			switch (array3.Length)
			{
			case 2:
				_context.SetState(StateType.ConfirmContinue);
				break;
			case 1:
				_context.SetState(StateType.ConfirmContinueOne, array3.First().MonitorIndex, ConfirmContinueOne.InitType.Normal);
				break;
			default:
				_context.SetState(StateType.ConfirmEntry, false);
				break;
			}
			Bd15070_4IF[] ledIf = MechaManager.LedIf;
			foreach (Bd15070_4IF obj in ledIf)
			{
				obj.ButtonLedReset();
				obj.SetColorMultiFet(Bd15070_4IF.BodyBrightOutGame);
			}
			IsTimeCounting(isTimeCount: false);
			Singleton<OperationManager>.Instance.AutomaticDownload = false;
			if (Singleton<SystemConfig>.Instance.config.IsAutoPlay)
			{
				_aging = new EntryAgingProcess(container, this);
				base.ProcessManager.AddProcess(_aging, 19);
			}
			GC.Collect();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_context.Execute((float)GameManager.GetGameMSecAdd() / 1000f);
		}

		public override void OnRelease()
		{
			base.OnRelease();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.OnCoinIn = null;
			base.ProcessManager.ClearTimeoutAction();
			if (_aging != null)
			{
				base.ProcessManager.ReleaseProcess(_aging);
			}
		}

		public string GetContextName()
		{
			return _context.ToString();
		}

		public void GetCurrentState(out StateType type, out int mode)
		{
			_context.GetCurrentState(out type, out mode);
		}

		public void IsTimeCounting(bool isTimeCount)
		{
			base.ProcessManager.IsTimeCounting(isTimeCount);
		}

		public void DecrementTimerSecond(int decrementValue)
		{
			for (int i = 0; i < _monitors.Count; i++)
			{
				base.ProcessManager.DecrementTime(i, decrementValue);
			}
		}

		public void PlayGameStartSE()
		{
			foreach (EntryMonitor monitor in _monitors)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(monitor.MonitorIndex).IsEntry)
				{
					Singleton<SeManager>.Instance.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, monitor.MonitorIndex);
				}
			}
		}

		public void IsFastSkip(bool isFastSkip)
		{
			for (int i = 0; i < _monitors.Count; i++)
			{
				base.ProcessManager.IsFastSkip(i, isFastSkip);
			}
		}

		public void SetNextProcess()
		{
			if (_monitors.Any((EntryMonitor m) => Singleton<UserDataManager>.Instance.GetUserData(m.MonitorIndex).IsEntry))
			{
				GameManager.AutoPlay = GameManager.AutoPlayMode.None;
				uint versionCode = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionCode;
				uint[] array = new uint[2];
				bool[] array2 = new bool[2];
				bool[] array3 = new bool[2];
				bool flag = false;
				bool flag2 = false;
				TakeOverMajorVersion takeOverMajorVersion = new TakeOverMajorVersion();
				for (int i = 0; i < _monitors.Count; i++)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						continue;
					}
					VersionNo versionNo = default(VersionNo);
					if (_monitors[i].RomDataVersion == null)
					{
						versionNo.tryParse("", setZeroIfFailed: true);
					}
					else
					{
						versionNo.tryParse(_monitors[i].RomDataVersion, setZeroIfFailed: true);
					}
					array[i] = versionNo.versionCode;
					if (array[i] != 0 && array[i] < versionCode)
					{
						TakeOverMonitor.MajorRomVersion majorRomVersion = takeOverMajorVersion.GetMajorRomVersion(array[i]);
						TakeOverMonitor.MajorRomVersion majorRomVersion2 = takeOverMajorVersion.GetMajorRomVersion(versionCode);
						if (majorRomVersion < majorRomVersion2)
						{
							flag = true;
							array2[i] = true;
						}
						else
						{
							flag2 = true;
							array3[i] = true;
						}
					}
				}
				if (!flag && !flag2)
				{
					if (_monitors.Select((EntryMonitor m) => Singleton<UserDataManager>.Instance.GetUserData(m.MonitorIndex)).Any((UserData d) => d.IsEntry && d.UserType == UserData.UserIDType.Inherit))
					{
						base.ProcessManager.ReleaseProcess(this);
						base.ProcessManager.AddProcess(new TakeOverProcess(container), 50);
						base.ProcessManager.AddProcess(new PleaseWaitProcess(container), 50);
					}
					else if (_monitors.Select((EntryMonitor m) => Singleton<UserDataManager>.Instance.GetUserData(m.MonitorIndex)).Any((UserData d) => d.IsEntry && d.UserType == UserData.UserIDType.New))
					{
						base.ProcessManager.ReleaseProcess(this);
						base.ProcessManager.AddProcess(new NameEntryProcess(container), 50);
						base.ProcessManager.AddProcess(new PleaseWaitProcess(container), 50);
					}
					else
					{
						base.ProcessManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container)), 50);
						base.ProcessManager.AddProcess(new PleaseWaitProcess(container), 50);
					}
				}
				else
				{
					base.ProcessManager.ReleaseProcess(this);
					base.ProcessManager.AddProcess(new TakeOverProcess(container, array[0], array2[0], array3[0], array[1], array2[1], array3[1]), 50);
					base.ProcessManager.AddProcess(new PleaseWaitProcess(container), 50);
				}
				foreach (EntryMonitor monitor in _monitors)
				{
					int monitorIndex = monitor.MonitorIndex;
					if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
					{
						Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Initialize();
						Singleton<UserDataManager>.Instance.SetDefault(monitorIndex);
					}
				}
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			}
			else
			{
				base.ProcessManager.AddProcess(new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
			}
			base.ProcessManager.SetVisibleTimers(isVisible: false);
			base.ProcessManager.ClearTimeoutAction();
		}

		public void CreateDownloadProcess()
		{
			base.ProcessManager.AddProcess(new UserDataDLProcess(container), 50);
		}
	}
}
