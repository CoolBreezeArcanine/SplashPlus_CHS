using MAI2.Util;
using Manager;
using Net.Packet;
using Process.MapCore;
using Process.UserDataNet.State;
using Process.UserDataNet.State.PlaylogULState;

namespace Process
{
	public class PlaylogULProcess : MapProcess
	{
		public enum MessageID
		{
			Kill = 100,
			IsDone = 200,
			IsError = 300
		}

		public enum StateType
		{
			Upload
		}

		private ProcessContext<PlaylogULProcess> _context;

		private bool _isDone;

		private bool _isError;

		private int _userIndex;

		public PlaylogULProcess(ProcessDataContainer container)
			: base(container, ProcessType.NetworkProcess)
		{
		}

		public override void OnStart()
		{
			if (Singleton<OperationManager>.Instance.IsAliveServer)
			{
				_context = new ProcessContext<PlaylogULProcess>(this);
				_context.AddState(0, new StateUpload());
				State = StateUserDataCheck;
			}
			else
			{
				TerminateProcess();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_context?.Execute((float)GameManager.GetGameMSecAdd() / 1000f);
		}

		private void StateUserDataCheck()
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(_userIndex).IsEntry && !Singleton<GamePlayManager>.Instance.IsEmpty())
			{
				SetStateTerminate();
				_context.SetState(0, _userIndex);
			}
			else
			{
				NextUserIndex();
			}
		}

		public void NextUserIndex()
		{
			if (++_userIndex >= 2)
			{
				TerminateProcess();
			}
			else
			{
				State = StateUserDataCheck;
			}
		}

		public void OnError(PacketStatus err)
		{
			TerminateProcess();
			_isDone = false;
			_isError = true;
		}

		private void TerminateProcess()
		{
			SetStateTerminate();
			_isDone = true;
		}

		public override object HandleMessage(Message message)
		{
			switch (message.Id)
			{
			case 100:
				container.processManager.ReleaseProcess(this);
				break;
			case 200:
				return _isDone;
			case 300:
				return _isError;
			}
			return null;
		}
	}
}
