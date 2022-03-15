using System;
using System.Linq;
using AMDaemon;
using MAI2.Util;
using Manager;
using Monitor.Entry;
using Monitor.Entry.Parts;

namespace Process.Entry
{
	public class TryAime : EntrySubProcess
	{
		private enum ProcMode
		{
			Nop,
			WaitAnyRead,
			WaitAdvCheck,
			GetResult,
			Error,
			NextError,
			Suspend,
			DelayStep
		}

		private readonly Action<EntryMonitor> _onDone;

		private readonly Action<EntryMonitor> _onNewAime;

		private readonly Action<EntryMonitor> _onNewFelica;

		private readonly Action<EntryMonitor, bool> _onErrorResume;

		private readonly Action<bool> _onTimeSuspend;

		private readonly Action<EntryMonitor, uint> _onTest;

		public static uint TestAimeId;

		private ProcMode Mode { get; set; }

		public bool IsError
		{
			get
			{
				if (Mode != ProcMode.Error)
				{
					return Mode == ProcMode.NextError;
				}
				return true;
			}
		}

		public TryAime(EntryMonitor monitor, EntryMonitor subMonitor, Action<EntryMonitor> onDone, Action<EntryMonitor> onNewAime, Action<EntryMonitor> onNewFelica, Action<EntryMonitor, bool> onErrorResume, Action<bool> onTimerSuspend, Action<EntryMonitor, uint> onTest = null)
		{
			MainMonitor = monitor;
			SubMonitor = subMonitor;
			_onDone = onDone;
			_onNewAime = onNewAime;
			_onNewFelica = onNewFelica;
			_onErrorResume = onErrorResume;
			_onTimeSuspend = onTimerSuspend;
			_onTest = onTest;
			Mode = ProcMode.WaitAdvCheck;
		}

		public override void Execute()
		{
			if (IsFinish)
			{
				return;
			}
			if (TestAimeId != 0)
			{
				_onTest?.Invoke(MainMonitor, TestAimeId);
				IsFinish = true;
				return;
			}
			switch (Mode)
			{
			case ProcMode.WaitAdvCheck:
				if (!Singleton<OperationManager>.Instance.IsAliveAimeReader)
				{
					Mode = ProcMode.Nop;
				}
				else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.AdvCheck())
				{
					_onTimeSuspend(obj: true);
					Mode = ProcMode.WaitAnyRead;
				}
				break;
			case ProcMode.WaitAnyRead:
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.AnyRead())
				{
					MainMonitor.LockScreenButtons();
					SubMonitor?.LockScreenButtons();
					Mode = ProcMode.GetResult;
				}
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetResult() == AimeReaderManager.Result.Error)
				{
					_onTimeSuspend(obj: false);
					AimeErrorCategory errorCategory = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetErrorCategory();
					if (errorCategory == AimeErrorCategory.Fatal)
					{
						Singleton<OperationManager>.Instance.IsAliveAimeReader = false;
					}
					DispAimeError((int)errorCategory);
					Mode = ProcMode.Error;
				}
				break;
			case ProcMode.GetResult:
				switch (SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetResult())
				{
				case AimeReaderManager.Result.Done:
				{
					_onTimeSuspend(obj: false);
					ulong num = new EntryMonitor[2] { MainMonitor, SubMonitor }.Where((EntryMonitor i) => i != null).FirstOrDefault((EntryMonitor j) => j.IsDecide)?.UserId ?? 0;
					uint value = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetAimeId().Value;
					if (num == value)
					{
						DispAimeError(512);
						Mode = ProcMode.NextError;
					}
					else
					{
						_onDone(MainMonitor);
						IsFinish = true;
					}
					break;
				}
				case AimeReaderManager.Result.NewAime:
					_onTimeSuspend(obj: false);
					_onNewAime(MainMonitor);
					Mode = ProcMode.Suspend;
					break;
				case AimeReaderManager.Result.NewFelica:
					_onTimeSuspend(obj: false);
					_onNewFelica(MainMonitor);
					Mode = ProcMode.Suspend;
					break;
				case AimeReaderManager.Result.Error:
				{
					_onTimeSuspend(obj: false);
					AimeErrorCategory errorCategory2 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetErrorCategory();
					if (errorCategory2 == AimeErrorCategory.Fatal)
					{
						Singleton<OperationManager>.Instance.IsAliveAimeReader = false;
					}
					DispAimeError((int)errorCategory2);
					Mode = ProcMode.Error;
					break;
				}
				}
				break;
			case ProcMode.Error:
				if (InputResponse() != 0)
				{
					_onErrorResume(MainMonitor, SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.GetErrorCategory() == AimeErrorCategory.Fatal);
					IsFinish = true;
				}
				break;
			case ProcMode.NextError:
				if (InputResponse() != 0)
				{
					_onErrorResume(MainMonitor, arg2: false);
					IsFinish = true;
				}
				break;
			case ProcMode.DelayStep:
				Mode = ProcMode.WaitAdvCheck;
				break;
			case ProcMode.Suspend:
				break;
			}
		}

		public void Resume()
		{
			Mode = ProcMode.DelayStep;
		}

		public void Discard()
		{
			_onTimeSuspend(obj: false);
			IsFinish = true;
		}

		private void DispAimeError(int code)
		{
			MainMonitor.OpenScreen(ScreenType.ErrorAime, code);
			MainMonitor.ResetResponse();
			if (SubMonitor != null)
			{
				SubMonitor.OpenScreen(ScreenType.ErrorAime, code);
				SubMonitor.ResetResponse();
			}
		}
	}
}
